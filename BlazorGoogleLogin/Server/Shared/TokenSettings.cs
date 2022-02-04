namespace BlazorGoogleLogin.Server.Shared
{
    public class TokenSettings
	{
		public string Issuer { get; set; } = default!;
		public string Audience { get; set; } = default!;
		public string Key { get; set; } = default!;
	}
}
