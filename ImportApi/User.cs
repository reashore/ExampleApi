
namespace ImportApi
{
    public class User
    {
        public User(string firstName, string lastName, string email, string password, bool isActive, string note = "")
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            State = (isActive ? "active" : "blocked");
            Note = note;
        }

        public string FirstName { set; get; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string State { get; private set; }
        public string Note { get; set; }
    }
}
