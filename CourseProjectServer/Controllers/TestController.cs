using Microsoft.AspNetCore.Mvc;
using CourseProjectServer.Model;
using CourseProjectServer.Repositories;
using DataTransferObject;
using CourseProjectServer.Extension;

namespace CourseProjectServer.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestController : ControllerBase
    {
        readonly UserDao userDao;
        readonly TestDao testDao;

        public TestController(IConfiguration config)
        {
            userDao = new(config);
            testDao = new(config);
        }

        [HttpGet]
        public GetTestsResult GetTests()
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);

            var tests = testDao.GetOwnedTests(user);

            List<TestInfo> testsInfo = new();
            foreach (var test in tests)
            {
                testsInfo.Add(test.ToTestInfo());
            }

            testsInfo.Reverse();

            return new GetTestsResult
            {
                Tests = testsInfo
            };
        }

        [HttpPost]
        public PostTestResult CreateTest([FromQuery] TestInfoSetter info)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);

            Test test = new Test
            {
                Author = user
            };

            info.ApplyTo(test);
            testDao.Add(test);

            return new PostTestResult
            {
                TestId = test.TestId
            };
        }
    }
}
