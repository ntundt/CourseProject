using CourseProjectServer.Model;

namespace CourseProjectServer.Exceptions
{
    public class UserDoesNotHavePassword : Exception
    {
        private User _user;
        public User User { get => _user; set => _user = value; }
        public UserDoesNotHavePassword(User user)
        {
            _user = user;
        }
    }
}
