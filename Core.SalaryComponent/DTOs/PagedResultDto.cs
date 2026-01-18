namespace Core.SalaryComponent.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}

/// <summary>
/// DTO cho một điều kiện lọc với condition và value
/// </summary>
public class FilterConditionDto
{
    /// <summary>
    /// Điều kiện lọc: contains, notContains, equals, notEquals, startsWith, endsWith, empty, notEmpty
    /// </summary>
    public string Condition { get; set; } = "contains";
    
    /// <summary>
    /// Giá trị để lọc
    /// </summary>
    public string? Value { get; set; }
}

public class PagingRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string? SearchText { get; set; }
    public int? Status { get; set; }
    public List<Guid>? OrganizationIds { get; set; }
    
    // Bộ lọc nâng cao với condition
    public FilterConditionDto? SalaryCompositionCodeFilter { get; set; }
    public FilterConditionDto? SalaryCompositionNameFilter { get; set; }
    public FilterConditionDto? SalaryCompositionTypeFilter { get; set; }
    public FilterConditionDto? SalaryCompositionNatureFilter { get; set; }
    public FilterConditionDto? IsTaxableFilter { get; set; }
    public FilterConditionDto? IsTaxDeductibleFilter { get; set; }
    public FilterConditionDto? QuotaFilter { get; set; }
    public FilterConditionDto? ValueTypeFilter { get; set; }
    public FilterConditionDto? ValueFilter { get; set; }
    public FilterConditionDto? DescriptionFilter { get; set; }
    public FilterConditionDto? ShowOnPayslipFilter { get; set; }
    public FilterConditionDto? SourceFilter { get; set; }
}
