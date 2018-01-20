using System;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая сущность карты сайта
    /// </summary>
    public class SiteMapModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Фронтальная секция
        /// </summary>
        public string FrontSection { get; set; }

        /// <summary>
        /// Путь
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Превью
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string Keyw { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Заблокировано админом, от удаления и редактирования некоторых параметров
        /// </summary>
        public bool Blocked { get; set; }

        /// <summary>
        /// Флаг запрещённости в меню
        /// </summary>
        public bool DisabledMenu { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Кол-во дочерних элементов
        /// </summary>
        public int CountSibling { get; set; }

        /// <summary>
        /// Родительский идентификатор
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Типы меню для элемента карты сайта
        /// </summary>
        public string[] MenuGroups { get; set; }

        /// <summary>
        /// Идентификатор меню для сортировки
        /// </summary>
        public Guid? MenuGr { get; set; }

        /// <summary>
        /// Алиас для группы меню
        /// </summary>
        public string MenuAlias { get; set; }

        /// <summary>
        /// Сортировка внутри меню
        /// </summary>
        public int MenuSort { get; set; }

        /// <summary>
        /// Изображение
        /// </summary>
        public Photo Photo { get; set; }

        /// <summary>
        /// Идентификатор в старой базе
        /// </summary>
        public int? OldId { get; set; }
    }

    /// <summary>
    /// Модель, описывающая список элементов карты сайта
    /// </summary>
    public class SiteMapList
    {
        /// <summary>
        /// Список элементов карты сайта
        /// </summary>
        public SiteMapModel[] Data { get; set; }

        /// <summary>
        /// Пэйджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Хлебные крошки для карты сайта
    /// </summary>
    public class BreadCrumbSiteMap
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
        /// Идентификатор родителя
        /// </summary>
        public Guid? ParentId { get; set; }
    }

    /// <summary>
    /// Элемент меню карты сайта
    /// </summary>
    public class SiteMapMenu
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }
}
