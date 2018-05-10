using cms.dbModel.entity;
using System;
using System.Xml.Serialization;

namespace Disly.Models
{
    [XmlRoot("Order")]
    public class OrdersXMLModel
    {
        /// <summary>
        /// Номер заказа
        /// </summary>
        [XmlAttribute("Num")]
        public int Num { get; set; }
        /// <summary>
        /// Дата
        /// </summary>
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }
        /// <summary>
        /// Имя получателя
        /// </summary>
        [XmlAttribute("UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// Название организации
        /// </summary>
        [XmlAttribute("Organization")]
        public string Organization { get; set; }
        /// <summary>
        /// Контактный Email
        /// </summary>
        [XmlAttribute("")]
        public string Email { get; set; }
        /// <summary>
        /// Контактный телефон
        /// </summary>
        [XmlAttribute("")]
        public string Phone { get; set; }
        /// <summary>
        /// Адрес доставки
        /// </summary>
        [XmlAttribute("")]
        public string Address { get; set; }
        /// <summary>
        /// Комментарий пользователя
        /// </summary>
        [XmlAttribute("")]
        public string Comment { get; set; }
        /// <summary>
        /// Детали 
        /// </summary>
        [XmlArrayItem("Detail", typeof(XMLOrderDetails))]
        [XmlArray("Details")]
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
        [XmlAttribute("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        [XmlAttribute("Count")]
        public int Count { get; set; }
    }
}
