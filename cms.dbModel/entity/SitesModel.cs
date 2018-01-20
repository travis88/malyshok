using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая список сайтов и пейджер
    /// </summary>
    public class SitesList
    {
        /// <summary>
        /// Список сайтов 
        /// </summary>
        public SitesModel[] Data;

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager;
    }

    /// <summary>
    /// Модель, описывающая сайт
    /// </summary>
    public class SitesModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Длинное название
        /// </summary>
        public string LongTitle { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        [Required(ErrorMessage = "Поле «Доменное имя» не должно быть пустым.")]
        [RegularExpression(@"^[^-]([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «Доменное имя» может содержать только буквы латинского алфавита и символ - (дефис). Доменное имя не может начинаться с дефиса.")]
        public string Alias { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Идентификатор контента
        /// </summary>
        public Guid ContentId { get; set; }
        /// <summary>
        /// тип контента сайта, например сайт организации
        /// </summary>
        public ContentLinkType ContentType { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Adress { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Факс
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ссылка на сайт
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Рабочее время
        /// </summary>
        public string Worktime { get; set; }

        /// <summary>
        /// Логотип
        /// </summary>
        public Photo Logo { get; set; }

        /// <summary>
        /// Пользовательские скрипты
        /// </summary>
        public string Scripts { get; set; }

        /// <summary>
        /// Список доменов
        /// </summary>
        public Domain[] DomainList { get; set; }

        /// <summary>
        /// Флаг отключенного сайта
        /// </summary>
        public bool SiteOff { get; set; }

        /// <summary>
        /// facebook link
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// vk link
        /// </summary>
        public string Vk { get; set; }

        /// <summary>
        /// instagramm link
        /// </summary>
        public string Instagramm { get; set; }

        /// <summary>
        /// odnoklassniki link
        /// </summary>
        public string Odnoklassniki { get; set; }

        /// <summary>
        /// twitter link
        /// </summary>
        public string Twitter { get; set; }

        /// <summary>
        /// Список дополнительных доменов в виде строки
        /// </summary>
        public string DomainListString { get; set; }

        /// <summary>
        /// Список дополнительных доменов
        /// </summary>
        public IEnumerable<string> DomainListArray { get; set; }

        /// <summary>
        /// Тема
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Изображение под слайдером
        /// </summary>
        public Photo BackGroundImg { get; set; }
    }

    /// <summary>
    /// Сокращённая модель сайта
    /// </summary>
    public class SitesShortModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Флаг отключенного сайта
        /// </summary>
        public bool SiteOff { get; set; }
        /// <summary>
        /// Список доменов
        /// </summary>
        public Domain[] DomainList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Origin { get; set; }
    }

    /// <summary>
    /// Модель, описывающая домен
    /// </summary>
    public class Domain
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set;}

        /// <summary>
        /// Доменное имя
        /// </summary>
        public string DomainName { get; set; }
    }
}