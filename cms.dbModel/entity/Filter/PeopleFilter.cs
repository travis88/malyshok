using System;
using System.Web;

public class PeopleFilter : FilterParams
{
    /// <summary>
    /// Фильтрация по специализациям
    /// </summary>
    public int[] Specialization { get; set; }

    /// <summary>
    /// Фильтрация по организациям
    /// </summary>
    public Guid[] Orgs { get; set; }
}
