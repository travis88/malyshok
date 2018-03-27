using cms.dbModel.entity;

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
    }
}
