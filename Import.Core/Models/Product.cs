using System;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Продукты
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute("ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("title")]
        public string Title { get; set; }

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
        
        /// <summary>
        /// Дата
        /// </summary>
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Список штрих-кодов
        /// </summary>
        [XmlArray(ElementName = "BarcodeList")]
        [XmlArrayItem("Barcode")]
        public Barcode[] BarcodeList { get; set; }

        /// <summary>
        /// Список цен
        /// </summary>
        [XmlArray(ElementName = "PriceList")]
        [XmlArrayItem("Price")]
        public Price[] PriceList { get; set; }

        /// <summary>
        /// Список изображений
        /// </summary>
        [XmlArray(ElementName = "ImageList")]
        [XmlArrayItem("Image")]
        public Image[] ImageList { get; set; }

        /// <summary>
        /// Сертификаты
        /// </summary>
        [XmlArray(ElementName = "Certificates")]
        [XmlArrayItem("Certificate")]
        public Certificate[] Certificates { get; set; }

        /// <summary>
        /// Категории товара
        /// </summary>
        [XmlArray(ElementName = "Categories")]
        [XmlArrayItem("Category")]
        public CategoryCore[] Categories { get; set; }
    }
}
