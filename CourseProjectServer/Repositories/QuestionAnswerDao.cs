using DataTransferObject;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using System.Data.SqlClient;
using System.Data;

namespace CourseProjectServer.Repositories
{
    public class QuestionAnswerDao
    {
        private IConfiguration _config;
        public QuestionAnswerDao(IConfiguration config)
        {
            _config = config;
        }

        public void SetUserAnswer(UserAnswer newUserAnswer)
        {
            _ = newUserAnswer ?? throw new ArgumentNullException(nameof(newUserAnswer));
            _ = newUserAnswer.Question ?? throw new ArgumentNullException(nameof(newUserAnswer.Question));

            var values = "";

            if (newUserAnswer.AnswerOption != null)
            {
                foreach (AnswerOption answerOption in newUserAnswer.AnswerOption)
                {
                    values += (values.Equals("") ? "" : ",") +
                        $"({newUserAnswer.TestAttempt.AttemptId}, {answerOption.AnswerOptionId}, NULL, NULL)";
                }
            }
            else if (newUserAnswer.Answer != null)
            {
                values +=
                    $"(" +
                    $"{newUserAnswer.TestAttempt.AttemptId}, " +
                    $"NULL, " +
                    $"(" +
                    $"  SELECT " +
                    $"    ID " +
                    $"  FROM " +
                    $"    QUESTION " +
                    $"  WHERE " +
                    $"    TEST_ID = {newUserAnswer.TestAttempt.Test.TestId} " +
                    $"    AND QUESTION_INDEX = {newUserAnswer.Question.Index}" +
                    $"), " +
                    $"@answer)";
            }

            var queryString =
                $"DELETE ua " +
                $"FROM USER_ANSWER ua " +
                $"LEFT JOIN ANSWER_OPTION q ON ua.ANSWER_ID = q.ID " +
                $"WHERE ua.QUESTION_ID = @question_id OR q.QUESTION_ID = @question_id;" +
                $"INSERT INTO USER_ANSWER (ONGOING_TEST_ID, ANSWER_ID, QUESTION_ID, ANSWER)" +
                $"  VALUES" +
                $"{values}";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter question_id = new("@question_id", SqlDbType.Int);
            question_id.Value = newUserAnswer.Question.QuestionId;
            command.Parameters.Add(question_id);

            if (newUserAnswer.Answer != null)
            {
                SqlParameter answer = new("@answer", SqlDbType.NVarChar, 500);
                answer.Value = newUserAnswer.Answer;
                command.Parameters.Add(answer);
            }

            command.Prepare();
            command.ExecuteNonQuery();
        }

        public List<Question> GetQuestions(TestAttempt attempt)
        {
            var queryString =
                $"SELECT " +
                $"    QUESTION.ID QUESTION_ID, " +
                $"    QUESTION.QUESTION, " +
                $"    QUESTION.QUESTION_INDEX, " +
                $"    QUESTION.CHECK_ALGORITHM, " +
                $"    QUESTION.TYPE, " +

                $"    ANSWER_OPTION.ID ANSWER_OPTION_ID, " +
                $"    ANSWER_OPTION.QUESTION_ID, " +
                $"    ANSWER_OPTION.ANSWER ANSWER_OPTION_ANSWER, " +
                $"    ANSWER_OPTION.CORRECT, " +

                $"    USER_ANSWER.ID USER_ANSWER_ID, " +
                $"    USER_ANSWER.ANSWER_ID, " +
                $"    USER_ANSWER.ANSWER USER_ANSWER_ANSWER " +
                $"FROM " +
                $"    ONGOING_TEST " +
                $"    LEFT JOIN QUESTION ON ONGOING_TEST.TEST_ID = QUESTION.TEST_ID " +
                $"    LEFT JOIN ANSWER_OPTION ON QUESTION.ID = ANSWER_OPTION.QUESTION_ID " +
                $"    LEFT JOIN USER_ANSWER ON ONGOING_TEST.ID = USER_ANSWER.ONGOING_TEST_ID " +
                $"WHERE " +
                $"    ONGOING_TEST.ID = @attempt_id " +
                $"ORDER BY " +
                $"    QUESTION.QUESTION_INDEX ASC";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter attempt_id = new("@attempt_id", SqlDbType.Int);
            attempt_id.Value = attempt.AttemptId;
            command.Parameters.Add(attempt_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            List<Question> questions = new();
            List<UserAnswer> userAnswers = new();
            while (reader.Read())
            {
                Question q = questions.FirstOrDefault(q => q.QuestionId == reader.GetFieldValue<int>("QUESTION_ID"), new Question
                {
                    QuestionId = reader.GetFieldValue<int>("QUESTION_ID"),
                    Test = attempt.Test,
                    Text = reader.GetFieldValue<string>("QUESTION"),
                    Index = reader.GetFieldValue<int>("QUESTION_INDEX"),
                    ChechAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("CHECK_ALGORITHM"),
                    QuestionType = (QuestionType)reader.GetFieldValue<int>("TYPE"),
                    AnswerOptions = new()
                });

                if (!questions.Contains(q))
                {
                    questions.Add(q);
                }

                if (reader.GetFieldValue<int?>("ANSWER_OPTION_ID") != null)
                {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                    AnswerOption a = q.AnswerOptions.FirstOrDefault(a => a.AnswerOptionId == reader.GetFieldValue<int>("ANSWER_OPTION_ID"), new AnswerOption
                    {
                        AnswerOptionId = reader.GetFieldValue<int>("ANSWER_OPTION_ID"),
                        Question = q,
                        Text = reader.GetFieldValue<string>("ANSWER_OPTION_ANSWER"),
                        IsCorrect = reader.GetFieldValue<bool>("CORRECT")
                    });
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                    if (reader.GetFieldValue<int>("ANSWER_OPTION_ID") == reader.GetFieldValue<int>("ANSWER_ID"))
                    {
                        a.Checked = true;
                    }
                    if (!q.AnswerOptions.Contains(a))
                    {
                        q.AnswerOptions.Add(a);
                    }
                }

                if (reader.GetFieldValue<int?>("USER_ANSWER_ID") != null)
                {
                    UserAnswer ua = userAnswers.FirstOrDefault(ua => ua.UserAnswerId == reader.GetFieldValue<int>("USER_ANSWER_ID"), new UserAnswer
                    {
                        UserAnswerId = reader.GetFieldValue<int>("USER_ANSWER_ID"),
                        Question = q,
                        Answer = reader.GetFieldValue<string>("USER_ANSWER_ANSWER")
                    });
                    if (ua.Answer != null)
                    {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                        q.AnswerOptions.Add(new AnswerOption
                        {
                            Question = q,
                            Text = ua.Answer,
                            Checked = true,
                            AnswerOptionId = -1
                        });
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                    }
                    if (!userAnswers.Contains(ua))
                    {
                        userAnswers.Add(ua);
                    }
                }
            }
            return questions;
        }
    }
}
