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
