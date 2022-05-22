using DataTransferObject;
using CourseProjectServer.Model;

namespace CourseProjectServer.Extension
{
    static class ResultExtension
    {
        public static ResultInfo ToResultInfo(this Result result)
        {
            var resultInfo = new ResultInfo
            {
                Mark = result.Mark,
                Started = ((DateTimeOffset)result.Attempt.Started).ToUnixTimeSeconds(),
                Ended = ((DateTimeOffset)result.Attempt.Ended).ToUnixTimeSeconds(),
                AttemptId = result.Attempt.AttemptId,
                UserId = result.Attempt.Testee.UserId,
                UserName = result.Attempt.Testee.Name,
                TestId = result.Attempt.Test.TestId,
                Answers = result.Answers.Select(a => new CorrectAnswerInfo
                {
                    QuestionId = a.QuestionId,
                    Index = a.Index,
                    Text = a.Text,
                    QuestionType = (int)a.QuestionType,
                    CheckAlgorithm = (int)a.CheckAlgorithm,
                    Mark = a.Mark,
                    MaxMark = a.MaxMark,
                    Options = a.Options.Select(o => new CorrectAnswerOptionInfo
                    {
                        AnswerId = o.AnswerId,
                        Text = o.Text,
                        Checked = o.Checked,
                        IsActuallyCorrect = o.IsActuallyCorrect
                    }).ToList()
                }).ToList()
            };
            return resultInfo;
        }

        public static void ComputeMark(this Result result)
        {
            result.Mark = 0;

            foreach (var answer in result.Answers)
            {
                foreach (var option in answer.Options)
                {
                    if (option.IsActuallyCorrect) 
                    {
                        answer.MaxMark += 1;
                    }
                }
            }

            foreach (var answer in result.Answers)
            {
                bool questionFailed = false;

                foreach (var option in answer.Options) {
                    if (answer.QuestionType == QuestionType.SingleChoise)
                    {
                        if (option.Checked && option.IsActuallyCorrect)
                        {
                            result.Mark += 1 / answer.MaxMark;
                            answer.Mark += 1 / answer.MaxMark;
                        }
                    }
                    else if (answer.QuestionType == QuestionType.MultipleChoise)
                    {
                        if (option.Checked && option.IsActuallyCorrect)
                        {
                            result.Mark += 1 / answer.MaxMark;
                            answer.Mark += 1 / answer.MaxMark;
                        }
                        else if (option.Checked && !option.IsActuallyCorrect && answer.CheckAlgorithm == CheckAlgorithm.Percentage)
                        {
                            result.Mark += 1 / answer.MaxMark;
                            answer.Mark += 1 / answer.MaxMark;
                        }
                        else if (option.Checked && !option.IsActuallyCorrect && answer.CheckAlgorithm == CheckAlgorithm.FullMatch)
                        {
                            questionFailed = true;
                            break;
                        }
                    }
                    else if (answer.QuestionType == QuestionType.StringInput)
                    {
                        if (option.Checked && option.IsActuallyCorrect)
                        {
                            result.Mark += 1 / answer.MaxMark;
                            answer.Mark += 1 / answer.MaxMark;
                        }
                    }
                }

                answer.MaxMark = 1;

                if (questionFailed)
                {
                    result.Mark -= answer.Mark;
                    answer.Mark = 0;
                }
            }

        }
    }
}