using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
