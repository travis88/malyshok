using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import.Core.Helpers
{
    /// <summary>
    /// Помощник для вставки
    /// </summary>
    public class InsertHelper
    {
        /// <summary>
        /// Файловый поток
        /// </summary>
        public FileStream FileStream { get; set; }

        /// <summary>
        /// Контекст
        /// </summary>
        public dbModel Db { get; set; }

        /// <summary>
        /// Перечисляемая сущность
        /// </summary>
        public Entity Entity { get; set; }
    }
}
