using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Изображение
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Главное изображение
        /// </summary>
        [XmlAttribute("main")]
        public bool IsMain { get; set; }
    }
}
