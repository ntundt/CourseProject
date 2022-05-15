namespace CourseProjectServer.Exceptions
{
    public class NotExactlyOneRowException : Exception
    {
        public readonly int RowCount;
        public NotExactlyOneRowException(int count)
        {
            RowCount = count;
        }
        public override string Message => $"Expected one result, got {RowCount}";
    }
}
