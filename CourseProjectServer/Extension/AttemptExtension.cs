using CourseProjectServer.Model;

namespace CourseProjectServer.Extension
{
    public static class AttemptExtension
    {
        public static bool HasEnded(this TestAttempt attempt)
        {
            if (attempt.Started.Equals(attempt.Ended))
            {
                return false;
            }
            if (attempt.Started < DateTime.Now && DateTime.Now < attempt.Ended)
            {
                return false;
            }
            return attempt.Started < attempt.Ended && attempt.Ended < DateTime.Now;
        }
    }
}
