using System;

/// <summary>
/// Фильтр
/// </summary>
public class FilterParams
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid[] Id { get; set; }

    /// <summary>
    /// Домен
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Страница
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Кол-во эл-тов на странице
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Флаг запрещённости
    /// </summary>
    public bool? Disabled { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Категория
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public Guid Order { get; set; }

    /// <summary>
    /// Дата
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    public DateTime? DateEnd { get; set; }

    /// <summary>
    /// Строка поиска
    /// </summary>
    public string SearchText { get; set; }

    /// <summary>
    /// Язык
    /// </summary>
    public string Lang { get; set; }

    public static T Extend<T>(FilterParams f)
        where T: FilterParams, new()
    {
        return new T()
        {
            Id = f.Id,
            Domain = f.Domain,
            Page = f.Page,
            Size = f.Size,
            Disabled = f.Disabled,
            Type = f.Type,
            Category = f.Category,
            Group = f.Group,
            Date = f.Date,
            DateEnd = f.DateEnd,
            SearchText = f.SearchText,
            Lang = f.Lang
        };
    }

}
