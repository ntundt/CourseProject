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
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password + "salt"));
            var hashString = new StringBuilder();
            foreach (byte x in hash)
            {
                hashString.Append(x.ToString("x2"));
            }
            return hashString.ToString() == passwordHash;
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

        [Route("info")]
        [HttpGet]
        public GetUserInfo GetInfo()
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);

            return new GetUserInfo
            {
                Name = user.Name,
                Login = user.Login
            };
        }

        [HttpPut]
        public DataTransferObject.ActionResult PutUserInfo([FromBody] PutUserInfo userInfo)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);

            if (userInfo.Name != null)
            {
                user.Name = userInfo.Name;
            }
            if (userInfo.Login != null)
            {
                user.Login = userInfo.Login;
            }
            if (userInfo.Password != null)
            {
                var sha256 = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(userInfo.Password + "salt"));
                user.PasswordHash = BitConverter.ToString(sha256).Replace("-", "").ToLower();
            }
            userDao.Update(user);

            return new DataTransferObject.ActionResult { Ok = true };
        }
    }
}
