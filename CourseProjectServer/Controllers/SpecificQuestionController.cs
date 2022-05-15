using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CourseProjectServer.Repositories;
using CourseProjectServer.Model;
using DataTransferObject;
using CourseProjectServer.Extension;
using System.Text.Json;

namespace CourseProjectServer.Controllers
{
    [Route("api/tests/{testId}/questions/{questionId}")]
    [ApiController]
    public class SpecificQuestionController : ControllerBase
    {
        private readonly QuestionDao questionDao;
        private readonly TestDao testDao;
        private readonly UserDao userDao;
        public SpecificQuestionController(IConfiguration config)
        {
            questionDao = new(config);
            testDao = new(config);
            userDao = new(config);
        }

        [HttpPut]
        public void PutQuestion([FromRoute] int testId, [FromRoute] int questionId, [FromBody] PutQuestion question)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            Question _question = questionDao.GetById(questionId);

            if (_question.Test.TestId != testId)
            {
                throw new AccessViolationException();
            }
            if (user.UserId != _question.Test.Author.UserId)
            {
                throw new AccessViolationException();
            }

            question.ApplyTo(_question);

            questionDao.PutQuestion(_question);
        }
    }
}
