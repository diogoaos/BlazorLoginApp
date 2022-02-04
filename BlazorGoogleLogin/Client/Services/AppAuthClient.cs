using BlazorGoogleLogin.Shared.Dtos;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorGoogleLogin.Client.Services
{
    public class AppAuthClient
    {
        private HttpClient _httpClient;
        public AppAuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResponse> RegisterGoogleUser(RegisterGoogleUserRequest googleUserRequest)
        {
            var postData = new StringContent(JsonSerializer.Serialize(googleUserRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Account/register-google-user", postData);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<TokenResponse>())!;
        }
    }
}
