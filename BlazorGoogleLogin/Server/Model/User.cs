namespace BlazorGoogleLogin.Server.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefershTokenExpiration { get; set; }
        public List<UserRole> Roles { get; set; } = new ();
    }
}
