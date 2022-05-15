using CourseProjectServer.Extension;
using CourseProjectServer.Model;
using DataTransferObject;
using System.Data;
using System.Data.SqlClient;

namespace CourseProjectServer.Repositories
{
    public class QuestionDao
    {
        private readonly IConfiguration _config;
        public QuestionDao(IConfiguration config)
        {
            _config = config;
        }

        public void Update(TestAttempt attempt, int index, UserAnswer answer)
        {
            /*var queryString =
                $*/


            using SqlConnection connection = new SqlConnection(_config.GetConnectionString("MsSql"));
        }

        public Question GetById(int questionId) {
            var queryString =
                $"SELECT " +
                $"  QUESTION.ID QID, " +
                $"  QUESTION.QUESTION_INDEX, " +
                $"  QUESTION.QUESTION," +
                $"  QUESTION.TYPE, " +
                $"  QUESTION.CHECK_ALGORITHM, " +

                $"  TEST.ID TID, " +
                $"  TEST.DATE_CREATED, " +
                $"  TEST.RESULTS_PUBLIC, " +
                $"  TEST.CAN_NOT_REVIEW_QUESTION, " +
                $"  TEST.ATTEMPTS, " +
                $"  TEST.TIME_LIMIT, " +
                $"  TEST.PUBLIC_UNTIL, " +
                $"  TEST.PRIVATE_UNTIL, " +

                $"  [USER].ID UID, " +
                $"  [USER].NAME, " +
                $"  [USER].DATE_CREATED UDATE_CREATED, " +
                $"  [USER].LOGIN, " +
                $"  [USER].PASSWORD_SHA256, " +

                $"  ANSWER_OPTION.ID AID, " +
                $"  ANSWER_OPTION.ANSWER, " +
                $"  ANSWER_OPTION.CORRECT " +

                $"FROM " +
                $"  QUESTION " +
                $"  LEFT JOIN TEST ON TEST.ID = QUESTION.TEST_ID " +
                $"  LEFT JOIN [USER] ON [USER].ID = TEST.AUTHOR_USER_ID " +
                $"  LEFT JOIN ANSWER_OPTION ON ANSWER_OPTION.QUESTION_ID = QUESTION.ID " +
                
                $"WHERE QUESTION.ID = @question_id ";

            using SqlConnection connection = new SqlConnection(_config.GetConnectionString("MsSql"));
            connection.Open();
            using SqlCommand command = new SqlCommand(queryString, connection);

            SqlParameter questionIdParam = new SqlParameter("@question_id", SqlDbType.Int);
            questionIdParam.Value = questionId;
            command.Parameters.Add(questionIdParam);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            var question = new Question
            {
                QuestionId = reader.GetFieldValue<int>("QID"),
                Test = new Test {
                    TestId = reader.GetFieldValue<int>("TID"),
                    Author = new User {
                        UserId = reader.GetFieldValue<int>("UID"),
                        Name = reader.GetFieldValue<string>("NAME"),
                        CreatedDate = reader.GetFieldValue<DateTime>("UDATE_CREATED"),
                        Login = reader.GetFieldValue<string>("LOGIN"),
                        PasswordHash = reader.GetFieldValue<string>("PASSWORD_SHA256")
                    },
                    CreatedDate = reader.GetFieldValue<DateTime>("DATE_CREATED"),
                    ResultsPublic = reader.GetFieldValue<bool>("RESULTS_PUBLIC"),
                    CanNotReviewQuestion = reader.GetFieldValue<bool>("CAN_NOT_REVIEW_QUESTION"),
                    Attempts = reader.GetFieldValue<int>("ATTEMPTS"),
                    TimeLimit = reader.GetFieldValue<TimeSpan>("TIME_LIMIT"),
                    PublicUntil = reader.GetFieldValue<DateTime>("PUBLIC_UNTIL"),
                    PrivateUntil = reader.GetFieldValue<DateTime>("PRIVATE_UNTIL")
                },
                Index = reader.GetFieldValue<int>("QINDEX"),
                Text = reader.GetFieldValue<string>("QQUESTION"),
                QuestionType = reader.GetFieldValue<QuestionType>("QTYPE"),
                ChechAlgorithm = reader.GetFieldValue<CheckAlgorithm>("QALGORITHM"),
                AnswerOptions = new List<AnswerOption>()
            };

            do {
                question.AnswerOptions.Add(new AnswerOption {
                    AnswerOptionId = reader.GetFieldValue<int>("AID"),
                    Text = reader.GetFieldValue<string>("ANSWER"),
                    IsCorrect = reader.GetFieldValue<bool>("CORRECT")
                });
            } while (reader.Read());

            return question;
        }

        public List<AnswerOption> GetAnswerOptionsByTest()
        {
            throw new NotImplementedException();
        }

        public List<Question> GetQuestionsByTest(Test test)
        {
            _ = test ?? throw new System.ArgumentNullException(nameof(test));

            var queryString =
                $"SELECT " +
                $"  QUESTION.ID QID, " +
                $"  QUESTION.TEST_ID, " +
                $"  QUESTION.RESERVED, " +
                $"  QUESTION.QUESTION_INDEX, " +
                $"  QUESTION.QUESTION, " +
                $"  QUESTION.TYPE, " +
                $"  QUESTION.CHECK_ALGORITHM " +

                $"  ANSWER_OPTION.ID AID, " +
                $"  ANSWER_OPTION.QUESTION_ID, " +
                $"  ANSWER_OPTION.ANSWER, " +
                $"  ANSWER_OPTION.IS_CORRECT " +

                $"FROM " +
                $"  QUESTION " +
                $"  LEFT JOIN ANSWER_OPTION ON QUESTION.ID = ANSWER_OPTION.QUESTION_ID " +
                $"WHERE " +
                $"  QUESTION.TEST_ID = @test_id;";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_id = new("@test_id", SqlDbType.Int);
            test_id.Value = test.TestId;
            command.Parameters.Add(test_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            
            List<Question> questions = new();
            while (reader.Read())
            {
                Question question = questions.FirstOrDefault(q => q.QuestionId == reader.GetFieldValue<int>("QID"), new Question {
                    QuestionId = reader.GetFieldValue<int>("QID"),
                    Test = test,
                    Index = reader.GetFieldValue<int>("QUESTION_INDEX"),
                    Text = reader.GetFieldValue<string>("QUESTION"),
                    QuestionType = (QuestionType)reader.GetFieldValue<int>("TYPE"),
                    ChechAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("CHECK_ALGORITHM"),
                    AnswerOptions = new()
                });
                if (!questions.Contains(question))
                {
                    questions.Add(question);
                }

                AnswerOption option = new AnswerOption {
                    AnswerOptionId = reader.GetFieldValue<int>("AID"),
                    Question = question,
                    Text = reader.GetFieldValue<string>("ANSWER"),
                    IsCorrect = reader.GetFieldValue<bool>("IS_CORRECT")
                };
                question.AnswerOptions.Add(option);                
            }

            return questions;
        }

        public List<AnswerOption> GetAnswerOptions(TestAttempt attempt)
        {
            var queryString =
                $"SELECT " +
                $"  ANSWER_OPTION.ID AID, " +
                $"  ANSWER_OPTION.QUESTION_ID AQUESTION_ID, " +
                $"  ANSWER_OPTION.ANSWER AANSWER, " +
                $"  ANSWER_OPTION.CORRECT ACORRECT, " +

                $"  QUESTION.ID QID, " +
                $"  QUESTION.TEST_ID QTEST_ID, " +
                $"  QUESTION.RESERVED QRESERVED, " +
                $"  QUESTION.QUESTION_INDEX QINDEX, " +
                $"  QUESTION.QUESTION QTEXT, " +
                $"  QUESTION.TYPE QTYPE, " +
                $"  QUESTION.CHECK_ALGORITHM QALGORHYTM " +

                $"FROM " +
                $"  QUESTION " +
                $"  JOIN ANSWER_OPTION ON ANSWER_OPTION.QUESTION_ID = QUESTION.ID " +
                $"WHERE " +
                $"  QUESTION.TEST_ID = @test_id;";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_id = new("@test_id", SqlDbType.Int);
            test_id.Value = attempt.Test.TestId;
            command.Parameters.Add(test_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            List<AnswerOption> result = new();

            while (reader.Read())
            {
                result.Add(new AnswerOption
                {
                    AnswerOptionId = reader.GetFieldValue<int>("AID"),
                    Question = new Question
                    { 
                        QuestionId = reader.GetFieldValue<int>("QID"),
                        Test = attempt.Test,
                        Index = reader.GetFieldValue<int>("QINDEX"),
                        Text = reader.GetFieldValue<string>("QTEXT"),
                        QuestionType = (QuestionType)reader.GetFieldValue<int>("QTYPE"),
                        ChechAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("QALGORHYTM")
                    },
                    Text = reader.GetFieldValue<string>("AANSWER"),
                    IsCorrect = reader.GetFieldValue<bool>("ACORRECT")
                });
            }

            return result;
        }

        public Question GetQuestionByIndex(Test test, int index)
        {
            throw new NotImplementedException();
            var queryString =
                $"";
        }

        public List<UserAnswer> GetUserAnswers(TestAttempt attempt, Question question)
        {
            var queryString = 
                $"SELECT " +
                $"  USER_ANSWER.ID UID, " +
                $"  USER_ANSWER.ONGOING_TEST_ID UTEST_ID, " +
                $"  USER_ANSWER.ANSWER_ID UANSWER_ID, " +
                $"  USER_ANSWER.QUESTION_ID UQUESTION_ID, " +
                $"  USER_ANSWER.ANSWER UANSWER, " +
                $"  ANSWER_OPTION.ID AID, " +
                $"  ANSWER_OPTION.QUESTION_ID QID, " +
                $"  ANSWER_OPTION.ANSWER AANSWER, " +
                $"  ANSWER_OPTION.CORRECT ACORRECT " +
                $"FROM " +
                $"  USER_ANSWER " +
                $"  LEFT JOIN ANSWER_OPTION ON ANSWER_OPTION.ID = USER_ANSWER.ANSWER_ID " +
                $"WHERE " +
                $"  USER_ANSWER.ID = @test_attempt_id " +
                $"  AND (ANSWER_OPTION.QUESTION_ID = @question_id OR USER_ANSWER.QUESTION_ID = @question_id) ";
                
            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_attempt_id = new("@test_attempt_id", SqlDbType.Int);
            test_attempt_id.Value = attempt.AttemptId;
            command.Parameters.Add(test_attempt_id);

            SqlParameter question_id = new("@question_id", SqlDbType.Int);
            question_id.Value = question.QuestionId;
            command.Parameters.Add(question_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            List<UserAnswer> result = new();

            while (reader.Read())
            {
                var temp = result.Where(x => x.UserAnswerId == reader.GetFieldValue<int>("UID"));
                if (temp.Count() == 1)
                {
                    temp.First().AnswerOption.Add(new AnswerOption
                    {
                        AnswerOptionId = reader.GetFieldValue<int>("UANSWER_ID"),
                        Question = question,
                        Text = reader.GetFieldValue<string>("UANSWER"),
                        IsCorrect = reader.GetFieldValue<bool>("ACORRECT")
                    });
                    continue;
                }
                result.Add(new UserAnswer
                {
                    UserAnswerId = reader.GetFieldValue<int>("UID"),
                    TestAttempt = attempt,
                    AnswerOption = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            AnswerOptionId = reader.GetFieldValue<int>("UANSWER_ID"),
                            Question = question,
                            Text = reader.GetFieldValue<string>("UANSWER"),
                            IsCorrect = reader.GetFieldValue<bool>("ACORRECT")
                        }
                    },
                    Question = question,
                    Answer = reader.GetFieldValue<string>("UANSWER")
                });
            }
            
            return result;
        }

        public List<UserAnswer> GetUserAnswers(TestAttempt attempt)
        {
            var queryString = 
                $"SELECT " +
                $"  USER_ANSWER.ID UAID, " +
                $"  USER_ANSWER.ONGOING_TEST_ID UTEST_ID, " +
                $"  USER_ANSWER.ANSWER_ID UANSWER_ID, " +
                $"  USER_ANSWER.QUESTION_ID UQUESTION_ID, " +
                $"  USER_ANSWER.ANSWER UANSWER, " +
                $"  ANSWER_OPTION.ID AID, " +
                $"  ANSWER_OPTION.QUESTION_ID AQID, " +
                $"  ANSWER_OPTION.ANSWER AANSWER, " +
                $"  ANSWER_OPTION.CORRECT ACORRECT, " +
                $"  QUESTION.ID QID, " +
                $"  QUESTION.TEST_ID QTEST_ID, " +
                $"  QUESTION.RESERVED QRESERVED, " +
                $"  QUESTION.QUESTION_INDEX QINDEX, " +
                $"  QUESTION.QUESTION QQUESTION, " +
                $"  QUESTION.TYPE QTYPE, " +
                $"  QUESTION.CHECK_ALGORITHM QALGORHYTM " +

                $"FROM " +
                $"  USER_ANSWER " +
                $"  LEFT JOIN ANSWER_OPTION ON ANSWER_OPTION.ID = USER_ANSWER.ANSWER_ID " +
                $"  LEFT JOIN QUESTION ON USER_ANSWER.QUESTION_ID = QUESTION.ID OR ANSWER_OPTION.QUESTION_ID = QUESTION.ID " +
                $"WHERE " +
                $"  USER_ANSWER.ONGOING_TEST_ID = @test_attempt_id ";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_attempt_id = new("@test_attempt_id", SqlDbType.Int);
            test_attempt_id.Value = attempt.AttemptId;
            command.Parameters.Add(test_attempt_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();

            List<UserAnswer> result = new();

            while (reader.Read())
            {
                var temp = result.Where(x => x.Question.QuestionId == reader.GetFieldValue<int>("QID"));
                if (temp.Count() == 1)
                {
                    temp.First().AnswerOption.Add(new AnswerOption
                    {
                        AnswerOptionId = reader.GetFieldValue<int>("AID"),
                        Question = temp.First().Question,
                        Text = reader.GetFieldValue<string>("AANSWER"),
                        IsCorrect = reader.GetFieldValue<bool>("ACORRECT")
                    });
                    continue;
                }
                Question question = new Question
                {
                    QuestionId = reader.GetFieldValue<int>("QID"),
                    Test = attempt.Test,
                    Index = reader.GetFieldValue<int>("QINDEX"),
                    Text = reader.GetFieldValue<string>("QQUESTION"),
                    QuestionType = (QuestionType)reader.GetFieldValue<int>("QTYPE"),
                    ChechAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("QALGORHYTM")
                };
                result.Add(new UserAnswer
                {
                    UserAnswerId = reader.GetFieldValue<int>("UAID"),
                    TestAttempt = attempt,
                    AnswerOption = new List<AnswerOption>
                    {
                        new AnswerOption
                        {
                            AnswerOptionId = reader.GetFieldValue<int>("AID"),
                            Question = question,
                            Text = reader.GetFieldValue<string>("AANSWER"),
                            IsCorrect = reader.GetFieldValue<bool>("ACORRECT")
                        }
                    },
                    Question = question,
                });
            }
            
            return result;
        }

        public Question PostQuestion(Test test)
        {
            var queryString =
                $"EXEC CREATE_QUESTION @test_id";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter test_id = new("@test_id", SqlDbType.Int);
            test_id.Value = test.TestId;
            command.Parameters.Add(test_id);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            return new Question
            {
                QuestionId = reader.GetFieldValue<int>("ID"),
                Test = test,
                Index = reader.GetFieldValue<int>("QUESTION_INDEX"),
                Text = reader.GetFieldValue<string>("QUESTION"),
                QuestionType = (QuestionType)reader.GetFieldValue<int>("TYPE"),
                ChechAlgorithm = (CheckAlgorithm)reader.GetFieldValue<int>("CHECK_ALGORITHM")
            };
        }

        public void UpdateQuestionIndex(Question question)
        {
            var queryString =
                $"EXEC QUESTION_SET_INDEX @question_id, @question_index";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter question_id = new("@question_id", SqlDbType.Int);
            question_id.Value = question.QuestionId;
            command.Parameters.Add(question_id);

            SqlParameter question_index = new("@question_index", SqlDbType.Int);
            question_index.Value = question.Index;
            command.Parameters.Add(question_index);

            command.Prepare();
            command.ExecuteNonQuery();
        }

        public void PutQuestion(Question question)
        {
            var queryString =
                $"EXEC PUT_QUESTION @question_id, @reserved, @question_text, @type, @check_algorithm";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            SqlParameter question_id = new("@question_id", SqlDbType.Int);
            question_id.Value = question.QuestionId;
            command.Parameters.Add(question_id);

            SqlParameter reserved = new("@reserved", SqlDbType.Bit);
            reserved.Value = 0;
            command.Parameters.Add(reserved);

            SqlParameter question_text = new("@question_text", SqlDbType.NVarChar, 4000);
            question_text.Value = question.Text;
            command.Parameters.Add(question_text);

            SqlParameter type = new("@type", SqlDbType.Int);
            type.Value = (int)question.QuestionType;
            command.Parameters.Add(type);

            SqlParameter check_algorithm = new("@check_algorithm", SqlDbType.Int);
            check_algorithm.Value = (int?)question.ChechAlgorithm ?? 0;
            command.Parameters.Add(check_algorithm);

            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            if (reader.GetFieldValue<int>("QUESTION_INDEX") != question.Index)
            {
                this.UpdateQuestionIndex(question);
            }

            this.PutAnswers(question);
        }

        private void PutAnswers(Question question)
        {
            question.AnswerOptions ??= new();

            List<string> values = new();

            List<SqlParameter> parameters = new();
            for (int i = 0; i < question.AnswerOptions.Count; i++)
            {
                values.Add($"(@question_id, @answer_text{i}, @is_correct{i})");
                
                SqlParameter answer_text = new($"@answer_text{i}", SqlDbType.NVarChar, 4000);
                answer_text.Value = question.AnswerOptions[i].Text;
                parameters.Add(answer_text);

                SqlParameter is_correct = new($"@is_correct{i}", SqlDbType.Bit);
                is_correct.Value = question.AnswerOptions[i].IsCorrect;
                parameters.Add(is_correct);
            }

            SqlParameter question_id = new("@question_id", SqlDbType.Int);
            question_id.Value = question.QuestionId;
            parameters.Add(question_id);

            var queryString =
                $"DELETE FROM ANSWER_OPTION WHERE QUESTION_ID = @question_id;" +
                $"INSERT INTO ANSWER_OPTION (QUESTION_ID, ANSWER, CORRECT) VALUES {string.Join(",", values)};";

            using SqlConnection connection = new(_config.GetConnectionString("MsSql"));
            connection.Open();
            SqlCommand command = new(queryString, connection);

            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            command.Prepare();
            command.ExecuteNonQuery();
        }
    }
}
