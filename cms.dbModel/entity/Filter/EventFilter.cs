using System;
using System.Web;

public class EventFilter : FilterParams
{
    /// <summary>
    /// Должен передаваться либо MaterialId либо EventId
    /// </summary>
    public Guid? RelId { get; set; }
    /// <summary>
    /// определяет в какой таблице связей искать связь с организацией
    /// </summary>
    public ContentType RelType { get; set; }
}
