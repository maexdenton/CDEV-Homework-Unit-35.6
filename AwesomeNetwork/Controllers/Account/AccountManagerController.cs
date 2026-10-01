using AutoMapper;
using AwesomeNetwork.Data;
using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using AwesomeNetwork.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Controllers
{
    [Authorize]
    public class AccountManagerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountManagerController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _context = context;
        }

        // Моя страница / Просмотр профиля
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            // Кого добавил я (Мои друзья)
            var myFriendIds = await _context.Friends
                .Where(f => f.UserId == user.Id)
                .Select(f => f.CurrentFriendId)
                .ToListAsync();

            var friends = await _context.Users
                .Where(u => myFriendIds.Contains(u.Id))
                .ToListAsync();

            // Кто добавил меня, но я их еще не добавил взаимно (Подписчики)
            var followerIds = await _context.Friends
                .Where(f => f.CurrentFriendId == user.Id && !myFriendIds.Contains(f.UserId))
                .Select(f => f.UserId)
                .ToListAsync();

            var followers = await _context.Users
                .Where(u => followerIds.Contains(u.Id))
                .ToListAsync();

            // Все сообщения, где я являюсь отправителем или получателем
            var allMyMessages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m => m.SenderId == user.Id || m.RecipientId == user.Id)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            // Находим уникальных собеседников
            var interlocutorIds = allMyMessages
                .Select(m => m.SenderId == user.Id ? m.RecipientId : m.SenderId)
                .Distinct()
                .ToList();

            var interlocutors = await _context.Users
                .Where(u => interlocutorIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var conversations = interlocutorIds
                .Where(id => interlocutors.ContainsKey(id))
                .Select(id =>
                {
                    var interlocutor = interlocutors[id];
                    var dialogMsgs = allMyMessages.Where(m => m.SenderId == id || m.RecipientId == id).ToList();
                    return new ConversationItemViewModel
                    {
                        Interlocutor = interlocutor,
                        LastMessage = dialogMsgs.FirstOrDefault(),
                        UnreadCount = dialogMsgs.Count(m => m.SenderId == id && m.RecipientId == user.Id && !m.IsRead)
                    };
                }).ToList();

            var model = new UserViewModel
            {
                User = user,
                Friends = friends,
                Followers = followers,
                Conversations = conversations
            };

            return View(model);
        }

        // Если пользователь переходит по ссылке /AccountManager/Login (GET) — отправляем его на главную
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Если уже залогинен — в профиль, иначе — на главную к форме входа
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("MyPage", "AccountManager");
            }
            return RedirectToAction("Index", "Home");
        }

        // Обработка отправки формы входа (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Ищем пользователя по Email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Пытаемся войти
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // Успешный вход -> перенаправляем в личный кабинет
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                }

                ModelState.AddModelError(string.Empty, "Неправильный логин (email) и/или пароль");
            }

            // Если данные невалидны или пароль не подошел, возвращаем главную страницу с ошибкой
            var mainModel = new MainViewModel
            {
                LoginView = model,
                RegisterView = new RegisterViewModel()
            };

            return View("~/Views/Home/Index.cshtml", mainModel);
        }

        // Выход из профиля
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Очищаем аутентификационные куки пользователя
            await _signInManager.SignOutAsync();

            // Перенаправляем на главную страницу (где формы входа и регистрации)
            return RedirectToAction("Index", "Home");
        }

        // Форма редактирования данных
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = _mapper.Map<UserEditViewModel>(user);
            return View(model);
        }

        // Сохранение отредактированных данных
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;
            user.Image = string.IsNullOrWhiteSpace(model.Image) ? "/img/default_avatar.png" : model.Image;
            user.Status = model.Status;
            user.About = model.About;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("MyPage");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Поиск по пользователям
        [HttpGet]
        public async Task<IActionResult> Search(string search)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToAction("Index", "Home");

            // Получаем ID друзей текущего пользователя
            var myFriendIds = await _context.Friends
                .Where(f => f.UserId == currentUser.Id)
                .Select(f => f.CurrentFriendId)
                .ToListAsync();

            // Исключаем текущего пользователя из выдачи
            var query = _context.Users.Where(u => u.Id != currentUser.Id);

            // Если введен поисковый запрос — фильтруем по имени, фамилии или email
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term))
                );
            }

            var users = await query.ToListAsync();

            var model = new SearchViewModel
            {
                UserList = users.Select(u => new UserWithFriendExt
                {
                    User = u,
                    IsFriendWithCurrent = myFriendIds.Contains(u.Id)
                }).ToList()
            };

            return View(model);
        }

        // Добавление в друзья
        [HttpPost]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendUser = await _userManager.FindByIdAsync(friendId);

            if (currentUser != null && friendUser != null)
            {
                var isAlreadyFriend = await _context.Friends.AnyAsync(f =>
                    f.UserId == currentUser.Id && f.CurrentFriendId == friendId);

                if (!isAlreadyFriend)
                {
                    _context.Friends.Add(new Friend
                    {
                        UserId = currentUser.Id,
                        CurrentFriendId = friendId
                    });
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("MyPage");
        }

        // Удаление из друзей
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string friendId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                var friendship = await _context.Friends.FirstOrDefaultAsync(f =>
                    f.UserId == currentUser.Id && f.CurrentFriendId == friendId);

                if (friendship != null)
                {
                    _context.Friends.Remove(friendship);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("MyPage");
        }

        // Вспомогательный метод получения друзей
        private async Task<System.Collections.Generic.List<User>> GetUserFriendsAsync(User user)
        {
            return await _context.Friends
                .Where(f => f.UserId == user.Id)
                .Select(f => f.CurrentFriend)
                .ToListAsync();
        }
    }
}
