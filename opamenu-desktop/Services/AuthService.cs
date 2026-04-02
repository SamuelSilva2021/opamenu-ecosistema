using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpaMenu.Desktop.Models.DTOs;

namespace OpaMenu.Desktop.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;

    public AuthService(HttpClient httpClient, IConfiguration configuration, TokenStore tokenStore)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _tokenStore = tokenStore;
        
        var baseUrl = _configuration.GetValue<string>("ApiSettings:AuthApiUrl") 
                      ?? throw new InvalidOperationException("ApiSettings:AuthApiUrl não configurado no appsettings.json");
                      
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var payload = new
            {
                usernameOrEmail = "admin@opamenu.com.br",
                password = "Abc@123"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", payload);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();

                if (apiResponse != null && apiResponse.Succeeded && apiResponse.Data != null)
                {
                    _tokenStore.AccessToken = apiResponse.Data.AccessToken;
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);

                    return true;
                }
            }

            return false;
        }
        catch (Exception)
        {
            throw new Exception("Falha ao comunicar com o servidor de autenticação. Verifique se a API está rodando.");
        }
    }

    public string? GetCurrentToken() => _tokenStore.AccessToken;

    public void Logout()
    {
        _tokenStore.AccessToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}