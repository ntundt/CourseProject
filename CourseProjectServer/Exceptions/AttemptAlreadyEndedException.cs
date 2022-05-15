using CourseProjectServer.Model;

namespace CourseProjectServer.Exceptions
{
    public class AttemptAlreadyEndedException : Exception
    {
        private readonly TestAttempt _attempt;
        public TestAttempt Attempt { get => _attempt; }
        public AttemptAlreadyEndedException(TestAttempt attempt)
        {
            _attempt = attempt;
        }
    }
}
