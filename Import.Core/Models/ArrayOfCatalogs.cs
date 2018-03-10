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
    public class ArrayOfCatalogs
    {
        /// <summary>
        /// Список каталогов
        /// </summary>
        [XmlElement(ElementName = "CatalogItem")]
        public CatalogModel[] CatalogList { get; set; }
    }
}
