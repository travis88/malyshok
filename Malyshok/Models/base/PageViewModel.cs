using System;
using System.Collections.Generic;
using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для внешнего представления сайта
    /// </summary>
    public class PageViewModel
    {
        /// <summary>
        /// Информация по ошибками
        /// </summary>
        public ErrorViewModel ErrorInfo { get; set; }

        /// <summary>
        /// Информация по сайту
        /// </summary>
        public SitesModel SitesInfo { get; set; }
        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public List<Breadcrumbs> Breadcrumbs { get; set; }
        /// <summary>
        /// Группы меню
        /// </summary>
        public SiteMapModel[] SiteMapArray { get; set; }
        /// <summary>
        /// Баннеры
        /// </summary>
        public BannersModel[] BannerArray { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public SiteMapModel CurrentPage { get; set; }
        /// <summary>
        /// дочерние элементы
        /// </summary>
        public SiteMapModel[] Child { get; set; }
        /// <summary>
        /// Табы
        /// </summary>
        public List<PageTabsViewModel> Nav { get; set; }
    }

    /// <summary>
    /// /Модель для пейджера
    /// </summary>
    public class PagerFront
    {
        public string text { get; set; }
        public string url { get; set; }
        public bool isChecked { get; set; }
    }

    /// <summary>
    /// Модель для табов на странице
    /// </summary>
    public class PageTabsViewModel
    {
        /// <summary>
        /// страница
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }

}