using BlazorGoogleLogin.Server.Services;
using BlazorGoogleLogin.Server.Shared;
using BlazorGoogleLogin.Shared.Dtos;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlazorGoogleLogin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountLogic _accountLogic;
        private readonly GoogleTokenSettings _googleSettings;
        public AccountController(IAccountLogic accountLogic, IOptions<GoogleTokenSettings> googleSettings)
        {
            _accountLogic = accountLogic;
            _googleSettings = googleSettings.Value;
        }

        [HttpPost]
        [Route("register-google-user")]
        public async Task<IActionResult> RegisterGoogleUser(RegisterGoogleUserRequest googleUserRequest)
        {
            try
            {
                var tokenPayload = await ValidateGoogleToken(googleUserRequest.IdToken);
                if (tokenPayload.GivenName != googleUserRequest.FirstName
                    || tokenPayload.Email != googleUserRequest.Email 
                    || tokenPayload.FamilyName != googleUserRequest.LastName)
                {
                    return BadRequest("The sent data is different from the token.");
                }
                var result = await _accountLogic.RegisterGoogleUser(googleUserRequest);
                return Ok(result);
            }
            catch (InvalidJwtException) 
            {
                return BadRequest("The token sent could not be validated.");
            }
        }

        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string googleTokenId)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            settings.Audience = new List<string>() { _googleSettings.ClientId };
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenId, settings);
            return payload;
        }
    }
}
