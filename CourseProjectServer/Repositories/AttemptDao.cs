using DataTransferObject;
using CourseProjectServer.Exceptions;
using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using System.Data;
using System.Data.SqlClient;

namespace CourseProjectServer.Repositories
{
    public class AttemptDao
    {
        private readonly IConfiguration _config;
        public AttemptDao(IConfiguration config)
        {
            _config = config;
        }

        public TestAttempt AddFor(User user, Test test)
        {
            var queryString =
                $"EXEC START_ONGOING_TEST @test_id, @user_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter testId = new("@test_id", SqlDbType.Int);
            testId.Value = test.TestId;
            command.Parameters.Add(testId);

            SqlParameter userId = new("@user_id", SqlDbType.Int);
            userId.Value = user.UserId;
            command.Parameters.Add(userId);

            command.Prepare();

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            return new TestAttempt
            {
                AttemptId = reader.GetFieldValue<int>("OID"),
                Testee = user,
                Test = test,
                Started = reader.GetFieldValue<DateTime>("OSTARTED"),
                Ended = reader.GetFieldValue<DateTime>("OENDED")
            };
        }

        public Result GetResultsByAttempt(TestAttempt attempt)
        {
            var queryString =
                $"SELECT " +
                $"    QUESTION.ID QUESTION_ID, " +
                $"    QUESTION.QUESTION, " +
                $"    QUESTION.QUESTION_INDEX, " +
                $"    QUESTION.CHECK_ALGORITHM, " +
                $"    QUESTION.TYPE, " +

                $"    ANSWER_OPTION.ID ANSWER_OPTION_ID, " +
                $"    ANSWER_OPTION.ANSWER ANSWER_OPTION_ANSWER, " +
                $"    ANSWER_OPTION.CORRECT, " +

                $"    USER_ANSWER.ID USER_ANSWER_ID, " +
                $"    USER_ANSWER.ANSWER_ID, " +
                $"    USER_ANSWER.ANSWER USER_ANSWER_ANSWER " +
                $"FROM " +
                $"    QUESTION " +
                $"    LEFT JOIN ANSWER_OPTION ON QUESTION.ID = ANSWER_OPTION.QUESTION_ID " +
                $"    LEFT JOIN ONGOING_TEST ON QUESTION.TEST_ID = ONGOING_TEST.TEST_ID " +
                $"    LEFT JOIN USER_ANSWER ON (USER_ANSWER.ANSWER_ID = ANSWER_OPTION.ID OR QUESTION.ID = USER_ANSWER.QUESTION_ID) AND USER_ANSWER.ONGOING_TEST_ID = ONGOING_TEST.ID " +
                $"WHERE " +
                $"    ONGOING_TEST.ID = @attempt_id " +
                $"ORDER BY " +
                $"    QUESTION.QUESTION_INDEX";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter attemptId = new("@attempt_id", SqlDbType.Int);
            attemptId.Value = attempt.AttemptId;
            command.Parameters.Add(attemptId);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            Result result = new Result {
                Attempt = attempt,
                Answers = new List<CorrectAnswer>()
            };

            do
            {
                CorrectAnswer answer = result.Answers.FirstOrDefault(
                    a => a.QuestionId == reader.GetFieldValue<int>("QUESTION_ID"),
                    new CorrectAnswer
                    {
                        QuestionId = reader.GetFieldValue<int>("QUESTION_ID"),
                        Index = reader.GetFieldValue<int>("QUESTION_INDEX"),
                        Text = reader.GetFieldValue<string>("QUESTION"),
                        QuestionType = (QuestionType)reader.GetFieldValue<int>("TYPE"),
                        CheckAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("CHECK_ALGORITHM"),
                        Mark = 0,
                        MaxMark = 0,
                        Options = new List<CorrectAnswerOption>()
                    }
                );
                if (!result.Answers.Contains(answer))
                {
                    result.Answers.Add(answer);
                }

                bool isChecked =
                    (
                        !reader.IsDBNull("ANSWER_OPTION_ID")
                        && reader.GetFieldValue<int>("ANSWER_OPTION_ID") == reader.GetFieldValue<int>("ANSWER_ID")
                    )
                    ||
                    (
                        !reader.IsDBNull("USER_ANSWER_ANSWER")
                        && reader.GetFieldValue<string>("USER_ANSWER_ANSWER").ToLower().Equals(reader.GetFieldValue<string>("ANSWER_OPTION_ANSWER").ToLower())
                    );


                bool isCorrect = reader.GetFieldValue<bool>("CORRECT");

                answer.Options.Add(new CorrectAnswerOption
                {
                    AnswerId = reader.GetFieldValue<int>("ANSWER_OPTION_ID"),
                    Text = reader.GetFieldValue<string>("ANSWER_OPTION_ANSWER"),
                    Checked = isChecked,
                    IsActuallyCorrect = isCorrect
                });

                if (!reader.IsDBNull("USER_ANSWER_ANSWER") && !isChecked)
                {
                    answer.Options.Add(new CorrectAnswerOption { 
                        Text = reader.GetFieldValue<string>("USER_ANSWER_ANSWER"), 
                        Checked = true, 
                        IsActuallyCorrect = false
                    });
                }
            } while (reader.Read());

            result.ComputeMark();

            return result;
        }

        public int GetUsedAttemptCount(int testId, int userId) {
            var queryString =
                $"SELECT " +
                $"    COUNT(*) C " +
                $"FROM " +
                $"    ONGOING_TEST " +
                $"WHERE " +
                $"    ONGOING_TEST.TEST_ID = @test_id " +
                $"    AND ONGOING_TEST.RESPONDENT_USER_ID = @user_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            
            SqlParameter testIdParameter = new("@test_id", SqlDbType.Int);
            testIdParameter.Value = testId;
            command.Parameters.Add(testIdParameter);

            SqlParameter userIdParameter = new("@user_id", SqlDbType.Int);
            userIdParameter.Value = userId;
            command.Parameters.Add(userIdParameter);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            return reader.GetFieldValue<int>("C");
        }

        public TestAttempt GetById(int id)
        {
            var queryString =
                $"SELECT " +
                $"  A.ID UID, " +
                $"  A.NAME UNAME, " +
                $"  A.DATE_CREATED UDATE_CREATED, " +
                $"  A.LOGIN ULOGIN, " +
                $"  A.PASSWORD_SHA256 UPASSWORD_SHA256, " +

                $"  B.ID BUID, " +
                $"  B.NAME BUNAME, " +
                $"  B.DATE_CREATED BUDATE_CREATED, " +
                $"  B.LOGIN BULOGIN, " +
                $"  B.PASSWORD_SHA256 BUPASSWORD_SHA256, " +

                $"  TEST.ID TID, " +
                $"  TEST.AUTHOR_USER_ID TAUTHOR_USER_ID, " +
                $"  TEST.DATE_CREATED TDATE_CREATED, " +
                $"  TEST.RESULTS_PUBLIC TRESULTS_PUBLIC, " +
                $"  TEST.CAN_NOT_REVIEW_QUESTION TCAN_NOT_REVIEW_QUESTION, " +
                $"  TEST.ATTEMPTS TATTEMPTS, " +
                $"  TEST.TIME_LIMIT TTIME_LIMIT, " +
                $"  TEST.PUBLIC_UNTIL TPUBLIC_UNTIL, " +
                $"  TEST.PRIVATE_UNTIL TPRIVATE_UNTIL, " +
                $"  TEST.RESULTS_PUBLIC TRESULTS_PUBLIC, " +

                $"  ONGOING_TEST.ID OID, " +
                $"  ONGOING_TEST.TEST_ID OTEST_ID, " +
                $"  ONGOING_TEST.RESPONDENT_USER_ID ORESPONDENT_USER_ID, " +
                $"  ONGOING_TEST.[STARTED] OSTARTED, " +
                $"  ONGOING_TEST.ENDED OENDED " +
                $"FROM " +
                $"  ONGOING_TEST " +
                $"  JOIN TEST ON TEST.ID = ONGOING_TEST.TEST_ID " +
                $"  JOIN [USER] A ON A.ID = ONGOING_TEST.RESPONDENT_USER_ID " +
                $"  JOIN [USER] B ON B.ID = TEST.AUTHOR_USER_ID " +
                $"WHERE " +
                $"  ONGOING_TEST.ID = @attempt_id; ";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter attempt_id = new("@attempt_id", SqlDbType.Int);
            attempt_id.Value = id;
            command.Parameters.Add(attempt_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                throw new AttemptNotFoundException(id);
            }

            User author = new User
            {
                UserId = reader.GetFieldValue<int>("BUID"),
                Name = reader.GetFieldValue<string>("BUNAME"),
                CreatedDate = reader.GetFieldValue<DateTime>("BUDATE_CREATED"),
                Login = reader.GetFieldValue<string?>("BULOGIN"),
                PasswordHash = reader.GetFieldValue<string?>("BUPASSWORD_SHA256")
            };
            User respondent = new User
            {
                UserId = reader.GetFieldValue<int>("UID"),
                Name = reader.GetFieldValue<string>("UNAME"),
                CreatedDate = reader.GetFieldValue<DateTime>("UDATE_CREATED"),
                Login = reader.GetFieldValue<string?>("ULOGIN"),
                PasswordHash = reader.GetFieldValue<string?>("UPASSWORD_SHA256")
            };

            Test test = new Test
            {
                TestId = reader.GetFieldValue<int>("TID"),
                Author = author,
                CreatedDate = reader.GetFieldValue<DateTime>("TDATE_CREATED"),
                ResultsPublic = reader.GetFieldValue<bool>("TRESULTS_PUBLIC"),
                CanNotReviewQuestion = reader.GetFieldValue<bool>("TCAN_NOT_REVIEW_QUESTION"),
                Attempts = reader.GetFieldValue<int>("TATTEMPTS"),
                TimeLimit = reader.GetFieldValue<TimeSpan>("TTIME_LIMIT"),
                PublicUntil = reader.GetFieldValue<DateTime>("TPUBLIC_UNTIL"),
                PrivateUntil = reader.GetFieldValue<DateTime>("TPRIVATE_UNTIL")
            };

            return new TestAttempt
            {
                AttemptId = reader.GetFieldValue<int>("OID"),
                Test = test,
                Testee = respondent,
                Started = reader.GetFieldValue<DateTime>("OSTARTED"),
                Ended = reader.GetFieldValue<DateTime>("OENDED")
            };
        }

        public void Update(TestAttempt attempt)
        {
            var queryString =
                $"UPDATE " +
                $"  ONGOING_TEST " +
                $"SET " +
                $"  ONGOING_TEST.TEST_ID = @attempt_id, " +
                $"  ONGOING_TEST.RESPONDENT_USER_ID = @respondent_user_id, " +
                $"  ONGOING_TEST.STARTED = @started, " +
                $"  ONGOING_TEST.ENDED = @ended " +
                $"WHERE" +
                $"  ONGOING_TEST.ID = @id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter attempt_id = new("@attempt_id", SqlDbType.Int);
            attempt_id.Value = attempt.Test.TestId;
            command.Parameters.Add(attempt_id);

            SqlParameter respondent_user_id = new("@respondent_user_id", SqlDbType.Int);
            respondent_user_id.Value = attempt.Testee.UserId;
            command.Parameters.Add(respondent_user_id);

            SqlParameter started = new("@started", SqlDbType.DateTime);
            started.Value = attempt.Started;
            command.Parameters.Add(started);

            SqlParameter ended = new("@ended", SqlDbType.DateTime);
            ended.Value = attempt.Ended;
            command.Parameters.Add(ended);

            SqlParameter id = new("@id", SqlDbType.Int);
            id.Value = attempt.AttemptId;
            command.Parameters.Add(id);

            command.Prepare();
            if (command.ExecuteNonQuery() != 1)
            {
                throw new AttemptNotFoundException(attempt.AttemptId);
            }
        }

        public List<TestAttempt> GetByUserId(int userId)
        {
            var queryString =
                $"SELECT " +
                $"  ONGOING_TEST.ID OID, " +
                $"  ONGOING_TEST.TEST_ID OTEST_ID, " +
                $"  ONGOING_TEST.RESPONDENT_USER_ID ORESPONDENT_USER_ID, " +
                $"  ONGOING_TEST.[STARTED] OSTARTED, " +
                $"  ONGOING_TEST.ENDED OENDED, " +

                $"  TEST.ID TID, " +
                $"  TEST.AUTHOR_USER_ID TAUTHOR_USER_ID, " +
                $"  TEST.DATE_CREATED TDATE_CREATED, " +
                $"  TEST.RESULTS_PUBLIC TRESULTS_PUBLIC, " +
                $"  TEST.CAN_NOT_REVIEW_QUESTION TCAN_NOT_REVIEW_QUESTION, " +
                $"  TEST.ATTEMPTS TATTEMPTS, " +
                $"  TEST.TIME_LIMIT TTIME_LIMIT, " +
                $"  TEST.PUBLIC_UNTIL TPUBLIC_UNTIL, " +
                $"  TEST.PRIVATE_UNTIL TPRIVATE_UNTIL, " +
                $"  TEST.RESULTS_PUBLIC TRESULTS_PUBLIC, " +

                $"  [USER].ID UID, " +
                $"  [USER].NAME UNAME, " +
                $"  [USER].DATE_CREATED UDATE_CREATED, " +
                $"  [USER].LOGIN ULOGIN, " +
                $"  [USER].PASSWORD_SHA256 UPASSWORD_SHA256 " +
                
                $"FROM " +
                $"  ONGOING_TEST " +
                $"  JOIN TEST ON TEST.ID = ONGOING_TEST.TEST_ID " +
                $"  JOIN [USER] ON [USER].ID = ONGOING_TEST.RESPONDENT_USER_ID " +
                $"WHERE " +
                $"  ONGOING_TEST.RESPONDENT_USER_ID = @user_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter user_id = new("@user_id", SqlDbType.Int);
            user_id.Value = userId;
            command.Parameters.Add(user_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            reader.Read();

            if (!reader.HasRows)
            {
                return new List<TestAttempt>();
            }
            
            User testee = new User 
            {
                UserId = reader.GetFieldValue<int>("UID"),
                Name = reader.GetFieldValue<string>("UNAME"),
                CreatedDate = reader.GetFieldValue<DateTime>("UDATE_CREATED"),
                Login = reader.GetFieldValue<string>("ULOGIN"),
                PasswordHash = reader.GetFieldValue<string?>("UPASSWORD_SHA256")
            };

            List<TestAttempt> attempts = new();
            do
            {
                TestAttempt attempt = attempts.FirstOrDefault(x => x.AttemptId == reader.GetFieldValue<int>("OID"), new TestAttempt {
                    AttemptId = reader.GetFieldValue<int>("OID"),
                    Test = new Test
                    {
                        TestId = reader.GetFieldValue<int>("OTEST_ID"),
                        Author = new User
                        {
                            UserId = reader.GetFieldValue<int>("TAUTHOR_USER_ID")
                        },
                        CreatedDate = reader.GetFieldValue<DateTime>("TDATE_CREATED"),
                        ResultsPublic = reader.GetFieldValue<bool>("TRESULTS_PUBLIC"),
                        CanNotReviewQuestion = reader.GetFieldValue<bool>("TCAN_NOT_REVIEW_QUESTION"),
                        Attempts = reader.GetFieldValue<int>("TATTEMPTS"),
                        TimeLimit = reader.GetFieldValue<TimeSpan>("TTIME_LIMIT"),
                        PublicUntil = reader.GetFieldValue<DateTime>("TPUBLIC_UNTIL"),
                        PrivateUntil = reader.GetFieldValue<DateTime>("TPRIVATE_UNTIL")
                    },
                    Testee = testee,
                    Started = reader.GetFieldValue<DateTime>("OSTARTED"),
                    Ended = reader.GetFieldValue<DateTime>("OENDED")
                });
                if (!attempts.Contains(attempt))
                {
                    attempts.Add(attempt);
                }
            } while (reader.Read());

            return attempts;
        }
    }
}
