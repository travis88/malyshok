using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Models
{
    /// <summary>
    /// Модель для главной страницы
    /// </summary>
    public class HomePageViewModel : PageViewModel
    {
        /// <summary>
        /// Список новостей
        /// </summary>
        public List<MaterialFrontModule> Materials { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }
    }
}
