using System;

namespace cms.dbModel.entity
{
    /// <summary>
    ///  сайт организации, события или персоны
    /// </summary>
    public class SiteContentType
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? Id;

        /// <summary>
        /// тип
        /// </summary>
        public string CType;
    }

    /// <summary>
    /// Хлебные крошки
    /// </summary>
    public class Breadcrumbs
    {
        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Модель справочника
    /// Используется для построения фильтров, категорий 
    /// и наполнения полей с выпадающими списками
    /// </summary>
    public class Catalog_list
    {
        /// <summary>
        /// Заголовок записи
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Значение записи (ключ для связи)
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// Иконка (иллюстрация)
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// Ссылка, для применения фильтра
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// Адрес формы для редактирования данной записи
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Выбрано
        /// </summary>
        public string selected { get; set; }
        /// <summary>
        /// Доступно для выбора пользователем
        /// </summary>
        public bool available { get; set; }
    }
}
