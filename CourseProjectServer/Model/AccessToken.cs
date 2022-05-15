namespace CourseProjectServer.Model
{
    public class AccessToken
    {
        public string Token { get; set; } = "";
        public User? User { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; } = DateTime.Now.AddMonths(1);
    }
}
