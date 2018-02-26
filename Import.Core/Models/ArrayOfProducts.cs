using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ProductModel[] Products { get; set; }
    }
}
