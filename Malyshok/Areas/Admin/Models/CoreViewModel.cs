using cms.dbModel.entity;
using System.ComponentModel;

namespace Disly.Areas.Admin.Models
{
    public abstract class CoreViewModel
    {
        public string DomainName { get; set; }

        public AccountModel Account { get; set; }
        public SettingsModel Settings { get; set; }
        public ResolutionsModel UserResolution { get; set; }
        public cmsLogModel Log { get; set; }

        public ErrorMessage ErrorInfo { get; set; }

        /// <summary>
        /// Название контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Название актина
        /// </summary>
        public string ActionName { get; set; }
    }

    // Ошибки
    public class ErrorMessage
    {
        public string title { get; set; }
        public string info { get; set; }
        public ErrorMassegeBtn[] buttons { get; set; }
    }
    public class ErrorMassegeBtn
    {
        public string url { get; set; }
        public string text { get; set; }
        [DefaultValue("default")]
        public string style { get; set; }
        public string action { get; set; }
    }

    // Pager (постраничный навигатор)
    public class PagerModel
    {
        public string text { get; set; }
        public string url { get; set; }
        public bool isChecked { get; set; }
    }
}