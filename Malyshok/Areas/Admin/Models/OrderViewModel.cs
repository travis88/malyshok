using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Заказы для представления
    /// </summary>
    public class OrderViewModel : CoreViewModel
    {
        /// <summary>
        /// Список заказов
        /// </summary>
        public OrdersList List { get; set; }

        /// <summary>
        /// Заказ
        /// </summary>
        public OrderModel Item { get; set; }
    }
}