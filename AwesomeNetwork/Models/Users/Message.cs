using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Models.Users
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Статус прочтения
        public bool IsRead { get; set; } = false;

        // Отправитель
        public string SenderId { get; set; }
        public User Sender { get; set; }

        // Получатель
        public string RecipientId { get; set; }
        public User Recipient { get; set; }
    }
}
