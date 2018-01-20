using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Модель, описывающая список новостей
    /// </summary>
    public class PhotoAlbumList
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public PhotoAlbum[] Data { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }
    }

    public class PhotoAlbum
    {
        public Guid Id { get; set; }
        /// <summary>
        /// ссылка на организацию/событие/персону по умолчанию
        /// </summary>
        public Guid ContentLink { get; set; }
        /// <summary>
        /// Тип (организация/событие/персона) по умолчанию
        /// </summary>
        public string ContentLinkType { get; set; }
        public string Title { get; set; }
        public Photo PreviewImage { get; set; }
        public string Text { get; set; }
        public string Path { get; set; }
        public bool Disabled { get; set; }
        public DateTime Date { get; set; }
        public PhotoModel[] Photos { get; set; }
    }
    /// <summary>
    /// Модель, описывающая фотографию
    /// </summary>
    public class PhotoModel
    {        
        public Guid Id { get; set; }
        public Guid AlbumId { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Заголовок» не должно быть пустым.")]
        public string Title { get; set; }
        /// <summary>
        /// Превью фотографии
        /// </summary>
        public Photo PreviewImage { get; set; }
        /// <summary>
        /// Фотография
        /// </summary>
        public Photo PhotoImage { get; set; }
        /// <summary>
        /// Фотография
        /// </summary>
        public string PhotoOriginal { get; set; }
        /// <summary>
        /// Дата
        /// </summary>        
        public DateTime Date { get; set; }
        public int Sort { get; set; }
    }


}
