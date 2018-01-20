using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace cms.dbModel.entity
{
    public class SettingsModel
    {
        /// <summary>
        /// Id записи
        /// </summary>
        //[Required (ErrorMessage = "Поле «Id записи» не должно быть пустым.")]
        public int Id { get; set; } 
        /// <summary>
        /// Guid записи
        /// </summary>
        //[Required(ErrorMessage = "Поле «Логин» не должно быть пустым.")]

        public Guid Guid { get; set; } // uniqueidentifier
        /// <summary>
        /// Сайт отключен
        /// </summary>
        //[Required(ErrorMessage = "Поле «Сайт отключен» не должно быть пустым.")]
        public bool B_Disabled { get; set; } // bit
        /// <summary>
        /// Наименование организации
        /// </summary>
        [Required(ErrorMessage = "Поле «Наименование организации» не должно быть пустым.")]
        public string Title { get; set; }
        /// <summary>
        /// Общая информация
        /// </summary>
        //[Required(ErrorMessage = "Поле «Общая информация» не должно быть пустым.")]
        public string Info { get; set; } // nvarchar(max)
        /// <summary>
        /// Описание организации
        /// </summary>
        //[Required(ErrorMessage = "Поле «Описание организации» не должно быть пустым.")]
        public string Desc { get; set; } // nvarchar(512)
        /// <summary>
        /// Ключевые слова
        /// </summary>
        //[Required(ErrorMessage = "Поле «Ключевые слова» не должно быть пустым.")]
        public string Keyw { get; set; } // nvarchar(256)
        /// <summary>
        /// Сервер исходящей почты
        /// </summary>
        //[Required(ErrorMessage = "Поле «Сервер исходящей почты» не должно быть пустым.")]
        public string MailServer { get; set; } // nvarchar(128)
        /// <summary>
        /// E-Mail для исходящей почты
        /// </summary>
        //[Required(ErrorMessage = "Поле «E-Mail для исходящей почты» не должно быть пустым.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес почты.")]
        public string MailFrom { get; set; } // nvarchar(128)
        /// <summary>
        /// Название почтового ящика
        /// </summary>
        //[Required(ErrorMessage = "Поле «Название почтового ящика» не должно быть пустым.")]
        public string MailFromAlias { get; set; } // nvarchar(128)
        /// <summary>
        /// Пароль от почтового ящика
        /// </summary>
        //[Required(ErrorMessage = "Поле «Пароль от почтового ящика» не должно быть пустым.")]
        public string MailPass { get; set; } // nvarchar(128)
        /// <summary>
        /// E-Mail адрес администратора
        /// </summary>
        //[Required(ErrorMessage = "Поле «E-Mail адрес администратора» не должно быть пустым.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес почты.")]
        public string MailTo { get; set; } // nvarchar(128)
        /// <summary>
        /// Адрес организации
        /// </summary>
        //[Required(ErrorMessage = "Поле «Адрес организации» не должно быть пустым.")]
        public string Adres { get; set; } // nvarchar(1024)
        /// <summary>
        /// Адрес организации (долгота)
        /// </summary>
        //[Required(ErrorMessage = "Поле «Адрес организации (долгота)» не должно быть пустым.")]
        public double? F_CoordX { get; set; } // float
        /// <summary>
        /// Адрес организации (широта)
        /// </summary>
        //[Required(ErrorMessage = "Поле «Адрес организации (широта)» не должно быть пустым.")]
        public double? F_CoordY { get; set; } // float
        /// <summary>
        /// Номер телефона
        /// </summary>
        //[Required(ErrorMessage = "Поле «Номер телефона» не должно быть пустым.")]
        public string Phone { get; set; } // nvarchar(256)
        /// <summary>
        /// Факс
        /// </summary>
        //[Required(ErrorMessage = "Поле «Факс» не должно быть пустым.")]
        public string Fax { get; set; } // nvarchar(256)
        /// <summary>
        /// Email адрес организации
        /// </summary>
        //[Required(ErrorMessage = "Поле «Email адрес организации» не должно быть пустым.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес почты.")]
        public string Mail { get; set; } // nvarchar(256)
        /// <summary>
        /// Адрес сайта
        /// </summary>
        //[Required(ErrorMessage = "Поле «Адрес сайта» не должно быть пустым.")]
        public string Url { get; set; } // nvarchar(512)
        /// <summary>
        /// Режим работы
        /// </summary>
        //[Required(ErrorMessage = "Поле «Режим работы» не должно быть пустым.")]
        public string WorkMode { get; set; } // nvarchar(128)
        public int? MailPort { get; set; }
        public bool SSL { get; set; }

        /// <summary>
        /// Информация о сайте, на котором находимся
        /// </summary>
        public SitesModel ThisSite { get; set; }
    }
}
