using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая загружаемое фото
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Размер
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Ссылка на файл
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Ссылка на оригинал
        /// </summary>
        public string Source { get; set; }
    }
}
