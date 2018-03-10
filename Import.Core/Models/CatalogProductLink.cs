using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Core.Models
{
    /// <summary>
    /// Связь продукта с каталогом
    /// </summary>
    public class CatalogProductLink
    {
        /// <summary>
        /// Идентификатор каталога
        /// </summary>
        public int CatalogId { get; set; }

        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public int ProductId { get; set; }
    }
}
