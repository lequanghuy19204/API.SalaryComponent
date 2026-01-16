namespace Core.SalaryComponent.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}

public class PagingRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string? SearchText { get; set; }
    public int? Status { get; set; }
    public List<Guid>? OrganizationIds { get; set; }
}
