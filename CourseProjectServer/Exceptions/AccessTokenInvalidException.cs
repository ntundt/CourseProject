namespace CourseProjectServer.Exceptions
{
    public class AccessTokenInvalidException : Exception
    {
        public override string Message => "Access token invalid";
    }
}
