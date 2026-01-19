using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using AutoMapper;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;

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

        public async Task<ResponseDTO<PagedResponseDTO<UserAccountWithGroupsDTO>>> GetAllUserAccountsPagedAsync(int page, int limit)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;

                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var userAccountEntities = await _userRepository.GetUsersPagedAsync(page, limit);

                var total = userAccountEntities.Count();

                if (!userAccountEntities.Any())
                {
                    var pagedNullResult = new PagedResponseDTO<UserAccountWithGroupsDTO>
                    {
                        Items = Enumerable.Empty<UserAccountWithGroupsDTO>(),
                        Page = page,
                        Limit = limit,
                        Total = 0,
                        TotalPages = 0
                    };
                    return StaticResponseBuilder<PagedResponseDTO<UserAccountWithGroupsDTO>>.BuildOk(pagedNullResult);
                }

                var items = _mapper.Map<IEnumerable<UserAccountWithGroupsDTO>>(userAccountEntities);

                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);
                var pagedResult = new PagedResponseDTO<UserAccountWithGroupsDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };
                return StaticResponseBuilder<PagedResponseDTO<UserAccountWithGroupsDTO>>.BuildOk(pagedResult);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PagedResponseDTO<UserAccountWithGroupsDTO>>.BuildErrorResponse(ex);
            }
        }
        public async Task<ResponseDTO<PagedResponseDTO<UserAccountDTO>>> GetAllUserAccountsByTenantIdPagedAsync(int page, int limit)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildError("TenantId não fornecido para filtrar usuários.");

                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var userAccountEntities = await _userRepository.GetUsersByTenantPagedAsync(tenantId.Value, page, limit);
                var total = userAccountEntities.Count();

                if (!userAccountEntities.Any())
                {
                    var pagedNullResult = new PagedResponseDTO<UserAccountDTO>
                    {
                        Items = Enumerable.Empty<UserAccountDTO>(),
                        Page = page,
                        Limit = limit,
                        Total = 0,
                        TotalPages = 0
                    };
                    return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildOk(pagedNullResult);
                }
                var items = _mapper.Map<IEnumerable<UserAccountDTO>>(userAccountEntities);
                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);
                var pagedResult = new PagedResponseDTO<UserAccountDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };
                return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildOk(pagedResult);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<PagedResponseDTO<UserAccountDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<UserAccountDTO>>> GetAllActiveUsersAsync()
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<IEnumerable<UserAccountDTO>>.BuildError("TenantId não fornecido para filtrar usuários.");

                var users = await _userRepository.GetActiveUsersByTenantAsync(tenantId.Value);

                if (users == null)
                    return StaticResponseBuilder<IEnumerable<UserAccountDTO>>.BuildOk(null!);

                var items = _mapper.Map<IEnumerable<UserAccountDTO>>(users);
                return StaticResponseBuilder<IEnumerable<UserAccountDTO>>.BuildOk(items);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<UserAccountDTO>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> GetUserAccountByIdAsync(Guid id)
        {
            try
            {
                var tenantId = _userContext.CurrentUser?.TenantId;
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null || (tenantId.HasValue && user.TenantId != tenantId))
                    return StaticResponseBuilder<UserAccountDTO>.BuildOk(null!);

                var dto = _mapper.Map<UserAccountDTO>(user);
                return StaticResponseBuilder<UserAccountDTO>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> AddUserAccountAsync(UserAccountCreateDTO dto)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(dto.Email))
                    return StaticResponseBuilder<UserAccountDTO>.BuildError("Email já está em uso.");

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
                return StaticResponseBuilder<UserAccountDTO>.BuildCreated(createdDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> UpdateUserAccountAsync(Guid id, UserAccountUpdateDto dto)
        {
            try
            {
                var userAccountEntity = await _userRepository.GetByIdAsync(id);

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailExists = await _userRepository.EmailExistsAsync(dto.Email, id);
                    if (emailExists)
                        return StaticResponseBuilder<UserAccountDTO>.BuildError("Email já está em uso.");
                }

                if (!string.IsNullOrWhiteSpace(dto.Username))
                {
                    var usernameExists = await _userRepository.UsernameExistsAsync(dto.Username, id);
                    if (usernameExists)
                        return StaticResponseBuilder<UserAccountDTO>.BuildError("Nome de usuário já está em uso.");
                }

                var updatedEntity = _mapper.Map(dto, userAccountEntity);

                await _userRepository.UpdateAsync(updatedEntity!);
                var updatedDto = _mapper.Map<UserAccountDTO>(updatedEntity);
                return StaticResponseBuilder<UserAccountDTO>.BuildOk(updatedDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<UserAccountDTO>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteUserAccountAsync(Guid id)
        {
            try
            {
                var existing = await _userRepository.GetByIdAsync(id);
                if (existing == null)
                    return StaticResponseBuilder<bool>.BuildOk(false);

                await _userRepository.DeleteAsync(existing);
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
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
                    return StaticResponseBuilder<bool>.BuildOk(true);

                var resetToken = _jwtTokenService.GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddHours(2);
                await _userRepository.SetPasswordResetTokenAsync(user.Id, resetToken, expiresAt);

                // TODO: Enviar email com o token (fora do escopo)
                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar reset de senha");
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<bool>> ResetPasswordAsync(UserAccountResetPasswordDTO dto)
        {
            try
            {
                var user = await _userRepository.GetByValidPasswordResetTokenAsync(dto.ResetToken);
                if (user == null)
                    return StaticResponseBuilder<bool>.BuildError("Token de reset de senha invÃ¡lido ou expirado.");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                user.PasswordResetToken = null;
                user.PasswordResetExpiresAt = null;
                await _userRepository.UpdateAsync(user);
                return StaticResponseBuilder<bool>.BuildOk(true);

            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<UserAccountDTO>> CreateUserAccountAdminAsync(Guid tenantId, CreateTenantDTO tenantDto)
        {
            try
            {
                //Criar o usuário Administrador
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(tenantDto.Password);
                var userName = await GenerateUniqueUsernameAsync(tenantDto.Email);

                var adminUser = new UserAccountEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Username = userName,
                    Email = tenantDto.Email.ToLower().Trim(),
                    PasswordHash = passwordHash,
                    FirstName = tenantDto.FirstName.Trim(),
                    LastName = tenantDto.LastName.Trim(),
                    PhoneNumber = tenantDto.UserPhone?.Trim(),
                    Status = EUserAccountStatus.Inativo,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow,
                };
                await _userRepository.AddAsync(adminUser);
                _logger.LogInformation("usuário administrador criado com sucesso: {UserId}", adminUser.Id);

                var adminDto = _mapper.Map<UserAccountDTO>(adminUser);
                return ResponseBuilder<UserAccountDTO>.Ok(adminDto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<UserAccountDTO>.Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
            
        }

        /// <summary>
        /// Gera um nome de usuário Único baseado no email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<string> GenerateUniqueUsernameAsync(string email)
        {
            var baseUsername = email.Split('@')[0].ToLower();
            var username = baseUsername;
            var counter = 1;

            while (await _userRepository.AnyAsync(u => u.Username == username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }
    }
}
