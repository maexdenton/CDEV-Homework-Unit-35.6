using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using System.Collections.Generic;

namespace AwesomeNetwork.ViewModels.Account
{
    public class ChatViewModel
    {
        public User CurrentUser { get; set; }
        public User Recipient { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public string NewMessageText { get; set; }
    }
}
