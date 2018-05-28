using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Список пользователей с пейджером
    /// </summary>
    public class UsersList 
    {
        /// <summary>
        /// Список пользователей
        /// </summary>
        public UsersModel[] Data;

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager;
    }

    /// <summary>
    /// Пользователь
    /// </summary>
    public class UsersModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Соль для пароля
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Хэш для пароля
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string Keyw { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string FIO { get; set; }

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
        /// Дата рождения
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// Фото
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Мобильный телефон
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        public string Contacts { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        [Required]
        public bool Disabled { get; set; }

        /// <summary>
        /// Флаг удалённости
        /// </summary>
        [Required]
        public bool Deleted { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get { return Surname + " " + Name + " " + Patronymic; } }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Уровень доступа группы, к которой он относится
        /// </summary>
        public int Lvl { get; set; }

        /// <summary>
        /// Вконтакте
        /// </summary>
        public string Vk { get; set; }

        /// <summary>
        /// Facebook
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// Количество неудачных попыток входа
        /// </summary>
        public bool isBlocked { get; set; }
        /// <summary>
        /// Дата блокировки пользователя
        /// </summary>
        public DateTime? LockDate { get; set; }
    }
    
    /// <summary>
    /// Пароль
    /// </summary>
    public class PasswordModel
    {
        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Поле Пароль» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Подтверждение пароля имеет не правильный формат")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirm { get; set; }
    }

    /// <summary>
    /// Разрешения для пользователей
    /// </summary>
    public class ResolutionsModel
    {
        /// <summary>
        /// Идентификатор меню
        /// </summary>
        public Guid MenuId { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Чтение
        /// </summary>
        [Display(Name = "Чтение")]
        public bool Read { get; set; }

        /// <summary>
        /// Запись
        /// </summary>
        [Display(Name = "Запись")]
        public bool Write { get; set; }

        /// <summary>
        /// Изменение
        /// </summary>
        [Display(Name = "Изменение")]
        public bool Change { get; set; }

        /// <summary>
        /// Удаление
        /// </summary>
        [Display(Name = "Удаление")]
        public bool Delete { get; set; }

        /// <summary>
        /// Важно
        /// </summary>
        public bool Importent { get; set; }
    }

    /// <summary>
    /// Группа пользователей
    /// </summary>
    public class GroupModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        [Required]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$", ErrorMessage = "Название группы должно быть написано латинскими буквами")]
        [Display(Name = "Название группы")]
        public string Alias { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        [Required]
        [Display(Name = "Отображаемое имя")]
        public string GroupName { get; set; }

        /// <summary>
        /// Разрешения
        /// </summary>
        public ResolutionsModel[] GroupResolutions { get; set; }
    }

    /// <summary>
    /// Тип действия
    /// </summary>
    public enum ClaimType
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        undefined = 0,
        
        /// <summary>
        /// Чтение
        /// </summary>
        read = 1,

        /// <summary>
        /// Запись
        /// </summary>
        write = 2,

        /// <summary>
        /// Изменение
        /// </summary>
        change = 3,

        /// <summary>
        /// Удаление
        /// </summary>
        delete = 4
    }

    /// <summary>
    /// Модель описывающая права группы на раздел сайта
    /// </summary>
    public class GroupClaims
    {
        /// <summary>
        /// Алиас группы
        /// </summary>
        public string GroupAlias { get; set; }

        /// <summary>
        /// Тип раздела сайта, к которому настраивается доступ
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Тип действия
        /// </summary>
        public ClaimType Claim { get; set; }
        
        /// <summary>
        /// разрешено
        /// </summary>
        public bool Checked { get; set; }
    }

    /// <summary>
    /// Действия для пользователя
    /// </summary>
    public class UserClaims
    {
        /// <summary>
        /// Тип раздела сайта, к которому настраивается доступ
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Тип раздела сайта, к которому настраивается доступ
        /// </summary>
        public Guid ContentId { get; set; }
        
        /// <summary>
        /// Тип действия
        /// </summary>
        public ClaimType Claim { get; set; }
        
        /// <summary>
        /// разрешено
        /// </summary>
        public bool Checked { get; set; }
    }

    /// <summary>
    /// Связь пользователя и сайтами
    /// </summary>
    public class UserSiteLinkModel
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Список идентификаторов сайтов
        /// </summary>
        public Guid SitesId { get; set; }

        /// <summary>
        /// Сайты
        /// </summary>
        public SitesShortModel[] Sites { get; set; }
    }
}
