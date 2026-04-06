using Microsoft.Extensions.Configuration;
using OpaMenu.Desktop.Models.DTOs.Api;
using OpaMenu.Desktop.Models.DTOs.Pdv;
using OpaMenu.Desktop.Models.DTOs.Requests;
using OpaMenu.Desktop.Models.Enums;
using OpaMenu.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services.Implementation;

public class CashRegisterService : ICashRegisterService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public CashRegisterService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
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
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Usuário não autenticado.");

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<CashShiftDto?> GetActiveShiftAsync()
    {
        EnsureAuthorizationHeader();

        var response = await _httpClient.GetAsync("/api/cash-register/active");
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
                return null;

            return JsonSerializer.Deserialize<CashShiftDto>(json, JsonOptions);
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    public async Task<CashShiftDto> OpenShiftAsync(decimal openingBalance)
    {
        EnsureAuthorizationHeader();

        var request = new OpenCashShiftRequestDto { OpeningBalance = openingBalance };
        var response = await _httpClient.PostAsJsonAsync("/api/cash-register/open", request, JsonOptions);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var shift = JsonSerializer.Deserialize<CashShiftDto>(json, JsonOptions);
            return shift ?? throw new InvalidOperationException("Resposta inválida ao abrir caixa.");
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    public async Task<CashShiftDto> CloseShiftAsync(decimal closingBalance)
    {
        EnsureAuthorizationHeader();

        var request = new CloseCashShiftRequestDto { ClosingBalance = closingBalance, DiscrepancyJustification = null };
        var response = await _httpClient.PostAsJsonAsync("/api/cash-register/close", request, JsonOptions);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var shift = JsonSerializer.Deserialize<CashShiftDto>(json, JsonOptions);
            return shift ?? throw new InvalidOperationException("Resposta inválida ao fechar caixa.");
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    public async Task<CashShiftCloseSummaryDto> CloseShiftWithSummaryAsync(decimal closingBalance, string? justification)
    {
        EnsureAuthorizationHeader();

        var request = new CloseCashShiftRequestDto { ClosingBalance = closingBalance, DiscrepancyJustification = justification };
        var response = await _httpClient.PostAsJsonAsync("/api/cash-register/close/summary", request, JsonOptions);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var summary = JsonSerializer.Deserialize<CashShiftCloseSummaryDto>(json, JsonOptions);
            return summary ?? throw new InvalidOperationException("Resposta inválida ao fechar caixa.");
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    public async Task<CashShiftSummaryDto?> GetActiveShiftSummaryAsync()
    {
        EnsureAuthorizationHeader();

        var response = await _httpClient.GetAsync("/api/cash-register/active/summary");
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
                return null;

            return JsonSerializer.Deserialize<CashShiftSummaryDto>(json, JsonOptions);
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    public async Task<CashMovementDto> AddMovementAsync(ECashMovementType type, decimal amount, string description)
    {
        EnsureAuthorizationHeader();

        var request = new AddCashMovementRequestDto
        {
            Type = (int)type,
            Amount = amount,
            Description = description,
            PaymentMethod = null
        };

        var response = await _httpClient.PostAsJsonAsync("/api/cash-register/movement", request, JsonOptions);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var movement = JsonSerializer.Deserialize<CashMovementDto>(json, JsonOptions);
            return movement ?? throw new InvalidOperationException("Resposta inválida ao registrar movimentação.");
        }

        throw new InvalidOperationException(ParseErrorsOrRaw(json, response));
    }

    private static string ParseErrorsOrRaw(string json, HttpResponseMessage response)
    {
        try
        {
            var errors = JsonSerializer.Deserialize<List<ApiErrorDto>>(json, JsonOptions);
            if (errors is { Count: > 0 })
                return string.Join(Environment.NewLine, errors.Select(e => e.ToString()));
        }
        catch
        {
        }

        return $"HTTP {(int)response.StatusCode} - {json}";
    }
}
