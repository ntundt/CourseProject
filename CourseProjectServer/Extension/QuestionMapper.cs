using CourseProjectServer.Model;
using DataTransferObject;
using System.Linq;

namespace CourseProjectServer.Extension
{
    public static class QuestionMapper
    {
        public static void ApplyTo(this PutQuestion questionInfo, Question question)
        {
            if (questionInfo.Index != null) 
            {
                question.Index = questionInfo.Index.Value;
            }
            if (questionInfo.Text != null)
            {
                question.Text = questionInfo.Text;
            }
            if (questionInfo.QuestionType != null)
            {
                question.QuestionType = (QuestionType)questionInfo.QuestionType;
            }
            if (questionInfo.CheckAlgorithm != null)
            {
                question.ChechAlgorithm = (CheckAlgorithm)questionInfo.CheckAlgorithm;
            }
            if (questionInfo.AnswerOptions != null)
            {
                question.AnswerOptions = new();
                foreach (var answerOption in questionInfo.AnswerOptions)
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = answerOption.Text ?? "",
                        IsCorrect = answerOption.Checked ?? false
                    });
                }
            }
        }

        public static List<QuestionInfo> ToQuestionInfo(List<AnswerOption> options, List<UserAnswer> answers)
        {
            List<QuestionInfo> questionInfos = new List<QuestionInfo>();

            var questions = options.Select(x => x.Question).DistinctBy(x => x.QuestionId).ToList();
            var checkedOptions = answers.SelectMany(x => x.AnswerOption ?? new List<AnswerOption>()).ToList();
            foreach (var question in questions)
            {
                questionInfos.Add(new QuestionInfo
                {
                    QuestionId = question.QuestionId,
                    Index = question.Index,
                    QuestionType = question.QuestionType,
                    CheckAlgorithm = question.ChechAlgorithm,
                    Text = question.Text,
                    AnswerOptions = options.Select(x => new AnswerOptionInfo
                    {
                        AnswerId = x.AnswerOptionId,
                        Text = x.Text,
                        Checked = checkedOptions.FirstOrDefault(y => (y ??= new()).AnswerOptionId == x.AnswerOptionId, null) != null
                    }).ToList()
                });
            }

            return questionInfos;
        }

        public static UserAnswer ToUserAnswer(TestAttempt attempt, Question question, PutAnswer answer)
        {
            if (answer.SelectedOption != null)
            {
                return new UserAnswer
                {
                    TestAttempt = attempt,
                    AnswerOption = new List<AnswerOption> 
                    {
                        new AnswerOption
                        {
                            Question = question,
                            AnswerOptionId = answer.SelectedOption.Value
                        }
                    },
                    Question = question
                };
            }

            if (answer.SelectedOptions != null)
            {
                return new UserAnswer
                {
                    TestAttempt = attempt,
                    AnswerOption = answer.SelectedOptions.Select(x => new AnswerOption
                    {
                        Question = question,
                        AnswerOptionId = x
                    }).ToList(),
                    Question = question
                };
            }

            if (answer.Answer != null)
            {
                return new UserAnswer
                {
                    TestAttempt = attempt,
                    Answer = answer.Answer,
                    Question = question
                };
            }

            throw new Exception("Invalid answer");
        }

        public static List<QuestionInfo> ToQuestionInfo(this List<Question> questions)
        {
            return questions.Select(x => new QuestionInfo
            {
                QuestionId = x.QuestionId,
                Index = x.Index,
                QuestionType = x.QuestionType,
                CheckAlgorithm = x.ChechAlgorithm,
                Text = x.Text,
                AnswerOptions = (x.AnswerOptions ??= new()).Where(y => y.Question.QuestionId == x.QuestionId).Select(y => new AnswerOptionInfo
                {
                    AnswerId = y.AnswerOptionId,
                    Checked = y.Checked,
                    Text = y.Text
                }).ToList()
            }).ToList();
        }
    }
}
