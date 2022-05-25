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
                int correctOptionCount = answer.Options.Count(x => x.IsActuallyCorrect);

                foreach (var option in answer.Options) 
                {
                    if (answer.QuestionType == QuestionType.SingleChoice)
                    {
                        if (option.Checked && option.IsActuallyCorrect)
                        {
                            answer.Mark = 1;
                        } 
                        else if (option.Checked && !option.IsActuallyCorrect)
                        {
                            answer.Mark = 0;
                            break;
                        }
                    }
                    else if (answer.QuestionType == QuestionType.MultipleChoice)
                    {
                        if (answer.CheckAlgorithm == CheckAlgorithm.FullMatch)
                        {
                            answer.Mark = 1;
                            if (option.IsActuallyCorrect && !option.Checked || !option.IsActuallyCorrect && option.Checked)
                            {
                                answer.Mark = 0;
                                break;
                            }
                        }
                        else if (answer.CheckAlgorithm == CheckAlgorithm.PartialMatch)
                        {
                            if (option.Checked && option.IsActuallyCorrect)
                            {
                                answer.Mark += 1f / correctOptionCount;
                            }
                        }
                        else if (answer.CheckAlgorithm == CheckAlgorithm.Percentage)
                        {
                            if (option.IsActuallyCorrect && option.Checked)
                            {
                                answer.Mark += 1f / correctOptionCount;
                            }
                            else if (!option.IsActuallyCorrect && option.Checked)
                            {
                                answer.Mark -= 1f / (answer.Options.Count - correctOptionCount);
                            }
                        }
                    }
                    else if (answer.QuestionType == QuestionType.StringInput)
                    {
                        if (option.Checked && option.IsActuallyCorrect)
                        {
                            answer.Mark = 1;
                        }
                    }
                }

                if (answer.Mark < 0)
                {
                    answer.Mark = 0;
                }

                answer.MaxMark = 1;
                result.MaxMark += 1;
                result.Mark += answer.Mark;
                answer.Mark = Math.Round(answer.Mark, 2);
            }
            result.Mark = Math.Round(result.Mark, 2);
            result.MaxMark = result.Answers.Count;
        }
    }
}