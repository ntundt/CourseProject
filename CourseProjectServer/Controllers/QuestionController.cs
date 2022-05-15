using DataTransferObject;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using CourseProjectServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseProjectServer.Controllers
{
    [Route("api/tests/{testId}/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly TestDao testDao;
        private readonly UserDao userDao;
        private readonly QuestionDao questionDao;
        public QuestionController(IConfiguration config)
        {
            testDao = new(config);
            userDao = new(config);
            questionDao = new(config);
        }

        [HttpGet]
        public List<QuestionInfo> GetQuestionsInfo([FromRoute] int testId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Test test = testDao.GetTestById(testId);

            if (user.UserId != test.Author.UserId)
            {
                throw new AccessViolationException();
            }

            return questionDao.GetQuestionsByTest(test).ToQuestionInfo();
        }

        [HttpPost]
        public PostQuestionResult PostQuestion([FromRoute] int testId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Test test = testDao.GetTestById(testId);

            if (user.UserId != test.Author.UserId)
            {
                throw new AccessViolationException();
            }

            Question question = questionDao.PostQuestion(test);

            return new PostQuestionResult
            {
                QuestionId = question.QuestionId,
                Index = question.Index
            };
        }
    }
}
