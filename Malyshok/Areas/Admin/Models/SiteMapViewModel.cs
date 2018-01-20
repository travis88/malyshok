using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления,
    /// описывающая элементы карты сайта
    /// </summary>
    public class SiteMapViewModel : CoreViewModel
    {
        /// <summary>
        /// Одиночная запись карты сайта
        /// </summary>
        public SiteMapModel Item { get; set; }

        /// <summary>
        /// Дочерние элементы карты сайта
        /// </summary>
        public SiteMapModel[] Childrens { get; set; }

        /// <summary>
        /// Список записей карты сайта
        /// </summary>
        public SiteMapList List { get; set; }

        /// <summary>
        /// Список типов страниц
        /// </summary>
        public SiteMapMenu[] FrontSectionList { get; set; }

        /// <summary>
        /// Список доступных типов меню
        /// </summary>
        public Catalog_list[] MenuTypes { get; set; }

        /// <summary>
        /// Хлебные крошки 
        /// </summary>
        public BreadCrumbSiteMap[] BreadCrumbs { get; set; }
    }
}
