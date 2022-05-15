namespace CourseProjectServer.Model
{
    public class User
    {
        public int UserId { get; set; }
        private string? name;
        public string Name { get => name ?? $"User {UserId}"; set => name = value; }
        public DateTime CreatedDate { get; set; }
        public string? Login { get; set; }
        public string? PasswordHash { get; set; }
    }
}
