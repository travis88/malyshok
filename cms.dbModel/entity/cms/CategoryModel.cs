using System;
using System.Collections.Generic;

namespace cms.dbModel.entity
{
    public class CategoryTree {
        public CategoryModel[] Tree { get; set; }
        public int CountItems { get; set; }
    }

    /// <summary>
    /// Категория товара
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Псевдоним
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Путь к корню
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Номер уровня вложенности
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? Parent { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }
                
        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Кол-во дочерних эл-тов
        /// </summary>
        public int CountChildren { get; set; }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        public CategoryModel[] Children { get; set; }
    }
    
    /// <summary>
    /// Хлебные крошки
    /// </summary>
    public class BreadCrumbCategory
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
        /// Родитель
        /// </summary>
        public Guid? Parent { get; set; }
    }

    /// <summary>
    /// Категории для фильтра продукций
    /// </summary>
    public class CategoryFilterModel
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
        /// Дочерние категории
        /// </summary>
        public IEnumerable<CategoryFilterModel> Childrens { get; set; }
    }
}
