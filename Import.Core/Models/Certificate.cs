﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Import.Core.Models
{
    /// <summary>
    /// Сертификат
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Идентификатор продукции
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Флаг гигиеничности
        /// </summary>
        [XmlAttribute("Hygienic")]
        public bool IsHygienic { get; set; }
    }
}