namespace CourseProjectServer.Exceptions
{
    public class TestNotFoundException : Exception
    {
        public readonly int TestId;
        public TestNotFoundException(int testId)
        {
           TestId = testId;
        }
        public override string Message => $"Test with id={TestId} not found";
    }
}
