namespace Space.Domain.DTO;

public class MeteoriteQueryParams
{
    public int? YearFrom { get; set; }

    public int? YearTo { get; set; }

    public string? RecClass { get; set; }

    public string? NameContains { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public string? SortField { get; set; } = "Year";
    public string? SortOrder { get; set; } = "Asc";
}
