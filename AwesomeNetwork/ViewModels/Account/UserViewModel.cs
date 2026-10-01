using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using System.Collections.Generic;

namespace AwesomeNetwork.ViewModels.Account
{
    public class UserViewModel
    {
        public User User { get; set; }

        // Друзья, которых добавил я
        public List<User> Friends { get; set; } = new List<User>();

        // Пользователи, которые добавили меня (но я их еще нет)
        public List<User> Followers { get; set; } = new List<User>();

        // Все активные диалоги с историей переписки
        public List<ConversationItemViewModel> Conversations { get; set; } = new List<ConversationItemViewModel>();
    }

    public class ConversationItemViewModel
    {
        public User Interlocutor { get; set; } // Собеседник
        public Message LastMessage { get; set; } // Последнее сообщение
        public int UnreadCount { get; set; }     // Количество непрочитанных от него
    }
}
