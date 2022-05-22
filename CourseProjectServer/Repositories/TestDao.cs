using CourseProjectServer.Model;
using CourseProjectServer.Extension;
using System.Data.SqlClient;
using System.Data;
using CourseProjectServer.Exceptions;
using DataTransferObject;

namespace CourseProjectServer.Repositories
{
    public class TestDao
    {
        private readonly IConfiguration _config;
        public TestDao(IConfiguration config)
        {
            _config = config;
        }

        public List<Test> GetOwnedTests(User owner)
        {
            var queryString = 
                $"SELECT " +
                $"  \"USER\".ID UID," +
                $"  \"USER\".NAME UNAME," +
                $"  \"USER\".DATE_CREATED UDATE_CREATED," +
                $"  \"USER\".LOGIN ULOGIN," +
                $"  \"USER\".PASSWORD_SHA256 UPASSWORD_SHA256," +
                $"  TEST.ID," +
                $"  TEST.AUTHOR_USER_ID," +
                $"  TEST.DATE_CREATED," +
                $"  TEST.RESULTS_PUBLIC," +
                $"  TEST.CAN_NOT_REVIEW_QUESTION," +
                $"  TEST.ATTEMPTS," +
                $"  TEST.TIME_LIMIT," +
                $"  TEST.PUBLIC_UNTIL," +
                $"  TEST.PRIVATE_UNTIL " +
                $"FROM " +
                $"  \"USER\" JOIN TEST ON \"USER\".ID = TEST.AUTHOR_USER_ID " +
                $"WHERE " +
                $"  \"USER\".ID = @uid";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            SqlParameter uid = new("@uid", SqlDbType.Int);
            uid.Value = owner.UserId;
            command.Parameters.Add(uid);
            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                return new List<Test>();
            }

            User author = new User
            {
                UserId = reader.GetFieldValue<int>("UID"),//reader.GetInt32(0),
                Name = reader.GetFieldValue<string>("UNAME"),////reader.GetString(1),
                CreatedDate = reader.GetFieldValue<DateTime>("UDATE_CREATED"),//(DateTime)reader["\"USER\".DATE_CREATED"],//reader.GetDateTime(2),
                Login = reader.GetFieldValue<string?>("ULOGIN"),//(string)reader["\"USER\".LOGIN"], //IsDBNull(3) ? null : reader.GetString(3),
                PasswordHash = reader.GetFieldValue<string?>("UPASSWORD_SHA256")//(string)reader["\"USER\".PASSWORD_HASH"] //.IsDBNull(4) ? null : reader.GetString(4)
            };

            List<Test> result = new List<Test>();
            do
            {
                result.Add(new Test
                {
                    TestId = reader.GetInt32(5),
                    Author = author,
                    CreatedDate = reader.GetDateTime(7),
                    ResultsPublic = reader.GetBoolean(8),
                    CanNotReviewQuestion = reader.GetBoolean(9),
                    Attempts = reader.GetInt32(10),
                    TimeLimit = reader.GetTimeSpan(11),
                    PublicUntil = reader.GetDateTime(12),
                    PrivateUntil = reader.GetDateTime(13)
                });
            } while (reader.Read());

            return result;
        }

        public void Update(Test test)
        {
            var queryString =
                $"UPDATE " +
                $"  TEST " +
                $"SET " +
                $"  TEST.RESULTS_PUBLIC = @results_public," +
                $"  TEST.CAN_NOT_REVIEW_QUESTION = @can_not_review_question," +
                $"  TEST.ATTEMPTS = @attempts," +
                $"  TEST.TIME_LIMIT = @time_limit," +
                $"  TEST.PUBLIC_UNTIL = @public_until," +
                $"  TEST.PRIVATE_UNTIL = @private_until " +
                $"WHERE TEST.ID = @test_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);
            
            SqlParameter testId = new("@test_id", SqlDbType.Int);
            testId.Value = test.TestId;
            command.Parameters.Add(testId);
            
            SqlParameter resultsPublic = new("@results_public", SqlDbType.Bit);
            resultsPublic.Value = test.ResultsPublic;
            command.Parameters.Add(resultsPublic);
            
            SqlParameter canNotReviewQuestion = new("@can_not_review_question", SqlDbType.Bit);
            canNotReviewQuestion.Value = test.CanNotReviewQuestion;
            command.Parameters.Add(canNotReviewQuestion);
            
            SqlParameter attempts = new("@attempts", SqlDbType.Int);
            attempts.Value = test.Attempts;
            command.Parameters.Add(attempts);
            
            SqlParameter timeLimit = new("@time_limit", SqlDbType.Time, 16);
            timeLimit.Value = test.TimeLimit;
            command.Parameters.Add(timeLimit);
            
            SqlParameter publicUntil = new("@public_until", SqlDbType.DateTime, 16);
            publicUntil.Value = test.PublicUntil;
            command.Parameters.Add(publicUntil);
            
            SqlParameter privateUntil = new("@private_until", SqlDbType.DateTime, 16);
            privateUntil.Value = test.PrivateUntil;
            command.Parameters.Add(privateUntil);

            command.Prepare();

            if (command.ExecuteNonQuery() != 1)
            {
                throw new TestNotFoundException(test.TestId);
            }
        }
        
        public void Add(Test test)
        {
            var queryString =
                $"INSERT INTO TEST (" +
                $"  AUTHOR_USER_ID," +
                $"  RESULTS_PUBLIC," +
                $"  CAN_NOT_REVIEW_QUESTION," +
                $"  ATTEMPTS," +
                $"  TIME_LIMIT," +
                $"  PUBLIC_UNTIL," +
                $"  PRIVATE_UNTIL" +
                $")" +
                $"VALUES (" +
                $"  @author_uid," +
                $"  @results_public," +
                $"  @can_not_review_question," +
                $"  @attempts," +
                $"  @time_limit," +
                $"  @public_until," +
                $"  @private_until" +
                $");" +
                $"SELECT SCOPE_IDENTITY() ID;";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            //command.Parameters.AddWithValue("@author_id", test.Author.UserId);
            SqlParameter authorId = new("@author_uid", SqlDbType.Int);
            authorId.Value = test.Author.UserId;
            command.Parameters.Add(authorId);

            //command.Parameters.AddWithValue("@results_public", test.ResultsPublic);
            SqlParameter resultsPublic = new("@results_public", SqlDbType.Bit);
            resultsPublic.Value = test.ResultsPublic;
            command.Parameters.Add(resultsPublic);

            //command.Parameters.AddWithValue("@can_not_review_question", test.CanNotReviewQuestion);
            SqlParameter canNotReviewQuestion = new("@can_not_review_question", SqlDbType.Bit);
            canNotReviewQuestion.Value = test.CanNotReviewQuestion;
            command.Parameters.Add(canNotReviewQuestion);

            //command.Parameters.AddWithValue("@attempts", test.Attempts);
            SqlParameter attempts = new("@attempts", SqlDbType.Int);
            attempts.Value = test.Attempts;
            command.Parameters.Add(attempts);

            //command.Parameters.AddWithValue("@time_limit", test.TimeLimit);
            SqlParameter timeLimit = new("@time_limit", SqlDbType.Time, 16);
            timeLimit.Value = test.TimeLimit;
            command.Parameters.Add(timeLimit);

            //command.Parameters.AddWithValue("@public_until", test.PublicUntil);
            SqlParameter publicUntil = new("@public_until", SqlDbType.DateTime, 16);
            publicUntil.Value = test.PublicUntil;
            command.Parameters.Add(publicUntil);

            //command.Parameters.AddWithValue("@private_until", test.PrivateUntil);
            SqlParameter privateUntil = new("@private_until", SqlDbType.DateTime, 16);
            privateUntil.Value = test.PrivateUntil;
            command.Parameters.Add(privateUntil);

            command.Prepare();

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            decimal id = reader.GetFieldValue<decimal>("ID");
            test.TestId = Convert.ToInt32(id);
        }

        public Test GetTestById(int id)
        {
            var queryString =
                $"SELECT " +
                $"  \"USER\".ID USER_ID," +
                $"  \"USER\".NAME USER_NAME," +
                $"  \"USER\".DATE_CREATED USER_DATE_CREATED," +
                $"  \"USER\".LOGIN USER_LOGIN," +
                $"  \"USER\".PASSWORD_SHA256 USER_PASSWORD_SHA256," +
                $"  TEST.ID TEST_ID," +
                $"  TEST.DATE_CREATED TEST_DATE_CREATED," +
                $"  TEST.RESULTS_PUBLIC TEST_RESULTS_PUBLIC," +
                $"  TEST.CAN_NOT_REVIEW_QUESTION TEST_CAN_NOT_REVIEW_QUESTION," +
                $"  TEST.ATTEMPTS TEST_ATTEMPTS," +
                $"  TEST.TIME_LIMIT TEST_TIME_LIMIT," +
                $"  TEST.PUBLIC_UNTIL TEST_PUBLIC_UNTIL," +
                $"  TEST.PRIVATE_UNTIL TEST_PRIVATE_UNTIL " +
                $"FROM " +
                $"  \"USER\" JOIN TEST ON \"USER\".ID = TEST.AUTHOR_USER_ID " +
                $"WHERE " +
                $"  TEST.ID = @test_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_id = new("@test_id", SqlDbType.Int);
            test_id.Value = id;
            command.Parameters.Add(test_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (!reader.HasRows)
            {
                throw new TestNotFoundException(id);
            }

            return new Test
            {
                TestId = reader.GetFieldValue<int>("TEST_ID"),
                Author = new User
                {
                    UserId = reader.GetFieldValue<int>("USER_ID"),
                    Name = reader.GetFieldValue<string>("USER_NAME"),
                    CreatedDate = reader.GetFieldValue<DateTime>("USER_DATE_CREATED"),
                    Login = reader.GetFieldValue<string?>("USER_LOGIN"),
                    PasswordHash = reader.GetFieldValue<string?>("USER_PASSWORD_SHA256")
                },
                CreatedDate = reader.GetFieldValue<DateTime>("TEST_DATE_CREATED"),
                ResultsPublic = reader.GetFieldValue<bool>("TEST_RESULTS_PUBLIC"),
                CanNotReviewQuestion = reader.GetFieldValue<bool>("TEST_CAN_NOT_REVIEW_QUESTION"),
                Attempts = reader.GetFieldValue<int>("TEST_ATTEMPTS"),
                TimeLimit = reader.GetFieldValue<TimeSpan>("TEST_TIME_LIMIT"),
                PublicUntil = reader.GetFieldValue<DateTime>("TEST_PUBLIC_UNTIL"),
                PrivateUntil = reader.GetFieldValue<DateTime>("TEST_PRIVATE_UNTIL")
            };
        }

        public List<Result> GetResults(Test test)
        {
            var queryString =
                $"SELECT " +
                $"  ONGOING_TEST.ID ONGOING_TEST_ID, " +
                $"  ONGOING_TEST.TEST_ID, " +
                $"  ONGOING_TEST.RESPONDENT_USER_ID, " +
                $"  ONGOING_TEST.STARTED, " +
                $"  ONGOING_TEST.ENDED, " +

                $"  TEST.ID TEST_ID, " +
                $"  TEST.DATE_CREATED, " +
                $"  TEST.RESULTS_PUBLIC, " +
                $"  TEST.CAN_NOT_REVIEW_QUESTION, " +
                $"  TEST.ATTEMPTS, " +
                $"  TEST.TIME_LIMIT, " +
                $"  TEST.PUBLIC_UNTIL, " +
                $"  TEST.PRIVATE_UNTIL, " +

                $"  \"USER\".ID USER_ID, " +
                $"  \"USER\".NAME, " +
                $"  \"USER\".DATE_CREATED, " +
                $"  \"USER\".LOGIN, " +
                $"  \"USER\".PASSWORD_SHA256, " +

                $"  QUESTION.ID QUESTION_ID, " +
                $"  QUESTION.QUESTION_INDEX, " +
                $"  QUESTION.QUESTION, " +
                $"  QUESTION.TYPE, " +
                $"  QUESTION.CHECK_ALGORITHM, " +

                $"  ANSWER_OPTION.ID ANSWER_OPTION_ID, " +
                $"  ANSWER_OPTION.ANSWER ANSWER_OPTION_ANSWER, " +
                $"  ANSWER_OPTION.CORRECT, " +

                $"  USER_ANSWER.ID USER_ANSWER_ID, " +
                $"  USER_ANSWER.ONGOING_TEST_ID, " +
                $"  USER_ANSWER.ANSWER_ID, " +
                $"  USER_ANSWER.QUESTION_ID, " +
                $"  USER_ANSWER.ANSWER USER_ANSWER_ANSWER " +

                $"FROM " +
                $"  ONGOING_TEST " +
                $"  JOIN TEST ON ONGOING_TEST.TEST_ID = TEST.ID " +
                $"  JOIN \"USER\" ON ONGOING_TEST.RESPONDENT_USER_ID = \"USER\".ID " +
                $"  LEFT JOIN QUESTION ON ONGOING_TEST.TEST_ID = QUESTION.TEST_ID " +
                $"  full outer JOIN ANSWER_OPTION ON QUESTION.ID = ANSWER_OPTION.QUESTION_ID " +
                $"  full outer JOIN USER_ANSWER ON " +

                $"    USER_ANSWER.ONGOING_TEST_ID = ONGOING_TEST.ID " +

                $"    AND( " +
                $"        USER_ANSWER.QUESTION_ID = QUESTION.ID OR USER_ANSWER.ANSWER_ID = ANSWER_OPTION.ID " +
                $"        ) " +
                $"WHERE " +
                $"  ONGOING_TEST.TEST_ID = @test_id " +
                $"ORDER BY " +
                $"  ONGOING_TEST.ID, " +
                $"  QUESTION.QUESTION_INDEX, " +
                $"  USER_ANSWER.ID";


            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_id = new("@test_id", SqlDbType.Int);
            test_id.Value = test.TestId;
            command.Parameters.Add(test_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            List<Result> results = new();
            List<TestAttempt> testAttempts = new();
            List<CorrectAnswer> correctAnswers = new();
            List<User> respondents = new();
            while (reader.Read())
            {
                User respondent;
                try
                {
                    respondent = respondents.First(r => r.UserId == reader.GetFieldValue<int>("USER_ID"));
                }
                catch (InvalidOperationException)
                {
                    respondent = new User
                    {
                        UserId = reader.GetFieldValue<int>("USER_ID"),
                        Name = reader.GetFieldValue<string>("NAME"),
                        CreatedDate = reader.GetFieldValue<DateTime>("DATE_CREATED"),
                        Login = reader.GetFieldValue<string?>("LOGIN"),
                        PasswordHash = reader.GetFieldValue<string?>("PASSWORD_SHA256")
                    };
                    respondents.Add(respondent);
                }

                Result result;
                try
                {
                    result = results.First(r => r.Attempt.AttemptId == reader.GetFieldValue<int>("ONGOING_TEST_ID"));
                }
                catch (InvalidOperationException)
                {
                    result = new Result
                    {
                        Attempt = new TestAttempt
                        {
                            AttemptId = reader.GetFieldValue<int>("ONGOING_TEST_ID"),
                            Test = test,
                            Testee = respondent,
                            Started = reader.GetFieldValue<DateTime>("STARTED"),
                            Ended = reader.GetFieldValue<DateTime>("ENDED")
                        }
                    };
                    results.Add(result);
                    testAttempts.Add(result.Attempt);
                }

                CorrectAnswer correctAnswer;
                try
                {
                    correctAnswer = result.Answers.First(c => c.QuestionId == reader.GetFieldValue<int>("QUESTION_ID"));
                }
                catch (InvalidOperationException)
                {
                    correctAnswer = new CorrectAnswer
                    {
                        QuestionId = reader.GetFieldValue<int>("QUESTION_ID"),
                        Index = reader.GetFieldValue<int>("QUESTION_INDEX"),
                        Text = reader.GetFieldValue<string>("QUESTION"),
                        QuestionType = reader.GetFieldValue<QuestionType>("TYPE"),
                        CheckAlgorithm = reader.GetFieldValue<CheckAlgorithm>("CHECK_ALGORITHM"),
                        Options = new()
                    };
                    correctAnswers.Add(correctAnswer);
                    result.Answers.Add(correctAnswer);
                }

                CorrectAnswerOption option;
                try
                {
                    option = correctAnswer.Options.First(o => o.AnswerId == reader.GetFieldValue<int>("ANSWER_OPTION_ID"));
                }
                catch (InvalidOperationException)
                {
                    option = new CorrectAnswerOption
                    {
                        AnswerId = reader.GetFieldValue<int>("ANSWER_OPTION_ID"),
                        Text = reader.GetFieldValue<string>("ANSWER_OPTION_ANSWER"),
                        IsActuallyCorrect = reader.GetFieldValue<bool>("CORRECT"),
                    };
                    correctAnswer.Options.Add(option);
                }

                option.Checked =
                    (
                        !reader.IsDBNull("ANSWER_OPTION_ID")
                        && reader.GetFieldValue<int>("ANSWER_OPTION_ID") == reader.GetFieldValue<int>("ANSWER_ID")
                    )
                    ||
                    (
                        !reader.IsDBNull("USER_ANSWER_ANSWER")
                        && reader.GetFieldValue<string>("USER_ANSWER_ANSWER").ToLower().Equals(reader.GetFieldValue<string>("ANSWER_OPTION_ANSWER").ToLower())
                    );
            }

            return results;
        }
    }
}
