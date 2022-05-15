namespace CourseProjectServer.Exceptions {
    public class TooManyAttemptsException : Exception {
        public TooManyAttemptsException(int testId, int maxAttemptCount) : base("Too many attempts") { }
    }
}