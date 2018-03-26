using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Параметры из конфига
    /// </summary>
    public class EmailParamsHelper
    {
        /// <summary>
        /// Почта рассыльщика
        /// </summary>
        public string EmailFromAddress { get; set; }

        /// <summary>
        /// Имя рассыльщика
        /// </summary>
        public string EmailFromName { get; set; }

        /// <summary>
        /// Пароль рассыльщика
        /// </summary>
        public string EmailPassword { get; set; }

        /// <summary>
        /// Хост рассыльщика
        /// </summary>
        public string EmailHost { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int EmailPort { get; set; }

        /// <summary>
        /// Разрешённость SSL
        /// </summary>
        public bool EmailEnableSsl { get; set; }

        /// <summary>
        /// Список получателей
        /// </summary>
        public string[] EmailTo { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public EmailParamsHelper()
        {
            EmailFromAddress = System.Configuration.ConfigurationManager.AppSettings["Email.From.Address"];
            EmailFromName = System.Configuration.ConfigurationManager.AppSettings["Email.From.Name"];
            EmailPassword = System.Configuration.ConfigurationManager.AppSettings["Email.Password"];
            EmailHost = System.Configuration.ConfigurationManager.AppSettings["Email.Host"];
            EmailPort = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["Email.Port"]);
            EmailEnableSsl = Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["Email.EnableSSL"]);
            EmailTo = System.Configuration.ConfigurationManager.AppSettings["Email.To"]
                        .Split(';').Where(w => !String.IsNullOrWhiteSpace(w)).Select(s => s).ToArray();
        }
    }
}
