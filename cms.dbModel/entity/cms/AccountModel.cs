using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Авторизованный пользователь
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Соль для шифрования пароля
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Хэш для шифрования пароля
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// ранг Группа
        /// </summary>
        public int GroupLvl { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }        

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Есть ли ошибки
        /// </summary>
        public bool CountError { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime? LockDate { get; set; }

        /// <summary>
        /// Запрещён
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Список доменов
        /// </summary>
        public DomainList[] Domains { get; set; }
    }

    /// <summary>
    /// Домен
    /// </summary>
    public class DomainList
    {
        /// <summary>
        /// Сортировка
        /// </summary>
        public int Permit { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Название домена
        /// </summary>
        public string DomainName { get; set; }
    }
}

