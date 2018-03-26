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
        /// Конструктор
        /// </summary>
        public ReceiverParamsHelper()
        {
            StartTime = System.Configuration.ConfigurationManager.AppSettings["Import.StartTime"];
            DirName = System.Configuration.ConfigurationManager.AppSettings["Import.DirName"];
        }
    }
}
