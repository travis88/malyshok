using System;
using System.Collections.Generic;
using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для внешнего представления ошибок
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// http-code
        /// </summary>
        public Int32? HttpCode { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Обратная ссылка
        /// </summary>
        public String BackUrl { get; set; }
    }

}