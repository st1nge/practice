using System;

namespace LibrarySystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } // "Admin" или "User"
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ParentPhone { get; set; }
        public DateTime RegistrationDate { get; set; }

        public User()
        {
            RegistrationDate = DateTime.Now;
            Phone = string.Empty;
            Email = string.Empty;
            Address = string.Empty;
            ParentPhone = string.Empty;
        }
    }
}
