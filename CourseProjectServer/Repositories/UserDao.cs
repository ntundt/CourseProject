using CourseProjectServer.Exceptions;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using System.Data.SqlClient;

namespace CourseProjectServer.Repositories
{
    // Actual repository
    public class UserDao
    {
        private readonly IConfiguration _config;
        public UserDao(IConfiguration config)
        {
            _config = config;
        }

        public User Add(User user)
        {
            List<string> columns = new List<string>();
            List<string> values = new List<string>();
            if (user.Name != null)
            {
                columns.Add("NAME");
                values.Add($"'{user.Name}'");
            }
            if (user.Login != null)
            {
                columns.Add("LOGIN");
                values.Add($"'{user.Login}'");
            }
            if (user.PasswordHash != null)
            {
                columns.Add("PASSWORD_HASH");
                values.Add($"'{user.PasswordHash}'");
            }

            var queryString =
                $"INSERT INTO \"USER\" ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)});" +
                $"SELECT " +
                $"  SCOPE_IDENTITY() ID, " +
                $"  \"USER\".DATE_CREATED DATE " +
                $"FROM " +
                $"  \"USER\" " +
                $"WHERE \"USER\".ID = SCOPE_IDENTITY();";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            user.UserId = Convert.ToInt32(reader.GetFieldValue<decimal>("ID"));
            user.CreatedDate = reader.GetFieldValue<DateTime>("DATE");

            return user;
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public User GetByLogin(string login)
        {
            var queryString = 
                $"SELECT " +
                $"  \"USER\".ID, " +
                $"  \"USER\".NAME, " +
                $"  \"USER\".DATE_CREATED, " +
                $"  \"USER\".LOGIN, " +
                $"  \"USER\".PASSWORD_SHA256 " +
                $"FROM " +
                $"  \"USER\" " +
                $"WHERE " +
                $"  \"USER\".LOGIN = '{login}'";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                throw new UserNotFoundException();
            }

            return new User
            {
                UserId = reader.GetInt32(0),
                Name = reader.GetString(1),
                CreatedDate = reader.GetDateTime(2),
                Login = reader.IsDBNull(3) ? null : reader.GetString(3),
                PasswordHash = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }

        public User GetByAccessToken(string accessToken)
        {
            var queryString = 
                $"SELECT " +
                $"  \"USER\".ID, " +
                $"  \"USER\".NAME, " +
                $"  \"USER\".DATE_CREATED, " +
                $"  \"USER\".LOGIN, " +
                $"  \"USER\".PASSWORD_SHA256 " +
                $"FROM " +
                $"  \"USER\" JOIN ACCESS_TOKEN ON ACCESS_TOKEN.USER_ID = \"USER\".ID " +
                $"WHERE " +
                $"  ACCESS_TOKEN.TOKEN = '{accessToken}'";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                throw new AccessTokenInvalidException();
            }

            return new User
            {
                UserId = reader.GetInt32(0),
                Name = reader.GetString(1),
                CreatedDate = reader.GetDateTime(2),
                Login = reader.IsDBNull(3) ? null : reader.GetString(3),
                PasswordHash = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
    }
}
