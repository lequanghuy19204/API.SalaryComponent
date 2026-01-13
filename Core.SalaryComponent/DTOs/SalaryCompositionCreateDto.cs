using System.ComponentModel.DataAnnotations;

namespace Core.SalaryComponent.DTOs;

public class SalaryCompositionCreateDto
{
    [Required(ErrorMessage = "Mã thành phần không được để trống")]
    [MaxLength(255)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên thành phần không được để trống")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public List<Guid> UnitIds { get; set; } = new();

    [Required(ErrorMessage = "Loại thành phần không được để trống")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tính chất không được để trống")]
    public string Property { get; set; } = "income";

    public string? TaxOption { get; set; }

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
}
