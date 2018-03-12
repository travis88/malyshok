using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Продукты
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [XmlAttribute("ID")]
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        [XmlAttribute("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Штрих-код
        /// </summary>
        [XmlAttribute("Barcode")]
        public string Barcode { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        [XmlAttribute("Count")]
        public int Count { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        [XmlAttribute("Price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Стандарт
        /// </summary>
        [XmlAttribute("Standart")]
        public string Standart { get; set; }

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
        public CategoryCoreModel[] Categories { get; set; }
    }
}
