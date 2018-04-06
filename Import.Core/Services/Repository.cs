using System.Linq;

namespace Import.Core.Services
{
    /// <summary>
    /// Репозиторий для работы с бд
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Строка подключения
        /// </summary>
        private readonly string connection = "cmsdbConnection";

        /// <summary>
        /// Возвращает список товаров
        /// </summary>
        /// <returns></returns>
        public content_products[] GetProducts()
        {
            using (var db = new dbModel(connection))
            {
                return db.content_productss.ToArray();
            }
        }
    }
}
