namespace Core.SalaryComponent.Entities;

/// <summary>
/// Thành phần lương hệ thống - Danh mục master do hệ thống định nghĩa sẵn
/// </summary>
public class SalaryCompositionSystem
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid SalaryCompositionSystemId { get; set; }

    /// <summary>
    /// Mã thành phần
    /// </summary>
    public string SalaryCompositionSystemCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên thành phần lương
    /// </summary>
    public string SalaryCompositionSystemName { get; set; } = string.Empty;

    /// <summary>
    /// Loại thành phần
    /// </summary>
    public string SalaryCompositionSystemType { get; set; } = string.Empty;

    /// <summary>
    /// Tính chất: income, deduction, other
    /// </summary>
    public string SalaryCompositionSystemNature { get; set; } = "income";

    /// <summary>
    /// Tùy chọn thuế
    /// </summary>
    public string? SalaryCompositionSystemTaxOption { get; set; }

    /// <summary>
    /// Giảm trừ khi tính thuế
    /// </summary>
    public bool SalaryCompositionSystemTaxDeduction { get; set; }

    /// <summary>
    /// Công thức định mức
    /// </summary>
    public string? SalaryCompositionSystemQuota { get; set; }

    /// <summary>
    /// Cho phép giá trị vượt quá định mức
    /// </summary>
    public bool SalaryCompositionSystemAllowExceedQuota { get; set; }

    /// <summary>
    /// Kiểu giá trị: number, currency, percent, text, date
    /// </summary>
    public string SalaryCompositionSystemValueType { get; set; } = "currency";

    /// <summary>
    /// Cách tính giá trị: auto_sum, formula
    /// </summary>
    public string SalaryCompositionSystemValueCalculation { get; set; } = "formula";

    /// <summary>
    /// Phạm vi cộng tổng
    /// </summary>
    public string? SalaryCompositionSystemSumScope { get; set; }

    /// <summary>
    /// Cấp cơ cấu tổ chức
    /// </summary>
    public string? SalaryCompositionSystemOrgLevel { get; set; }

    /// <summary>
    /// Mã thành phần lương để cộng giá trị
    /// </summary>
    public string? SalaryCompositionSystemComponentToSum { get; set; }

    /// <summary>
    /// Công thức tính giá trị
    /// </summary>
    public string? SalaryCompositionSystemValueFormula { get; set; }

    /// <summary>
    /// Mô tả thành phần lương
    /// </summary>
    public string? SalaryCompositionSystemDescription { get; set; }

    /// <summary>
    /// Hiển thị trên phiếu lương: 1-Có, 2-Không, 3-Nếu khác 0
    /// </summary>
    public int SalaryCompositionSystemShowOnPayslip { get; set; } = 1;

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime SalaryCompositionSystemCreatedDate { get; set; }
}
