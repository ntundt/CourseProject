namespace CourseProjectServer.Exceptions
{
    public class AttemptNotFoundException : Exception
    {
        private readonly int _id;
        public int Id { get => _id; }
        public AttemptNotFoundException(int id)
        {
            _id = id;
        }
        public override string Message => $"Could not find attempt with id={_id}";
    }
}
