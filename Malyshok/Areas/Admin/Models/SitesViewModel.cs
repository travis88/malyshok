using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для настроек сайт в представлении
    /// </summary>
    public class SitesViewModel : CoreViewModel
    {
        /// <summary>
        /// Список сайтов
        /// </summary>
        public SitesList List { get; set; }

        /// <summary>
        /// Единичная запись сайта
        /// </summary>
        public SitesModel Item { get; set; }

        /// <summary>
        /// Список типов
        /// </summary>
        public SelectList TypeList { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public SelectList OrgsList { get; set; }

        /// <summary>
        /// Список событий
        /// </summary>
        public SelectList EventsList { get; set; }

        /// <summary>
        /// Список людей
        /// </summary>
        public SelectList MainSpecialistList { get; set; }

        /// <summary>
        /// Список тем
        /// </summary>
        public SelectList Themes { get; set; }
    }
}
