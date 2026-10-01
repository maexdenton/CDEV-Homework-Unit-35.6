using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Models.Users
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Image { get; set; } = "/img/default_avatar.png"; // Фото по умолчанию
        public string Status { get; set; }
        public string About { get; set; }

        public string GetFullName() => $"{FirstName} {LastName}".Trim();
    }
}
