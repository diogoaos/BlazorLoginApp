using BlazorGoogleLogin.Shared.Dtos;

namespace BlazorGoogleLogin.Server.Services
{
    public interface IAccountLogic
    {
        Task<TokenResponse> RegisterGoogleUser(RegisterGoogleUserRequest googleUserModel);
    }
}
