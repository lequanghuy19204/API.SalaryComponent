namespace Core.SalaryComponent.DTOs;

public class SalaryCompositionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Guid> UnitIds { get; set; } = new();
    public string Type { get; set; } = string.Empty;
    public string Property { get; set; } = "income";
    public string TaxOption { get; set; } = "taxable";
    public bool DeductWhenCalculatingTax { get; set; }
    public string? Quota { get; set; }
    public bool AllowExceedQuota { get; set; }
    public string ValueType { get; set; } = "currency";
    public string ValueCalculation { get; set; } = "formula";
    public string? SumScope { get; set; }
    public string? OrgLevel { get; set; }
    public string? SalaryComponentToSum { get; set; }
    public string? ValueFormula { get; set; }
    public string? Description { get; set; }
    public string ShowOnPayslip { get; set; } = "yes";
    public string Source { get; set; } = "manual";
    public int Status { get; set; } = 1;
    public string? TaxablePart { get; set; }
    public string? TaxExemptPart { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
