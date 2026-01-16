namespace Core.SalaryComponent.DTOs;

/// <summary>
/// DTO hiển thị thành phần lương hệ thống
/// </summary>
public class SalaryCompositionSystemDto
{
    public Guid SalaryCompositionSystemId { get; set; }
    public string SalaryCompositionSystemCode { get; set; } = string.Empty;
    public string SalaryCompositionSystemName { get; set; } = string.Empty;
    public string SalaryCompositionSystemType { get; set; } = string.Empty;
    public string SalaryCompositionSystemNature { get; set; } = string.Empty;
    public string? SalaryCompositionSystemTaxOption { get; set; }
    public bool SalaryCompositionSystemTaxDeduction { get; set; }
    public string? SalaryCompositionSystemQuota { get; set; }
    public bool SalaryCompositionSystemAllowExceedQuota { get; set; }
    public string SalaryCompositionSystemValueType { get; set; } = string.Empty;
    public string SalaryCompositionSystemValueCalculation { get; set; } = string.Empty;
    public string? SalaryCompositionSystemSumScope { get; set; }
    public string? SalaryCompositionSystemOrgLevel { get; set; }
    public string? SalaryCompositionSystemComponentToSum { get; set; }
    public string? SalaryCompositionSystemValueFormula { get; set; }
    public string? SalaryCompositionSystemDescription { get; set; }
    public string SalaryCompositionSystemShowOnPayslip { get; set; } = "yes";
    public DateTime SalaryCompositionSystemCreatedDate { get; set; }
}
