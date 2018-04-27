using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Сертификат
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Флаг гигиеничности
        /// </summary>
        [XmlAttribute("Hygienic")]
        public bool IsHygienic { get; set; }
    }
}
