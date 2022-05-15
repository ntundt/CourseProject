namespace DataTransferObject
{
    public class AuthResult
    {
        public string AccessToken { get; set; } = "";
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public long CreatedDate { get; set; }
    }
}
