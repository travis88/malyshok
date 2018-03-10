using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Каталог
    /// </summary>
    public class CatalogModel : CategoryCoreModel
    {
        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; }

        /// <summary>
        /// Родительский идентификатор
        /// </summary>
        [XmlAttribute("parentId")]
        public int ParentId { get; set; }
    }
}
