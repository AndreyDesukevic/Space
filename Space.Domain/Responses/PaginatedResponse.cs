namespace Space.Domain.Responses;

public class PaginatedResponse <T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
}
