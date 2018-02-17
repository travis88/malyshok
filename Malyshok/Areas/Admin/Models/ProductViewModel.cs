using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель продукции в представлении
    /// </summary>
    public class ProductViewModel : CoreViewModel
    {
        /// <summary>
        /// Список продукции с пейджером
        /// </summary>
        public ProductList List { get; set; }

        /// <summary>
        /// Единичная запись продукции
        /// </summary>
        public ProductModel Item { get; set; }

        /// <summary>
        /// Список категорий
        /// </summary>
        public IEnumerable<CategoryModel> Categories { get; set; }

        /// <summary>
        /// Категории для фильтра
        /// </summary>
        public IEnumerable<CategoryFilterModel> CategoryFilters { get; set; }
    }
}