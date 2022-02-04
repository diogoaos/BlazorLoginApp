using BlazorGoogleLogin.Server.Data;
using BlazorGoogleLogin.Server.Model;
using BlazorGoogleLogin.Server.Shared;
using BlazorGoogleLogin.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorGoogleLogin.Server.Services
{
    public class AccountLogic : IAccountLogic
    {
        private readonly ApplicationContext _context;
        private readonly TokenSettings _tokenSettings;

        public AccountLogic(ApplicationContext context, IOptions<TokenSettings> tokenSettings)
        {
            _context = context;
            _tokenSettings = tokenSettings.Value;
        }
        public async Task<TokenResponse> RegisterGoogleUser(RegisterGoogleUserRequest googleUserRequest)
        {
            User? user = await _context.User.FirstOrDefaultAsync(u => u.EmailAddress == googleUserRequest.Email);
            if (user == null)
            {
                user = await CreateNewUser(googleUserRequest);
            }
            return new TokenResponse { JwtToken = GetJWTAuthKey(user) };
        }

        private async Task<User> CreateNewUser(RegisterGoogleUserRequest googleUserRequest)
        {
            var newUser = new User
            {
                EmailAddress = googleUserRequest.Email,
                FirstName = googleUserRequest.FirstName,
                LastName = googleUserRequest.LastName
            };

            newUser.Roles.Add(await GetOrCreateRole("admin"));

            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        private async Task<UserRole> GetOrCreateRole(string roleName)
        {
            var role = await _context.UserRoles.SingleOrDefaultAsync(r => r.Name == roleName);
            return role ?? new UserRole { Name = roleName };
        }

        private string GetJWTAuthKey(User user)
        {
            var securtityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));

            var credentials = new SigningCredentials(securtityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            if (user.Roles is not null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name!));
                }
            }
            
            claims.Add(new Claim(ClaimTypes.Name, user.FirstName! + user.LastName!));
            claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress!));

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
