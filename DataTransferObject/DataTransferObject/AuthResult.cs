namespace DataTransferObject
{
    public class AuthResult
    {
        // токен доступа аользователя
        public string AccessToken { get; set; } = "";
        // id пользователя
        public int UserId { get; set; }
        // Имя пользователя
        public string Name { get; set; } = "";
        // Дата регистрации (unix timestamp, секунды)
        public long CreatedDate { get; set; }
    }
}
