using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Application.Interfaces.Auth;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.UserAccounts
{
    /// <summary>
    /// Serviço para gerenciamento de contas de usuários
    /// </summary>
    public class UserAccountService(
        IUserAccountsRepository userRepository,
        IMapper mapper,
        IUserContext userContext,
        IJwtTokenService jwtTokenService,
        ILogger<UserAccountService> logger
    ) : IUserAccountService
    {
        private readonly IUserAccountsRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IUserContext _userContext = userContext;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly ILogger<UserAccountService> _logger = logger;

        public async Task<ResponseDTO<PagedResponseDTO<UserAccountDTO>>> GetAllUserAccountsPagedAsync(int page, int limit)
        {
            try
            {
                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var entities = await _userRepository.GetUsersPagedAsync(page, limit);
                var total = entities.Count();
                var items = _mapper.Map<IEnumerable<UserAccountDTO>>(entities);

                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);
                var pagedResult = new PagedResponseDTO<UserAccountDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };

                return ResponseBuilder<PagedResponseDTO<UserAccountDTO>>.Ok(pagedResult).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários paginados");
                return ResponseBuilder<PagedResponseDTO<UserAccountDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<IEnumerable<UserAccountDTO>>> GetAllActiveUsersAsync()
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                {
                    return ResponseBuilder<IEnumerable<UserAccountDTO>>
                        .Fail(new ErrorDTO { Message = "Tenant não identificado" })
                        .WithCode(400)
                        .Build();
                }

                var users = await _userRepository.GetActiveUsersByTenantAsync(tenantId.Value);
                var items = _mapper.Map<IEnumerable<UserAccountDTO>>(users);
                return ResponseBuilder<IEnumerable<UserAccountDTO>>.Ok(items).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários ativos");
                return ResponseBuilder<IEnumerable<UserAccountDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> GetUserAccountByIdAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || (tenantId.HasValue && user.TenantId != tenantId))
                {
                    return ResponseBuilder<UserAccountDTO>
                        .Fail(new ErrorDTO { Message = "Usuário não encontrado" })
                        .WithCode(404)
                        .Build();
                }

                var dto = _mapper.Map<UserAccountDTO>(user);
                return ResponseBuilder<UserAccountDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário por ID");
                return ResponseBuilder<UserAccountDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> AddUserAccountAsync(UserAccountCreateDTO dto)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(dto.Email))
                {
                    return ResponseBuilder<UserAccountDTO>
                        .Fail(new ErrorDTO { Message = "Email já¡ está em uso" })
                        .WithCode(400)
                        .Build();
                }
                var userName = dto.Email.Split('@')[0];

                if (await _userRepository.UsernameExistsAsync(userName))
                {
                    userName = $"{userName}{new Random().Next(1000, 9999)}";
                    while (await _userRepository.UsernameExistsAsync(userName))
                    {
                        userName = $"{userName}{new Random().Next(1000, 9999)}";
                    }
                }

                var entity = _mapper.Map<UserAccountEntity>(dto);
                entity.Username = userName;
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                if(dto.TenantId == Guid.Empty)
                    entity.TenantId = null;
                else
                    entity.TenantId = dto.TenantId;

                var created = await _userRepository.AddAsync(entity);
                var createdDto = _mapper.Map<UserAccountDTO>(created);
                return ResponseBuilder<UserAccountDTO>.Ok(createdDto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                return ResponseBuilder<UserAccountDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> UpdateUserAccountAsync(Guid id, UserAccountUpdateDTO dto)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailExists = await _userRepository.EmailExistsAsync(dto.Email, id);
                    if (emailExists)
                    {
                        return ResponseBuilder<UserAccountDTO>
                            .Fail(new ErrorDTO { Message = "Email já está em uso." })
                            .WithCode(400)
                            .Build();
                    }
                }

                if (!string.IsNullOrWhiteSpace(dto.Username))
                {
                    var usernameExists = await _userRepository.UsernameExistsAsync(dto.Username, id);
                    if (usernameExists)
                    {
                        return ResponseBuilder<UserAccountDTO>
                            .Fail(new ErrorDTO { Message = "Username já está em uso." })
                            .WithCode(400)
                            .Build();
                    }
                }

                var updatedEntity = _mapper.Map(dto, existing);

                if(dto.TenantId == Guid.Empty || dto.TenantId == null)
                    updatedEntity.TenantId = null;

                await _userRepository.UpdateAsync(updatedEntity);
                var updatedDto = _mapper.Map<UserAccountDTO>(updatedEntity);
                return ResponseBuilder<UserAccountDTO>.Ok(updatedDto).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário");
                return ResponseBuilder<UserAccountDTO>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> DeleteUserAccountAsync(Guid id)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (existing == null || (tenantId.HasValue && existing.TenantId != tenantId))
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Usuário não encontrado" })
                        .WithCode(404)
                        .Build();
                }

                await _userRepository.DeleteAsync(existing);
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário.");
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        //public async Task<ResponseDTO<bool>> ChangePasswordAsync(UserAccountChangePasswordDTO dto)
        //{
        //    try
        //    {
        //        var currentUserId = _userContext.CurrentUser?.UserId;
        //        if (!currentUserId.HasValue)
        //        {
        //            return ResponseBuilder<bool>
        //                .Fail(new ErrorDTO { Message = "usuário nÃ£o autenticado" })
        //                .WithCode(401)
        //                .Build();
        //        }

        //        var user = await _userRepository.GetByIdAsync(currentUserId.Value);
        //        if (user == null)
        //        {
        //            return ResponseBuilder<bool>
        //                .Fail(new ErrorDTO { Message = "usuário nÃ£o encontrado" })
        //                .WithCode(404)
        //                .Build();
        //        }

        //        var isValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
        //        if (!isValid)
        //        {
        //            return ResponseBuilder<bool>
        //                .Fail(new ErrorDTO { Message = "Senha atual invÃ¡lida" })
        //                .WithCode(400)
        //                .Build();
        //        }

        //        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        //        user.UpdatedAt = DateTime.UtcNow;
        //        await _userRepository.UpdateAsync(user);
        //        return ResponseBuilder<bool>.Ok(true).Build();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao alterar senha");
        //        return ResponseBuilder<bool>
        //            .Fail(new ErrorDTO { Message = ex.Message })
        //            .WithException(ex)
        //            .WithCode(500)
        //            .Build();
        //    }
        //}

        public async Task<ResponseDTO<bool>> ForgotPasswordAsync(UserAccountForgotPasswordDTO dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ResponseBuilder<bool>.Ok(true).Build();
                }

                var resetToken = _jwtTokenService.GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddHours(2);
                await _userRepository.SetPasswordResetTokenAsync(user.Id, resetToken, expiresAt);

                // TODO: Enviar email com o token (fora do escopo)
                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar fluxo de esqueci a senha");
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        public async Task<ResponseDTO<bool>> ResetPasswordAsync(UserAccountResetPasswordDTO dto)
        {
            try
            {
                var user = await _userRepository.GetByValidPasswordResetTokenAsync(dto.ResetToken);
                if (user == null)
                {
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Token de reset inválido ou expirado" })
                        .WithCode(400)
                        .Build();
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                user.PasswordResetToken = null;
                user.PasswordResetExpiresAt = null;
                await _userRepository.UpdateAsync(user);

                return ResponseBuilder<bool>.Ok(true).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar senha");
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }
    }
}
