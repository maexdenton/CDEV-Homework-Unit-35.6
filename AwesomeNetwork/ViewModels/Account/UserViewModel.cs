using AwesomeNetwork.Models.Users;
using System.Collections.Generic;

namespace AwesomeNetwork.ViewModels.Account
{
    /// <summary>
    /// Для просмотра профиля
    /// </summary>
    public class UserViewModel
    {
        public User User { get; set; }
        public List<User> Friends { get; set; } = new List<User>();
    }
}
