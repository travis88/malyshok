using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class BasketViewModel : PageViewModel
    {         
        public ProductModel[] Items { get; set; } 
    }
}
