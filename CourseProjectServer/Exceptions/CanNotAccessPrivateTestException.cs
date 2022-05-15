namespace CourseProjectServer.Exceptions
{
    public class CanNotAccessPrivateTestException : Exception
    {
        public readonly int TestId;
        public CanNotAccessPrivateTestException(int testId)
        {
            TestId = testId;
        }
        public override string Message => $"Can not access test with id={TestId}, because it is private";
    }
}
