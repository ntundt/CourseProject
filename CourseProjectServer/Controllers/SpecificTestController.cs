using CourseProjectServer.Repositories;
using CourseProjectServer.Model;
using DataTransferObject;
using CourseProjectServer.Extension;
using CourseProjectServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using AccessViolationException = CourseProjectServer.Exceptions.AccessViolationException;

namespace CourseProjectServer.Controllers
{
    [ApiController]
    [Route("api/tests/{testId}")]
    public class SpecificTestController : ControllerBase
    {
        private readonly UserDao userDao;
        private readonly TestDao testDao;
        private readonly AttemptDao attemptDao;

        public SpecificTestController(IConfiguration config)
        {
            testDao = new(config);
            userDao = new(config);
            attemptDao = new(config);
        }

        [HttpGet]
        public TestInfo GetTest([FromRoute] int testId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Test test = testDao.GetTestById(testId);
            
            if (test.IsPrivate() && test.Author.UserId != user.UserId)
            {
                throw new CanNotAccessPrivateTestException(testId);
            }

            return test.ToTestInfo();
        }

        [HttpPut]
        public TestInfo PutTest([FromRoute] int testId, [FromBody] TestInfoSetter info)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Test test = testDao.GetTestById(testId);

            if (test.Author.UserId != user.UserId)
            {
                throw new CanNotAccessOtherUsersTestException();
            }

            info.ApplyTo(test);

            testDao.Update(test);

            return testDao.GetTestById(test.TestId).ToTestInfo();
        }

        [Route("start")]
        [HttpGet]
        public GetStartTestResult StartTest([FromRoute] int testId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Test test = testDao.GetTestById(testId);

            if (test.IsPrivate() && test.Author.UserId != user.UserId)
            {
                throw new CanNotAccessPrivateTestException(testId);
            }
            if (attemptDao.GetUsedAttemptCount(user.UserId, testId) > test.Attempts)
            {
                throw new TooManyAttemptsException(testId, test.Attempts);
            }

            var attempt = attemptDao.AddFor(user, test);

            return new GetStartTestResult
            {
                AttemptId = attempt.AttemptId,
                Started = ((DateTimeOffset)attempt.Started).ToUnixTimeSeconds(),
                Limit = attempt.Test.HasTimeLimit() ? ((DateTimeOffset)(attempt.Started + attempt.Test.TimeLimit)).ToUnixTimeSeconds() : null
            };
        }
    }
}
