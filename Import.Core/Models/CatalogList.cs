using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Массив каталогов
    /// </summary>
    public class CatalogList
    {
        /// <summary>
        /// Список каталогов
        /// </summary>
        [XmlElement(ElementName = "CatalogItem")]
        public Catalog[] Catalogs { get; set; }
    }
}
