using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Модель для типовой страницы 
    /// </summary>
    public class TypePageViewModel : PageViewModel
    {         
        public SiteMapModel Item { get; set; }        
        /// <summary>
        /// Прикрепленные документы
        /// </summary>
        public DocumentsModel[] Documents { get; set; }
    }
}
