using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Изображение
    /// </summary>
    public class Image
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
        /// Главное изображение
        /// </summary>
        [XmlAttribute("main")]
        public bool IsMain { get; set; }
    }
}
