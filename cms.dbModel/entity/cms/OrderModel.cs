using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// 
    /// </summary>
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
        /// Номер заказа
        /// </summary>
        public int? Num { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Пользователь (зарегистрированный пользователь)
        /// </summary>
        public UsersModel User { get; set; }

        /// <summary>
        /// Имя получателя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Тип покупателя  (false - ЮЛ или ИП / true - ФЛ)
        /// </summary>
        public bool UserType { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Контактный Email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Контактный телефон
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Способ доставки (false - Самовывоз / true - По адресу)
        /// </summary>
        public bool Delivery { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Детали 
        /// </summary>
        public OrderDetails[] Details { get; set; }

        /// <summary>
        /// Комментарий пользователя
        /// </summary>
        public string UserComment { get; set; }

        /// <summary>
        /// Комментарий админа
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Количество наименований продукции
        /// </summary>
        public int ProdCount { get; set; }

        /// <summary>
        /// Общая стоимость
        /// </summary>
        public decimal Total { get; set; }
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
}
