using cms.dbModel.entity;
using System.Collections.Generic;

namespace Disly.Areas.Admin.Models
{
    /// <summary>
    /// Модель, описывающая новости в представлении
    /// </summary>
    public class DocumentsViewModel : CoreViewModel
    {
        public IEnumerable<DocumentsModel> List { get; set; }
        public DocumentsModel Item { get; set; }


    }
}