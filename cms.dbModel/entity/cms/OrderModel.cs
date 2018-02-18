using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public UsersModel User { get; set; }

        /// <summary>
        /// Детали 
        /// </summary>
        public IEnumerable<OrderDetails> Details { get; set; }
    }

    /// <summary>
    /// Детали заказа
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Продукт
        /// </summary>
        public ProductModel Product { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public class OrderStatus
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }

    public class OrdersList
    {
        /// <summary>
        /// Список заказов
        /// </summary>
        public IEnumerable<OrderModel> Orders { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }
}
