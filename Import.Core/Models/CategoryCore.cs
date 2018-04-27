using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Категория
    /// </summary>
    public class CategoryCore
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute("ID")]
        public Guid Id { get; set; }
    }
}
