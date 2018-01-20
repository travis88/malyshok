using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class cmsMenuViewModel : CoreViewModel
    {
        public cmsMenuModel[] List { get; set; }
        public cmsMenuItem Item { get; set; }
        public cmsMenuType[] MenuType { get; set; }
    }
}