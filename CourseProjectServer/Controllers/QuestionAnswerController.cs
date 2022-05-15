using CourseProjectServer.Repositories;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using AccessViolationException = CourseProjectServer.Exceptions.AccessViolationException;

namespace CourseProjectServer.Controllers
{
    [Route("api/attempts/{attemptId}/questions")]
    [ApiController]
    public class QuestionAnswerController : ControllerBase
    {
        private readonly QuestionAnswerDao questionAnswerDao;
        private readonly AttemptDao attemptDao;
        private readonly UserDao userDao;
        public QuestionAnswerController(IConfiguration config)
        {
            questionAnswerDao = new(config);
            attemptDao = new(config);
            userDao = new(config);
        }

        [HttpGet]
        public List<QuestionInfo> GetQuestions([FromRoute] int attemptId)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            TestAttempt attempt = attemptDao.GetById(attemptId);

            if (user.UserId != attempt.Testee.UserId)
            {
                throw new AccessViolationException();
            }

            return questionAnswerDao.GetQuestions(attempt).ToQuestionInfo();
        }


        // TODO: write questionDao.GetQuestion(OngoingTest, int index) for this to be more efficient
        // TODO: validate PutAnswer
        [Route("{index}/answer")]
        [HttpPut]
        public void PutAnswer([FromRoute] int attemptId, [FromRoute] int index, [FromBody] PutAnswer answer)
        {
            User user = userDao.GetByAccessToken(Request.Headers.Authorization);
            TestAttempt attempt = attemptDao.GetById(attemptId);
            List<Question> questions = questionAnswerDao.GetQuestions(attempt);

            if (user.UserId != attempt.Testee.UserId)
            {
                throw new AccessViolationException();
            }

            try 
            {
                Question question = questions.First(x => x.Index == index);
                UserAnswer newUserAnswer = QuestionMapper.ToUserAnswer(attempt, question, answer);

                questionAnswerDao.SetUserAnswer(newUserAnswer);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("There is no question with such index");
            }
        }
    }
}
