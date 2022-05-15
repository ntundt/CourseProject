namespace CourseProjectServer.Exceptions
{
    public class WrongAnswerTypeException : Exception
    {
        private readonly Type _expectedType;
        public Type ExpectedType { get => _expectedType; }

        private readonly Type _actualType;
        public Type ActualType { get => _actualType; }

        public WrongAnswerTypeException(Type actualType, Type expectedType)
        {
            _actualType = actualType;
            _expectedType = expectedType;
        }

        public override string Message => $"Expected type {_expectedType}, got {_actualType}";
    }
}
