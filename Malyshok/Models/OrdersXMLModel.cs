using cms.dbModel.entity;
using System;

namespace Disly.Models
{
    public class OrdersXMLModel
    {
        /// <summary>
        /// Номер заказа
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Имя получателя
        /// </summary>
        public string UserName { get; set; }
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
        /// Адрес доставки
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Комментарий пользователя
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Детали 
        /// </summary>
        public XMLOrderDetails[] Details { get; set; }
    }


    /// <summary>
    /// Детали заказа
    /// </summary>
    public class XMLOrderDetails
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public int Count { get; set; }
    }
}
