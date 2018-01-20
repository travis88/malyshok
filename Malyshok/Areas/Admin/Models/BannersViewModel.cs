using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая баннеры и секции в представление
    /// </summary>
    public class BannersViewModel : CoreViewModel
    {
        /// <summary>
        /// Отдельный баннер
        /// </summary>
        public BannersModel Item { get; set; }

        /// <summary>
        /// Одельная секция баннеров
        /// </summary>
        public BannersSectionModel SectionItem { get; set; }

        /// <summary>
        /// Секции баннеров
        /// </summary>
        public BannersSectionModel[] Sections { get; set; }
    }
}
