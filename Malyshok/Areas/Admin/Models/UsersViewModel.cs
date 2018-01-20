using cms.dbModel.entity;

namespace Disly.Areas.Admin.Models
{
    public class UsersViewModel : CoreViewModel
    {
        public UsersList List { get; set; }
        public UsersModel Item { get; set; }

        public Catalog_list[] GroupList { get; set; }
        public PasswordModel Password { get; set; }
    }
}
