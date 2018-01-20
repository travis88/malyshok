using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая баннеры
    /// </summary>
    public class BannersModel
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
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Изображение
        /// </summary>
        public Photo Photo { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Кол-во переходов
        /// </summary>
        public int CountClick { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Флаг запрещённости эл-та для отображения
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Флаг запрещённости эл-та для редактирования
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Секция
        /// </summary>
        public Guid? Section { get; set; }

        /// <summary>
        /// Алиас секции
        /// </summary>
        public string SectionAlias { get; set; }
        /// <summary>
        /// Ссылки на другие объекты портала
        /// </summary>
        public ObjectLinks Links { get; set; }
    }

    /// <summary>
    /// Модель, описывающая список баннеров
    /// </summary>
    public class BannersListModel
    {
        /// <summary>
        /// Список баннеров
        /// </summary>
        public BannersModel[] Data { get; set; }

        /// <summary>
        /// Пэйджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    /// <summary>
    /// Модель, описывающая секции баннеров
    /// </summary>
    public class BannersSectionModel
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
        /// Порядок сортировки
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Флаг запрещённости эл-та
        /// </summary>
        public bool Disabled { get; set; }
        
        /// <summary>
        /// Кол-во баннеров для данной секции и домена
        /// </summary>
        public int CountBanners { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Список баннеров
        /// </summary>
        public BannersListModel BannerList { get; set; }
    }
}
