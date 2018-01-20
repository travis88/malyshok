using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая разделы
    /// </summary>
    public class SiteSectionViewModel : CoreViewModel
    {
        /// <summary>
        /// Список разделов сайта
        /// </summary>
        public SiteSectionList List { get; set; }

        /// <summary>
        /// Эл-т разделов сайта
        /// </summary>
        public SiteSectionModel Item { get; set; }

        /// <summary>
        /// Типы сайтов
        /// </summary>
        public SiteMapMenu[] SiteType { get; set; }
    }
}