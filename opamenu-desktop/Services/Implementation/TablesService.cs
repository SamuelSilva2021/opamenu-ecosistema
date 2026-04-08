using Microsoft.Extensions.Configuration;
using OpaMenu.Desktop.Models.DTOs.Api;
using OpaMenu.Desktop.Models.DTOs.Tables;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public class TablesService : ITablesService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public TablesService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;

        var baseUrl = configuration.GetValue<string>("ApiSettings:CoreApiUrl")
                      ?? throw new InvalidOperationException("ApiSettings:CoreApiUrl não configurado");

        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    private void EnsureAuthorizationHeader()
    {
        var token = _authService.GetCurrentToken();
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IReadOnlyList<TableFullDto>> GetTablesFullAsync(int pageNumber = 1, int pageSize = 200)
    {
        try
        {
            EnsureAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/api/tables/full?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return Array.Empty<TableFullDto>();

            var json = await response.Content.ReadAsStringAsync();

            var paged = JsonSerializer.Deserialize<PagedResponseDto<TableFullDto>>(json, JsonOptions);
            if (paged?.Succeeded == true && paged.Data != null)
                return paged.Data.Where(t => t.IsActive).ToList();

            return Array.Empty<TableFullDto>();
        }
        catch
        {
            return Array.Empty<TableFullDto>();
        }
    }

    public async Task<TableFullDto?> GetTableFullByIdAsync(Guid tableId)
    {
        try
        {
            EnsureAuthorizationHeader();

            var response = await _httpClient.GetAsync($"/api/tables/{tableId}/full");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var envelope = JsonSerializer.Deserialize<ResponseDto<TableFullDto>>(json, JsonOptions);

            if(envelope?.Data == null)
                return JsonSerializer.Deserialize<TableFullDto>(json, JsonOptions);

            if (envelope?.Succeeded == true)
                return envelope.Data;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task CheckoutTabAsync(Guid tableId, Guid tabId, EPaymentMethod paymentMethod)
    {
        EnsureAuthorizationHeader();

        var request = new TabCheckoutRequestDto
        {
            PaymentMethod = paymentMethod
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/tables/{tableId}/tabs/{tabId}/checkout", request, JsonOptions);
        if (response.IsSuccessStatusCode)
            return;

        var json = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(json);
    }
}
