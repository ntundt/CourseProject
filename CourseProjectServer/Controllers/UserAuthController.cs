using CourseProjectServer.Repositories;
using CourseProjectServer.Exceptions;
using CourseProjectServer.Model;
using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace CourseProjectServer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly UserDao userDao;
        private readonly AccessTokenDao accessTokenDao;

        public UserAuthController(IConfiguration config)
        {
            userDao = new(config);
            accessTokenDao = new(config);
        }

        private static bool CheckPassword(string password, string passwordHash)
        {
            using SHA256 sha = SHA256.Create();
            return passwordHash.Equals(sha.ComputeHash(Encoding.ASCII.GetBytes(password + "salt")));
        }

        [HttpGet]
        public AuthResult Auth([FromQuery] string login, [FromQuery] string password)
        {
            User loginee;
            try
            {
                loginee = userDao.GetByLogin(login);
                if (loginee.PasswordHash == null)
                {
                    throw new UserDoesNotHavePassword(loginee);
                }
            }
            catch (NotExactlyOneRowException)
            {
                throw new UserNotFoundException();
            }

            if (!CheckPassword(password, loginee.PasswordHash))
            {
                throw new WrongPasswordException();
            }

            AccessToken token = accessTokenDao.CreateFor(loginee);

            return new AuthResult {
                AccessToken = token.Token,
                UserId = loginee.UserId,
                Name = loginee.Name,
                CreatedDate = ((DateTimeOffset)loginee.CreatedDate).ToUnixTimeSeconds()
            };
        }

        [HttpPost]
        public AuthResult Register([FromBody] PostUserAuth name)
        {
            User user = new User
            {
                Name = name.Name
            };

            userDao.Add(user);

            return new AuthResult {
                AccessToken = accessTokenDao.CreateFor(user).Token,
                UserId = user.UserId,
                Name = user.Name,
                CreatedDate = ((DateTimeOffset)user.CreatedDate).ToUnixTimeSeconds()
            };
        }
    }
}
