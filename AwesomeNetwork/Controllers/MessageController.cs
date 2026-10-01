using AwesomeNetwork.Data;
using AwesomeNetwork.Hubs;
using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using AwesomeNetwork.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Открытие чата и пометка сообщений как прочитанные
        [HttpGet]
        public async Task<IActionResult> Chat(string recipientId)
        {
            if (string.IsNullOrEmpty(recipientId)) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var recipient = await _userManager.FindByIdAsync(recipientId);

            if (recipient == null || currentUser == null) return NotFound();

            // Помечаем все входящие сообщения от этого пользователя как прочитанные
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == recipientId && m.RecipientId == currentUser.Id && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m => m.IsRead = true);
                await _context.SaveChangesAsync();
            }

            var history = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m => (m.SenderId == currentUser.Id && m.RecipientId == recipientId) ||
                            (m.SenderId == recipientId && m.RecipientId == currentUser.Id))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var model = new ChatViewModel
            {
                CurrentUser = currentUser,
                Recipient = recipient,
                Messages = history
            };

            return View(model);
        }

        // POST: Отправка сообщения и отправка уведомления через SignalR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string recipientId, string text)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrEmpty(recipientId))
            {
                var message = new Message
                {
                    SenderId = currentUser.Id,
                    RecipientId = recipientId,
                    Text = text.Trim(),
                    Timestamp = DateTime.Now,
                    IsRead = false
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Отправляем сигнал получателю в реальном времени
                await _hubContext.Clients.User(recipientId).SendAsync(
                    "ReceiveNotification",
                    currentUser.GetFullName(),
                    message.Text,
                    currentUser.Id,
                    currentUser.Image ?? "/img/default_avatar.png"
                );
            }

            return RedirectToAction("Chat", new { recipientId });
        }
    }
}
