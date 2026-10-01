using AwesomeNetwork.Models.Users;
using System.Collections.Generic;

namespace AwesomeNetwork.ViewModels.Account
{
    /// <summary>
    /// Для поиска пользователей
    /// </summary>
    public class SearchViewModel
    {
        public List<UserWithFriendExt> UserList { get; set; } = new List<UserWithFriendExt>();
    }

    public class UserWithFriendExt
    {
        public User User { get; set; }
        public bool IsFriendWithCurrent { get; set; }
    }
}
