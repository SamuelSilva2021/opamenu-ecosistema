using Microsoft.Extensions.Configuration;
using OpaMenu.Desktop.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public CatalogService(HttpClient httpClient, IConfiguration configuration, IAuthService authService)
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
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            EnsureAuthorizationHeader();
            var response = await _httpClient.GetAsync("/api/Categories");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                try 
                {
                    // Tenta ler como envelope ApiResponse
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<IEnumerable<CategoryDto>>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }
                catch
                {
                    // Se falhar, é porque a API retornou o array diretamente
                    var directArray = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CategoryDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (directArray != null)
                    {
                        return directArray;
                    }
                }
            }
            return new List<CategoryDto>();
        }
        catch (Exception)
        {
            return new List<CategoryDto>();
        }
    }

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        try
        {
            EnsureAuthorizationHeader();
            // A rota base /api/products sem query params deve retornar todos os produtos permitidos para o tenant
            var response = await _httpClient.GetAsync("/api/products");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                try 
                {
                    // Tenta ler como envelope ApiResponse
                    var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<IEnumerable<ProductDto>>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }
                catch
                {
                    // Se falhar, é porque a API retornou o array diretamente
                    var directArray = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ProductDto>>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (directArray != null)
                    {
                        return directArray;
                    }
                }
            }
            return new List<ProductDto>();
        }
        catch (Exception)
        {
            return new List<ProductDto>();
        }
    }
}