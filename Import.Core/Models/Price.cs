using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Цена
    /// </summary>
    public class Price
    {
        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("name")]
        public string Title { get; set; }

        /// <summary>
        /// Стоимость
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
}
