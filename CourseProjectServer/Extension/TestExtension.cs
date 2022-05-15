using CourseProjectServer.Model;
using DataTransferObject;

namespace CourseProjectServer.Extension
{
    public static class TestExtension
    {
        public static bool HasTimeLimit(this Test test)
        {
            return !test.TimeLimit.Equals(new TimeSpan(0, 0, 0));
        } 

        public static bool IsPrivate(this Test test)
        {
            if (test.PublicUntil.Equals(new DateTime(0)) && test.PrivateUntil.Equals(new DateTime(0)))
            {
                return true;
            }

            if (!test.PublicUntil.Equals(new DateTime(0)) && !test.PrivateUntil.Equals(new DateTime(0)))
            {
                return test.PublicUntil > DateTime.Now && test.PrivateUntil < DateTime.Now;
            }
            
            if (!test.PublicUntil.Equals(new DateTime(0)))
            {
                return test.PublicUntil > DateTime.Now;
            }

            return test.PrivateUntil < DateTime.Now;
        }

        public static TestInfo ToTestInfo(this Test test)
        {
            return new TestInfo
            {
                Id = test.TestId,
                AuthorId = test.Author.UserId,
                CreatedDate = ((DateTimeOffset)test.CreatedDate).ToUnixTimeSeconds(),
                ResultsPublic = test.ResultsPublic,
                CanNotReviewQuestions = test.CanNotReviewQuestion,
                Attempts = test.Attempts,
                TimeLimit = (long)test.TimeLimit.TotalSeconds,
                PrivateUntil = ((DateTimeOffset)test.PrivateUntil).ToUnixTimeSeconds(),
                PublicUntil = ((DateTimeOffset)test.PublicUntil).ToUnixTimeSeconds()
            };
        }

        public static void ApplyTo(this TestInfoSetter setter, Test test)
        {
            if (setter.ResultsPublic.HasValue)
            {
                test.ResultsPublic = setter.ResultsPublic.Value;
            }
            if (setter.CanNotReviewQuestions.HasValue)
            {
                test.CanNotReviewQuestion = setter.CanNotReviewQuestions.Value;
            }
            if (setter.Attempts.HasValue)
            {
                test.Attempts = setter.Attempts.Value;
            }
            if (setter.TimeLimit.HasValue)
            {
                test.TimeLimit = TimeSpan.FromSeconds(setter.TimeLimit.Value);
            }
            if (setter.PrivateUntil.HasValue)
            {
                test.PrivateUntil = DateTimeOffset.FromUnixTimeSeconds(setter.PrivateUntil.Value).DateTime;
            }
            if (setter.PublicUntil.HasValue)
            {
                test.PublicUntil = DateTimeOffset.FromUnixTimeSeconds(setter.PublicUntil.Value).DateTime;
            }
        }
    }
}
