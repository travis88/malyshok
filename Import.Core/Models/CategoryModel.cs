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
    public class CategoryModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute("Id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("Title")]
        public string Title { get; set; }
    }
}
