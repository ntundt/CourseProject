using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseProjectServer.Repositories;
using CourseProjectServer.Model;
using DataTransferObject;
using CourseProjectServer.Extension;

namespace CourseProjectServer.Controllers
{
    [Route("api/attempts/{attemptId}/results")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly AttemptDao attemptDao;
        private readonly TestDao testDao;
        private readonly UserDao userDao;
        public ResultsController(IConfiguration config)
        {
            attemptDao = new(config);
            testDao = new(config);
            userDao = new(config);
        }

        [HttpGet]
        public ResultInfo GetResultsInfo([FromRoute] int attemptId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            TestAttempt attempt = attemptDao.GetById(attemptId);

            if (user.UserId != attempt.Testee.UserId && user.UserId != attempt.Test.Author.UserId)
            {
                throw new AccessViolationException();
            }
            if (!attempt.HasEnded())
            {
                throw new InvalidOperationException("Attempt has not ended yet");
            }

            return attemptDao.GetResultsByAttempt(attempt).ToResultInfo();
        }
    }
}
