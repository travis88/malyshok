using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая новости в представлении
    /// </summary>
    public class PhotoViewModel : CoreViewModel
    {
        /// <summary>
        /// Список фотоальбомов
        /// </summary>
        public PhotoAlbumList List { get; set; }
        public PhotoAlbum Album { get; set; }

        /// <summary>
        /// Конкретная галлерея
        /// </summary>
        public PhotoModel Photo { get; set; }
    }
}