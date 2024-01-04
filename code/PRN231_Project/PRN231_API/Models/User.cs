using System;
using System.Collections.Generic;

namespace PRN231_API.Models
{
    public partial class User
    {
        public User()
        {
            Comics = new HashSet<Comic>();
            ComicsNavigation = new HashSet<Comic>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Comic>? Comics { get; set; }

        public virtual ICollection<Comic>? ComicsNavigation { get; set; }
    }
}
