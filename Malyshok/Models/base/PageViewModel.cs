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
                
        public UsersModel UserInfo { get; set; }
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
}