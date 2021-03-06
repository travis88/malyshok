﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Продукция
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Штрих-код
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Фото
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Стандарт 
        /// </summary>
        public string Standart { get; set; }
        
        /// <summary>
        /// Строка категорий в которых отображается продукт
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Массив категорий в которых отображается продукт
        /// </summary>
        public Catalog_list[] CatalogPath { get; set; }

        /// <summary>
        /// Категории
        /// </summary>
        public CategoryModel[] Categories { get; set; }

        /// <summary>
        /// Кол-во в корзине
        /// </summary>
        public int inBasket { get; set; }

        /// <summary>
        /// дата последнего заказа
        /// </summary>
        public DateTime prevOrderDate { get; set; }

        /// <summary>
        /// Кол-во в последнем заказе
        /// </summary>
        public int prevOrderCount { get; set; }

        /// <summary>
        /// Сертификаты
        /// </summary>
        public CertificateModel[] Certificates { get; set; }

        /// <summary>
        /// Идентификаторы категорий
        /// </summary>
        public Guid[] CategoriesIds { get; set; }
    }

    /// <summary>
    /// Постраничный список продукции
    /// </summary>
    public class ProductList
    {
        /// <summary>
        /// Список продукции
        /// </summary>
        public ProductModel[] Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Массив продуктов
    /// </summary>
    public class ProductArray
    {
        /// <summary>
        /// Продукты
        /// </summary>
        public ProductModel[] Products { get; set; }
    }
}
