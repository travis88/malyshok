using System;

namespace cms.dbModel.entity
{
    /// <summary>
    /// Сертификат
    /// </summary>
    public class CertificateModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Гигиеничность
        /// </summary>
        public bool IsHygienic { get; set; }
    }
}
