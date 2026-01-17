namespace Core.SalaryComponent.DTOs;

public class SalaryCompositionDto
{
    public Guid SalaryCompositionId { get; set; }
    public string SalaryCompositionCode { get; set; } = string.Empty;
    public string SalaryCompositionName { get; set; } = string.Empty;
    public List<Guid> OrganizationIds { get; set; } = new();
    public string SalaryCompositionType { get; set; } = string.Empty;
    public string SalaryCompositionNature { get; set; } = "income";
    public string? SalaryCompositionTaxOption { get; set; }
    public bool SalaryCompositionTaxDeduction { get; set; }
    public string? SalaryCompositionQuota { get; set; }
    public bool SalaryCompositionAllowExceedQuota { get; set; }
    public string SalaryCompositionValueType { get; set; } = "currency";
    public string SalaryCompositionValueCalculation { get; set; } = "formula";
    public string? SalaryCompositionSumScope { get; set; }
    public string? SalaryCompositionOrgLevel { get; set; }
    public string? SalaryCompositionComponentToSum { get; set; }
    public string? SalaryCompositionValueFormula { get; set; }
    public string? SalaryCompositionDescription { get; set; }
    public string SalaryCompositionShowOnPayslip { get; set; } = "yes";
    public string SalaryCompositionSource { get; set; } = "manual";
    public int SalaryCompositionStatus { get; set; } = 1;
    public string? SalaryCompositionTaxablePart { get; set; }
    public string? SalaryCompositionTaxExemptPart { get; set; }
    public DateTime SalaryCompositionCreatedDate { get; set; }
    public DateTime SalaryCompositionModifiedDate { get; set; }
}
