using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Лог
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Id пользователя, внесшего изменения
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// IP пользователя, внесшего изменения
        /// </summary>
        public string IP { get; set; }
        
        /// <summary>
        /// Сайт
        /// </summary>
        public string Site { get; set; }
        
        /// <summary>
        /// Раздел сайта, в котором произошли изменения
        /// </summary>
        public LogSection Section { get; set; }
        
        /// <summary>
        /// Тип изменений
        /// </summary>
        public LogAction Action { get; set; }
        
        /// <summary>
        /// Id изменяемой записи
        /// </summary>
        public Guid PageId { get; set; }
        
        /// <summary>
        /// Текст, характеризующий изменяемую запись
        /// </summary>
        public string PageName { get; set; }
    }

    /// <summary>
    /// Действие для логирования
    /// </summary>
    public enum LogAction
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        undefined = 0,

        /// <summary>
        /// Создание
        /// </summary>
        insert,

        /// <summary>
        /// Удаление
        /// </summary>
        delete,

        /// <summary>
        /// Изменение
        /// </summary>
        update,

        /// <summary>
        /// Восстановление
        /// </summary>
        recovery,

        /// <summary>
        /// Заблокировано
        /// </summary>
        admin_lock,


        /// <summary>
        /// Авторизация
        /// </summary>
        login,

        /// <summary>
        /// Выход из системы
        /// </summary>
        log_off,

        /// <summary>
        /// Неудачная попытка входа
        /// </summary>
        failed_login,

        /// <summary>
        /// Блокировка аккаунта
        /// </summary>
        account_lockout,

        /// <summary>
        /// Изменение пароля
        /// </summary>
        change_pass,

        /// <summary>
        /// Запрос на востановление пароля
        /// </summary>
        reqest_change_pass,
        
        /// <summary>
        /// Добавление домена
        /// </summary>
        insert_domain,

        /// <summary>
        /// Удаление домена
        /// </summary>
        delete_domain,

        /// <summary>
        /// Создание связи пользователя и сайта
        /// </summary>
        insert_sitelink,

        /// <summary>
        /// Удаление связи пользователя и сайта
        /// </summary>
        delete_sitelink,

        /// <summary>
        /// Изменение прав доступа
        /// </summary>
        change_resolutions,

        /// <summary>
        /// Отправлен ответ
        /// </summary>
        mail_answer,

        /// <summary>
        /// Сообщение прочитано
        /// </summary>
        mail_read,
        
        /// <summary>
        ///  Удаление телефона депртамента
        /// </summary>
        delete_phone_derpart,

        /// <summary>
        /// Добавление телефона департменту
        /// </summary>
        insert_phone_depart
    }

    /// <summary>
    /// Секция для логирования
    /// </summary>
    public enum LogSection
    {
        /// <summary>
        /// Неопределена
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Аккаунт
        /// </summary>
        Account = 1,

        /// <summary>
        /// Баннеры
        /// </summary>
        Banners = 2,

        /// <summary>
        /// структуры CMS
        /// </summary>
        CmsMenu = 3,

        /// <summary>
        /// Организации
        /// </summary>
        Orgs = 4,

        /// <summary>
        /// Пользователи портала
        /// </summary>
        PortalUsers = 5,

        /// <summary>
        /// Карта сайта
        /// </summary>
        SiteMap = 6,

        /// <summary>
        /// Сайты
        /// </summary>
        Sites = 7,

        /// <summary>
        /// Пользователи сайтов
        /// </summary>
        Users = 8,

        /// <summary>
        /// Материалы
        /// </summary>
        Materials = 9,

        /// <summary>
        /// События 
        /// </summary>
        Events = 10,

        /// <summary>
        /// Вакансии
        /// </summary>
        Vacancy = 11,

        /// <summary>
        /// Группа пользователей
        /// </summary>
        UserGroup = 12,

        /// <summary>
        /// Разделы сайта
        /// </summary>
        SiteSection = 13,

        /// <summary>
        /// Голосование
        /// </summary>
        Vote = 14,

        /// <summary>
        /// Фотоальбомы
        /// </summary>
        PhotoAlbums = 15,

        /// <summary>
        /// Фотоальбомы
        /// </summary>
        Anketa = 16
    }

    /// <summary>
    /// Уровни логирования
    /// </summary>
    public enum LogLevelEnum
    {
        /// <summary>
        /// Все
        /// </summary>
        All = 0,

        /// <summary>
        /// Info
        /// </summary>
        Info = 1,

        /// <summary>
        /// Error
        /// </summary>
        Error = 2,

        /// <summary>
        /// Warning
        /// </summary>
        Warn = 3,

        /// <summary>
        /// Debug
        /// </summary>
        Debug = 4,

    }

    public class DislyEventArgs
    {
        public LogLevelEnum EventLevel { get; private set; }
        public String Message { get; private set; }
        public Exception Exception { get; private set; }

        public DislyEventArgs(LogLevelEnum eventLevel, String message, Exception exception = null)
        {
            EventLevel = eventLevel;
            Message = message;
            Exception = exception;
        }

    }
}


