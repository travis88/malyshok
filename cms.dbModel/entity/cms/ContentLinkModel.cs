using System;

namespace cms.dbModel.entity
{
     public class ContentLinkModel //: CoreViewModel
     {
        /// <summary>
        /// Id Объкта, который привязываем (Новости, События и тд) 
        /// </summary>
        public Guid ObjctId { get; set; }
        /// <summary>
        /// Тип Объкта, который привязываем (Новости, События и тд)
        /// </summary>
        public ContentType ObjctType { get; set; }

        /// <summary>
        /// Объект, к которому привязываем или отвязываем
        /// </summary>
        public Guid LinkId { get; set; }

        /// <summary>
        /// Тип объекта, к которому привязываем
        /// </summary>
        public ContentLinkType LinkType { get; set; }

        /// <summary>
        /// Флаг, определяющий привязку контента к объекту
        /// </summary>
        public bool Checked { get; set; }

    }
 }