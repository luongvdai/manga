using System;
using System.Collections.Generic;
using PRN231_API.Models;

namespace PRN231_API.Dtos
{
    public partial class UserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
