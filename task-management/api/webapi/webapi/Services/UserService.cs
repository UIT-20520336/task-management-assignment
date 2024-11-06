using webapi.Models;
using webapi.Utilities;

namespace webapi.Services
{
    public class UserService
    {
        private readonly TaskContext _context; 

        public UserService(TaskContext context)
        {
            _context = context;
        }
        public bool VerifyUserPassword(User user, string password)
        {
            return PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }

        public void RegisterUser(string username, string password)
        {
            PasswordHasher.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public User? GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
