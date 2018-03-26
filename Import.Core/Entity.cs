namespace Import.Core
{
    /// <summary>
    /// Сущность
    /// </summary>
    public enum Entity
    {
        /// <summary>
        /// Каталоги
        /// </summary>
        Catalogs,
        
        /// <summary>
        /// Товары
        /// </summary>
        Products,

        /// <summary>
        /// Связи каталогов и товаров
        /// </summary>
        CatalogProductLinks,
        
        /// <summary>
        /// Штрих-коды
        /// </summary>
        Barcodes,
        
        /// <summary>
        /// Цены
        /// </summary>
        Prices,
        
        /// <summary>
        /// Изображения
        /// </summary>
        Images,

        /// <summary>
        /// Сертификаты
        /// </summary>
        Certificates
    }
}
