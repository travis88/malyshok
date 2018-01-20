using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class FiltrModel
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Icon { get; set; }
        public string BtnName { get; set; }
        public string Link { get; set; }
        public string Url { get; set; }
        public Catalog_list[] Items { get; set; }
        public string AccountGroup { get; set; }
        /// <summary>
        /// Флаг, отображающий является ли фильтр редактируемым
        /// </summary>
        public bool ReadOnly { get; set; }
    }
}