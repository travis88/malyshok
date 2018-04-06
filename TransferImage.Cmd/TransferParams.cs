using System;
using System.Linq;

namespace TransferImage.Cmd
{
    /// <summary>
    /// Параметры для переноса изображений
    /// </summary>
    public class TransferParams
    {
        /// <summary>
        /// Откуда
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Куда
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Разрешённые расширения
        /// </summary>
        public string[] AllowedPicTypes { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TransferParams()
        {
            From = System.Configuration.ConfigurationManager.AppSettings["From"];
            To = System.Configuration.ConfigurationManager.AppSettings["To"];
            AllowedPicTypes = System.Configuration.ConfigurationManager.AppSettings["AllowedPicTypes"]
                .Split(',').Where(w => !String.IsNullOrWhiteSpace(w)).ToArray();
        }
    }
}
