using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления категорий
    /// </summary>
    public class CategoryViewModel : CoreViewModel
    {
        /// <summary>
        /// Единичная запись
        /// </summary>
        public CategoryModel Item { get; set; }

        /// <summary>
        /// Список категорий
        /// </summary>
        public IEnumerable<CategoryModel> List { get; set; }

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public IEnumerable<BreadCrumbCategory> BreadCrumbs { get; set; }
    }
}