namespace Import.Core.Helpers
{
    /// <summary>
    /// Помощник для сохранения изображений под необходимый формат
    /// </summary>
    public class ImageSaverHelper
    {
        /// <summary>
        /// Название изображения
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Путь для сохранения
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Позиционирование по верхнему краю
        /// </summary>
        public string PositionTop { get; set; }

        /// <summary>
        /// Позиционирование по левому краю
        /// </summary>
        public string PositionLeft { get; set; }

        /// <summary>
        /// Ориентация при обрезке
        /// </summary>
        public string Orientation { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ImageSaverHelper(string fullName, string path, int width,
                                int height, string positionTop, 
                                string positionLeft, string orientation)
        {
            FullName = fullName;
            SavePath = path;
            Width = width;
            Height = height;
            PositionTop = positionLeft;
            PositionLeft = positionLeft;
            Orientation = orientation;
        }
    }
}
