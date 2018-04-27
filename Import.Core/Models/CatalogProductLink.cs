using System;

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
        public Guid CatalogId { get; set; }

        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public Guid ProductId { get; set; }
    }
}
