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
        public bool Type { get; set; }
        public string OrgName { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «ФИО» не должно быть пустым.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Поле «Адрес» не должно быть пустым.")]
        public string Mail { get; set; }

        public PasswordModel Pass { get; set; }

        public bool Compliance { get; set; }
    }

}
