using Microsoft.Extensions.Configuration;
using OpaMenu.Desktop.Models.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly TokenStore _tokenStore;
    private readonly UserStore _userStore;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public AuthService(HttpClient httpClient, IConfiguration configuration, TokenStore tokenStore, UserStore userStore)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _tokenStore = tokenStore;
        _userStore = userStore;

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
                usernameOrEmail = email,
                password
            };


            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", payload, JsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>(JsonOptions);

                if (apiResponse != null && apiResponse.Succeeded && apiResponse.Data != null)
                {
                    _tokenStore.AccessToken = apiResponse.Data.AccessToken;
                    
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenStore.AccessToken);

                    var responseUser = await _httpClient.GetAsync("/api/auth/me");
                    if (responseUser.IsSuccessStatusCode)
                    {
                        var userInfoResponse = await responseUser.Content.ReadFromJsonAsync<ApiResponse<UserInfo>>(JsonOptions);
                        if (userInfoResponse?.Succeeded == true && userInfoResponse.Data != null)
                        {
                            _userStore.Id = userInfoResponse.Data.Id;
                            _userStore.Name = userInfoResponse.Data.Username;
                            _userStore.Email = userInfoResponse.Data.Email;
                        }
                    }

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
        _userStore.Id = null;
        _userStore.Name = null;
        _userStore.Email = null;
    }
}
