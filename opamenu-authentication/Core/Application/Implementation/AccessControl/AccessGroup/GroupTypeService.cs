using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.AccessGroup
{
    public class GroupTypeService(
        IMapper mapper,
        IGroupTypeRepository groupTypeRepository
        ) : IGroupTypeService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;

        /// <summary>
        /// Cria um novo tipo de grupo
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<GroupTypeDTO>> CreateAsync(GroupTypeCreateDTO groupType)
        {
            try
            {
                var existingGroupType = await _groupTypeRepository.FindAsync(gt => gt.Name == groupType.Name);

                if (existingGroupType is not null && existingGroupType.Any())
                    return ResponseBuilder<GroupTypeDTO>
                        .Fail(new ErrorDTO { Message = "já existe um tipo de grupo com esse nome." }).WithCode(400).Build();

                var groupTypeEntity = _mapper.Map<GroupTypeEntity>(groupType);

                await _groupTypeRepository.AddAsync(groupTypeEntity);
                var dto = _mapper.Map<GroupTypeDTO>(groupTypeEntity);

                return ResponseBuilder<GroupTypeDTO>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<GroupTypeDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }


        }

        /// <summary>
        /// Deleta um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {

                var existingGroupType = await _groupTypeRepository.GetByIdAsync(id);
                if (existingGroupType == null)
                    return ResponseBuilder<bool>
                        .Fail(new ErrorDTO { Message = "Tipo de grupo nÃ£o encontrado" }).WithCode(404).Build();

                await _groupTypeRepository.DeleteAsync(existingGroupType);
                return ResponseBuilder<bool>.Ok(default).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<bool>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }
        }

        /// <summary>
        /// Recupera todos os tipos de grupo
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseDTO<IEnumerable<GroupTypeDTO>>> GetAllAsync()
        {
            try
            {
                var groupTypeEntities = await _groupTypeRepository.GetAllAsync();
                if(!groupTypeEntities.Any())
                    return ResponseBuilder<IEnumerable<GroupTypeDTO>>
                        .Ok(Enumerable.Empty<GroupTypeDTO>()).Build();

                var groupTypes = _mapper.Map<IEnumerable<GroupTypeDTO>>(groupTypeEntities);
                return ResponseBuilder<IEnumerable<GroupTypeDTO>>.Ok(groupTypes).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<IEnumerable<GroupTypeDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }
            
        }

        /// <summary>
        /// Recupera tipos de grupo paginados
        /// </summary>
        /// <param name="page">Número da página (>=1)</param>
        /// <param name="limit">Itens por página (1-100)</param>
        /// <returns></returns>
        public async Task<ResponseDTO<PagedResponseDTO<GroupTypeDTO>>> GetPagedAsync(int page, int limit)
        {
            try
            {
                // sane defaults and bounds
                if (page < 1) page = 1;
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100;

                var total = await _groupTypeRepository.CountAsync();
                var entities = await _groupTypeRepository.GetPagedAsync(page, limit);
                var items = _mapper.Map<IEnumerable<GroupTypeDTO>>(entities);

                var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit);

                var payload = new PagedResponseDTO<GroupTypeDTO>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = totalPages
                };

                return ResponseBuilder<PagedResponseDTO<GroupTypeDTO>>.Ok(payload).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<PagedResponseDTO<GroupTypeDTO>>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }
        }

        /// <summary>
        /// Recupera um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseDTO<GroupTypeDTO?>> GetByIdAsync(Guid id)
        {
            try
            {
                var groupTypeEntity = await _groupTypeRepository.GetByIdAsync(id);
                if (groupTypeEntity == null)
                    return ResponseBuilder<GroupTypeDTO?>
                        .Fail(new ErrorDTO { Message = "Tipo de grupo nÃ£o encontrado" }).WithCode(404).Build();

                var groupType = _mapper.Map<GroupTypeDTO>(groupTypeEntity);
                return ResponseBuilder<GroupTypeDTO?>.Ok(groupType).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<GroupTypeDTO?>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }
            
        }

        /// <summary>
        /// Atualiza um tipo de grupo pelo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<GroupTypeDTO?>> UpdateAsync(Guid id, GroupTypeUpdateDTO groupType)
        {
            try
            {
                var existingGroupType = await _groupTypeRepository.GetByIdAsync(id);

                if (existingGroupType == null)
                    return ResponseBuilder<GroupTypeDTO?>
                        .Fail(new ErrorDTO { Message = "Tipo de grupo nÃ£o encontrado." }).WithCode(404).Build();

                var duplicateGroupType = await _groupTypeRepository.FindAsync(gt => gt.Name == groupType.Name && gt.Id != id);

                if (duplicateGroupType is not null && duplicateGroupType.Any())
                    return ResponseBuilder<GroupTypeDTO?>
                        .Fail(new ErrorDTO { Message = "já existe um tipo de grupo com esse nome." }).WithCode(400).Build();

                var updatedGroupType = _mapper.Map(groupType, existingGroupType);
                await _groupTypeRepository.UpdateAsync(updatedGroupType);

                var dto = _mapper.Map<GroupTypeDTO>(updatedGroupType);
                return ResponseBuilder<GroupTypeDTO?>.Ok(dto).Build();
            }
            catch (Exception ex)
            {
                return ResponseBuilder<GroupTypeDTO?>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithCode(500).Build();
            }

        }
    }
}

