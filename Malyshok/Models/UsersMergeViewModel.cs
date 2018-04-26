using cms.dbModel.entity;

namespace Disly.Models
{
    /// <summary>
    /// Слияние пользователей
    /// </summary>
    public class UsersMergeViewModel
    {
        /// <summary>
        /// Пользователи
        /// </summary>
        public UsersModel[] Users { get; set; }
    }
}