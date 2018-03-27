using cms.dbModel.entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы контактов
    /// </summary>
    public class regModel
    {
        /// <summary>
        /// Тип покупателя  (false - ЮЛ или ИП / true - ФЛ)
        /// </summary>
        public bool UserType { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Имя получателя
        /// </summary>
        [Required(ErrorMessage = "Поле «ФИО» не должно быть пустым.")]
        public string UserName { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Address { get; set; }


        /// <summary>
        /// Контактный телефон
        /// </summary>
        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Phone { get; set; }

        /// <summary>
        /// Контактный Email
        /// </summary>
        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Email { get; set; }

        public PasswordModel Pass { get; set; }

        public bool Compliance { get; set; }
    }

}
