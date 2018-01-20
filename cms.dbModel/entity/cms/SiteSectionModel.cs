using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая список новостей
    /// </summary>
    public class SiteSectionList
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public SiteSectionModel[] Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Раздел сайта
    /// </summary>
    public class SiteSectionModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название раздела
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Алиас который впоследствии будет совпадать с названием контроллера во внешней части
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Ссылка на представление
        /// </summary>
        public string Url { get; set; }
    }
    
}
