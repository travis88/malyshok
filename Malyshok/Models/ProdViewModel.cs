using cms.dbModel.entity;
using System.Web.Mvc;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы новостей
    /// </summary>
    public class ProdViewModel : PageViewModel
    {
        public ProductList List { get; set; }
        public ProductModel Item { get; set; }

        public CategoryTree Categorys { get; set; }

        public Catalog_list[] sortParams { get; set; }
        public Catalog_list[] availableParams { get; set; }
    }
}
