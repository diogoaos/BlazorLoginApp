namespace BlazorGoogleLogin.Server.Model
{
    public class UserRole
    {
        public int Id { get; set; }
        public List<User> Users { get; set; } = new();
        public string? Name { get; set; }
    }
}
