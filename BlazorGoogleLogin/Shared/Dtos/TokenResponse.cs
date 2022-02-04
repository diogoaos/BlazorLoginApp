using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorGoogleLogin.Shared.Dtos
{
    public class TokenResponse
    {
        public string JwtToken { get; set; } = default!;
    }
}
