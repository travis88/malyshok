using System;
using System.Linq;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Параметры для получения файлов импорта
    /// </summary>
    public class ReceiverParamsHelper
    {
        /// <summary>
        /// Время начала
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Директория с файлами
        /// </summary>
        public string DirName { get; set; }

        /// <summary>
        /// Разрешённые форматы изображений
        /// </summary>
        public string[] AllowedPicTypes { get; set; }

        /// <summary>
        /// Директория для сохранения изображений
        /// </summary>
        public string SaveDirName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReceiverParamsHelper()
        {
            StartTime = System.Configuration.ConfigurationManager.AppSettings["Import.StartTime"];
            DirName = System.Configuration.ConfigurationManager.AppSettings["Import.DirName"];
            AllowedPicTypes = System.Configuration.ConfigurationManager.AppSettings["AllowedImageTypes"]
                .Split(',').Where(w => !String.IsNullOrWhiteSpace(w)).ToArray();
            SaveDirName = System.Configuration.ConfigurationManager.AppSettings["Import.SaveDirName"];
        }
    }
}
