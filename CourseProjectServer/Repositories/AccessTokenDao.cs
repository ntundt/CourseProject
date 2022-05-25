using CourseProjectServer.Model;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace CourseProjectServer.Repositories
{

public class AccessTokenDao
{
    private readonly IConfiguration _config;
    public AccessTokenDao(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Generates a random string of 64 hex characters
    /// </summary>
    /// <returns></returns>
    private static string GenerateToken()
    {
        StringBuilder token = new();
        Random random = new();
        byte[] buffer = new byte[32];
        random.NextBytes(buffer);
        foreach (byte b in buffer)
        {
            token.Append(Convert.ToString(b, 16));
        }
        return token.ToString();
    }

    /// <summary>
    /// Creates and saves access token for the user specified
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public AccessToken CreateFor(User user)
    {
        AccessToken token = new()
        {
            Token = GenerateToken(),
            User = user
        };
        Add(token);
        return token;
    }

    /// <summary>
    /// Adds AccessToken object to a database
    /// </summary>
    /// <param name="token"></param>
    /// <exception cref="Exception"></exception>
    private void Add(AccessToken token)
    {
        _ = token ?? throw new ArgumentNullException(nameof(token));
        _ = token.User ?? throw new ArgumentNullException(nameof(token.User));

        var queryString = $"INSERT INTO ACCESS_TOKEN VALUES (@token, @user_id);";

        using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
        connection.Open();
        SqlCommand command = new(queryString, connection);

        SqlParameter tokenParam = new("@token", SqlDbType.NVarChar, 64);
        tokenParam.Value = token.Token;
        command.Parameters.Add(tokenParam);

        SqlParameter userIdParam = new("@user_id", SqlDbType.Int);
        userIdParam.Value = token.User.UserId;
        command.Parameters.Add(userIdParam);

        int rows = command.ExecuteNonQuery();
            
        if (rows != 1)
        {
            throw new Exception($"Exception in AccessTokenRepository.Add(): rows was {rows}");
        }
    }
}
}
