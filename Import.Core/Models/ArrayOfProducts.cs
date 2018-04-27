using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Массив продукции
    /// </summary>
    public class ArrayOfProducts
    {
        /// <summary>
        /// Продукты
        /// </summary>
        [XmlElement(ElementName = "Product")]
        public Product[] Products { get; set; }
    }
}
