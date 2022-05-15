namespace CourseProjectServer.Exceptions
{
    public class AccessViolationException : Exception
    {
        public AccessViolationException()
        {
        }
        public override string Message => "User can not access that object";
    }
}
