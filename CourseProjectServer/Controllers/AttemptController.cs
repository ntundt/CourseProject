using CourseProjectServer.Repositories;
using CourseProjectServer.Exceptions;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using AccessViolationException = CourseProjectServer.Exceptions.AccessViolationException;

namespace CourseProjectServer.Controllers
{
    [Route("api/attempts")]
    [ApiController]
    public class AttemptController : ControllerBase
    {
        private readonly AttemptDao attemptDao;
        private readonly UserDao userDao;
        public AttemptController(IConfiguration config)
        {
            attemptDao = new(config);
            userDao = new(config);
        }

        [HttpGet]
        public List<AttemptInfo> GetAttempts()
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            List<TestAttempt> testAttempts = attemptDao.GetByUserId(user.UserId);

            return testAttempts.Select(x => new AttemptInfo() {
                AttemptId = x.AttemptId,
                TestId = x.Test.TestId,
                UserId = x.Testee.UserId,
                Started = ((DateTimeOffset)x.Started).ToUnixTimeSeconds(),
                Ended = ((DateTimeOffset)(x.Ended < new DateTime(1970, 1, 1) ? new DateTime(1970, 1, 1) : x.Ended)).ToUnixTimeSeconds()
            }).ToList();
        }

        [Route("{testAttemptId}/end")]
        [HttpGet]
        public AttemptInfo GetAttempt([FromRoute] int testAttemptId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            TestAttempt testAttempt = attemptDao.GetById(testAttemptId);

            if (user.UserId != testAttempt.Testee.UserId)
            {
                throw new AccessViolationException();
            }

            if (testAttempt.HasEnded())
            {
                throw new AttemptAlreadyEndedException(testAttempt);
            }

            testAttempt.Ended = DateTime.Now;
            attemptDao.Update(testAttempt);

            return new AttemptInfo
            {
                AttemptId = testAttempt.AttemptId,
                TestId = testAttempt.Test.TestId,
                UserId = testAttempt.Testee.UserId,
                Started = ((DateTimeOffset)testAttempt.Started).ToUnixTimeSeconds(),
                Ended = ((DateTimeOffset)testAttempt.Ended).ToUnixTimeSeconds()
            };
        }
    }
}
