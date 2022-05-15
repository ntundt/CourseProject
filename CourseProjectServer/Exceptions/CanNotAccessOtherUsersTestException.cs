namespace CourseProjectServer.Exceptions
{
    public class CanNotAccessOtherUsersTestException : Exception
    {
        public override string Message => $"Can not access other user's test";
    }
}
