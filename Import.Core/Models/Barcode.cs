using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Штрих-код
    /// </summary>
    public class Barcode
    {
        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
