using System;
using System.Configuration;
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
        /// Дата создания продукций
        /// </summary>
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TransferParams()
        {
            From = ConfigurationManager.AppSettings["From"];
            To = ConfigurationManager.AppSettings["To"];
            AllowedPicTypes = ConfigurationManager.AppSettings["AllowedPicTypes"]
                .Split(',').Where(w => !String.IsNullOrWhiteSpace(w)).ToArray();
            DateTime.TryParseExact(ConfigurationManager.AppSettings["DateCreate"], "dd.MM.yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime dateCreate);
            DateCreate = dateCreate;
        }
    }
}
