using AutoMapper;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.BankDetails;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Services.Opamenu
{
    public class BankDetailsService(
        IMapper mapper,
        IBankDetailsRepository bankDetailsRepository,
        ICurrentUserService currentUserService
        ) : IBankDetailsService
    {

        public async Task<ResponseDTO<BankDetailsDto>> CreateAsync(CreateBankDetailsRequestDto request)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid()!.Value;

                var entity = mapper.Map<BankDetailsEntity>(request);
                entity.TenantId = tenantId;

                await bankDetailsRepository.AddAsync(entity);

                var entities = await bankDetailsRepository.GetByTenantIdAsync(tenantId);
                
                var bankDetail = entities.FirstOrDefault(b => b.PixKey == request.PixKey);
                if (bankDetail!.IsPixKeySelected)
                {
                    foreach (var bank in entities)
                    {
                        bank.IsPixKeySelected = bank.Id == bankDetail.Id;
                        bank.UpdatedAt = DateTime.UtcNow;
                    }
                    await bankDetailsRepository.UpdateRangeAsync(entities);
                }

                var dto = mapper.Map<BankDetailsDto>(entity);

                return StaticResponseBuilder<BankDetailsDto>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<BankDetailsDto>.BuildErrorResponse(ex);
            }
            
        }

        public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid()!.Value;

                var entity = await bankDetailsRepository.GetByIdAsync(tenantId, id) ?? throw new Exception("Não foi encontrado dados bancários");
                await bankDetailsRepository.DeleteAsync(entity);

                return StaticResponseBuilder<bool>.BuildOk(true);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<BankDetailsDto>>> GetAllAsync()
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid()!.Value;

                var entities = await bankDetailsRepository.GetByTenantIdAsync(tenantId);
                var dtos = mapper.Map<IEnumerable<BankDetailsDto>>(entities);
                return StaticResponseBuilder<IEnumerable<BankDetailsDto>>.BuildOk(dtos);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<BankDetailsDto>>.BuildErrorResponse(ex);
            }
            
        }

        public async Task<ResponseDTO<BankDetailsDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid()!.Value;

                var entity = await bankDetailsRepository.GetByIdAsync(tenantId, id);
                var dto = mapper.Map<BankDetailsDto>(entity);

                return StaticResponseBuilder<BankDetailsDto>.BuildOk(dto);

            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<BankDetailsDto>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<BankDetailsDto>> UpdateAsync(UpdateBankDetailsRequestDto request)
        {
            try
            {
                var tenantId = currentUserService.GetTenantGuid()!.Value;

                var entity = await bankDetailsRepository.GetByIdAsync(tenantId, request.Id);

                entity!.BankName = request.BankName;
                entity.Agency = request.Agency;
                entity.AccountNumber = request.AccountNumber;
                entity.AccountType = request.AccountType;
                entity.BankId = request.BankId;
                entity.PixKey = request.PixKey;
                entity.IsPixKeySelected = (bool)request.IsPixKeySelected!;

                await bankDetailsRepository.UpdateAsync(entity);

                var dto = mapper.Map<BankDetailsDto>(entity);

                return StaticResponseBuilder<BankDetailsDto>.BuildOk(dto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<BankDetailsDto>.BuildErrorResponse(ex);
            }
        }
    }
}
