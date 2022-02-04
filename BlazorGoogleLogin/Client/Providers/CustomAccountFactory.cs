using BlazorGoogleLogin.Client.Services;
using BlazorGoogleLogin.Shared.Dtos;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorGoogleLogin.Client.Providers
{
    public class CustomAccountFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly AppAuthClient _appAuthClient;
        private readonly IJSRuntime _jsRuntime;

        public CustomAccountFactory(IAccessTokenProviderAccessor accessor, AppAuthClient appAuthClient, IJSRuntime jsRuntime) : base(accessor)
        {
            _appAuthClient = appAuthClient;
            _jsRuntime = jsRuntime;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);
            // var tokenResult = await base.TokenProvider.RequestAccessToken();

            //if (tokenResult != null)
            //{
            //    if (tokenResult.TryGetToken(out var token))
            //    {
            //        Console.WriteLine(token.Value);
            //    }
            //}
            try
            {
                if (initialUser.Identity?.IsAuthenticated == true)
                {
                    var token = await GetToken();
                    var googleUser = new RegisterGoogleUserRequest
                    {
                        Email = initialUser.Claims.Where(_ => _.Type == "email").Select(_ => _.Value).FirstOrDefault()!,
                        FirstName = initialUser.Claims.Where(_ => _.Type == "given_name").Select(_ => _.Value).FirstOrDefault()!,
                        LastName = initialUser.Claims.Where(_ => _.Type == "family_name").Select(_ => _.Value).FirstOrDefault()!,
                        IdToken = token.id_token
                    };

                    var response = await _appAuthClient.RegisterGoogleUser(googleUser);

                    ((ClaimsIdentity)initialUser.Identity).AddClaim(
                        new Claim("APIjwt", response.JwtToken)
                    );

                }
            }
            catch
            {
                initialUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return initialUser;
        }

        private async Task<GoogleToken> GetToken()
        {
            string key = "Microsoft.AspNetCore.Components.WebAssembly.Authentication.CachedAuthSettings";
            string authSettingsRAW = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
            if (authSettingsRAW == null) return new GoogleToken { id_token = "" };
            var authSettings = JsonSerializer.Deserialize<CachedAuthSettings>(authSettingsRAW);
            string userRAW = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", authSettings?.OIDCUserKey);
            return JsonSerializer.Deserialize<GoogleToken>(userRAW)!;
        }
    }

    public class CachedAuthSettings
    {
        public string authority { get; set; } = default!;
        public string metadataUrl { get; set; } = default!;
        public string client_id { get; set; } = default!;
        public string[] defaultScopes { get; set; } = default!;
        public string redirect_uri { get; set; } = default!;
        public string post_logout_redirect_uri { get; set; } = default!;
        public string response_type { get; set; } = default!;
        public string response_mode { get; set; } = default!;
        public string scope { get; set; } = default!;

        public string OIDCUserKey => $"oidc.user:{authority}:{client_id}";
    }

    public class GoogleToken
    {
        public string id_token { get; set; } = default!;
    }
}
