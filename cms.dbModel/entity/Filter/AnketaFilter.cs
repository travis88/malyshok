using System;
using System.Web;

public class AnketaFilter : FilterParams
{
    /// <summary>
    /// Должен передаваться siteId,
    /// </summary>
    public Guid? RelId { get; set; }
    /// <summary>
    /// определяет в какой таблице связей искать связь с организацией
    /// </summary>
    public ContentType RelType { get; set; }
}
