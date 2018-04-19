using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для страницы новостей
    /// </summary>
    public class OrdersViewModel : PageViewModel
    {
        public OrdersList List { get; set; }
        public OrderModel Item { get; set; }      
    }
}
