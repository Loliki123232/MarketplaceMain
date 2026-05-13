using System;

namespace Marketplace.BusinessLogic.Models
{
    /// <summary>
    /// Представляет пользователя системы
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя пользователя (логин)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegistrationDate { get; set; }
    }
}