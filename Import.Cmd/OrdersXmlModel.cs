using System;
using System.Xml.Serialization;

namespace Import.Cmd
{
    /// <summary>
    /// Заказ, экспортируемый в xml
    /// </summary>
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
        [XmlAttribute("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Контактный телефон
        /// </summary>
        [XmlAttribute("Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [XmlAttribute("Address")]
        public string Address { get; set; }

        /// <summary>
        /// Комментарий пользователя
        /// </summary>
        [XmlAttribute("Comment")]
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
