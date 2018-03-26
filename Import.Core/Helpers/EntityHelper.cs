using System.Collections.Generic;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Сущность для вставки в БД
    /// </summary>
    public class EntityHelper<T>
    {
        /// <summary>
        /// Контекст
        /// </summary>
        public dbModel Db { get; set; }

        /// <summary>
        /// Сущность
        /// </summary>
        public IEnumerable<T> List { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
    }
}
