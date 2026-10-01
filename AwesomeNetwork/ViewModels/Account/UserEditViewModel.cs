using System;
using System.ComponentModel.DataAnnotations;

namespace AwesomeNetwork.ViewModels.Account
{
    /// <summary>
    /// Для редактирования данных
    /// </summary>
    public class UserEditViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Ссылка на аватар/фото")]
        public string Image { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Display(Name = "О себе")]
        public string About { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
