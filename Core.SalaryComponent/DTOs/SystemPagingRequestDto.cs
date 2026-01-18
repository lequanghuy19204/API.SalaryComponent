namespace Core.SalaryComponent.DTOs;

/// <summary>
/// DTO yêu cầu phân trang cho thành phần lương hệ thống với bộ lọc nâng cao
/// </summary>
public class SystemPagingRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public string? SearchText { get; set; }
    public string? Type { get; set; }
    
    // Bộ lọc nâng cao với condition
    public FilterConditionDto? SalaryCompositionSystemCodeFilter { get; set; }
    public FilterConditionDto? SalaryCompositionSystemNameFilter { get; set; }
    public FilterConditionDto? SalaryCompositionTypeFilter { get; set; }
    public FilterConditionDto? SalaryCompositionNatureFilter { get; set; }
    public FilterConditionDto? IsTaxableFilter { get; set; }
    public FilterConditionDto? IsTaxDeductibleFilter { get; set; }
    public FilterConditionDto? QuotaFilter { get; set; }
    public FilterConditionDto? ValueTypeFilter { get; set; }
    public FilterConditionDto? ValueFilter { get; set; }
    public FilterConditionDto? DescriptionFilter { get; set; }
    public FilterConditionDto? ShowOnPayslipFilter { get; set; }
}
