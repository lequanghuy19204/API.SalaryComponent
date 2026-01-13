namespace Core.SalaryComponent.Entities;

public class SalaryComposition
{
    public Guid Id { get; set; }
    public string CompositionCode { get; set; } = string.Empty;
    public string CompositionName { get; set; } = string.Empty;
    public string CompositionType { get; set; } = string.Empty;
    public string Nature { get; set; } = "income";
    public string TaxOption { get; set; } = "taxable";
    public bool TaxDeduction { get; set; }
    public string? Quota { get; set; }
    public bool AllowExceedQuota { get; set; }
    public string ValueType { get; set; } = "currency";
    public string ValueCalculation { get; set; } = "formula";
    public string? SumScope { get; set; }
    public string? OrgLevel { get; set; }
    public string? SalaryComponentToSum { get; set; }
    public string? ValueFormula { get; set; }
    public string? Description { get; set; }
    public int ShowOnPayslip { get; set; } = 1;
    public int Source { get; set; } = 2;
    public int Status { get; set; } = 1;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
