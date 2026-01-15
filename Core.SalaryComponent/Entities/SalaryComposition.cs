namespace Core.SalaryComponent.Entities;

/// <summary>
/// Thành phần lương - Định nghĩa các loại thu nhập, khấu trừ trong bảng lương
/// </summary>
public class SalaryComposition
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid SalaryCompositionId { get; set; }

    /// <summary>
    /// Mã thành phần (viết liền, chỉ A-Z, 0-9, _)
    /// - Không được chứa khoảng trắng, dấu tiếng Việt
    /// - Tự động chuyển thành chữ IN HOA
    /// </summary>
    public string SalaryCompositionCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên thành phần lương
    /// </summary>
    public string SalaryCompositionName { get; set; } = string.Empty;

    /// <summary>
    /// Loại thành phần:
    /// - employee_info: Thông tin nhân viên
    /// - attendance: Chấm công
    /// - revenue: Doanh số
    /// - kpi: KPI
    /// - product: Sản phẩm
    /// - salary: Lương
    /// - pit: Thuế TNCN
    /// - insurance_union: Bảo hiểm - Công đoàn
    /// - other: Khác
    /// </summary>
    public string SalaryCompositionType { get; set; } = string.Empty;

    /// <summary>
    /// Tính chất:
    /// - income: Thu nhập (hiển thị TaxOption)
    /// - deduction: Khấu trừ (hiển thị TaxDeduction)
    /// - other: Khác (ẩn Định mức, cho phép chọn ValueType)
    /// </summary>
    public string SalaryCompositionNature { get; set; } = "income";

    /// <summary>
    /// Tùy chọn thuế - CHỈ áp dụng khi Nature = "income":
    /// - taxable: Chịu thuế
    /// - tax_exempt_full: Miễn thuế toàn phần
    /// - tax_exempt_partial: Miễn thuế một phần
    /// - null: Không áp dụng (Nature != "income")
    /// </summary>
    public string SalaryCompositionTaxOption { get; set; } = "taxable";

    /// <summary>
    /// Giảm trừ khi tính thuế - CHỈ áp dụng khi Nature = "deduction"
    /// - true: Có giảm trừ
    /// - false: Không giảm trừ (hoặc Nature != "deduction")
    /// </summary>
    public bool SalaryCompositionTaxDeduction { get; set; }

    /// <summary>
    /// Công thức định mức - CHỈ hiển thị khi Nature != "other"
    /// - Áp dụng cho Thu nhập và Khấu trừ
    /// - null: Không áp dụng (Nature = "other")
    /// </summary>
    public string? SalaryCompositionQuota { get; set; }

    /// <summary>
    /// Cho phép giá trị tính vượt quá định mức - CHỈ hiển thị khi Nature != "other"
    /// - false: Không áp dụng (Nature = "other")
    /// </summary>
    public bool SalaryCompositionAllowExceedQuota { get; set; }

    /// <summary>
    /// Kiểu giá trị:
    /// - number: Số
    /// - currency: Tiền tệ (mặc định)
    /// - percent: Phần trăm
    /// - text: Chữ
    /// - date: Ngày
    /// Lưu ý: Chỉ cho phép thay đổi khi Nature = "other", ngược lại mặc định "currency"
    /// </summary>
    public string SalaryCompositionValueType { get; set; } = "currency";

    /// <summary>
    /// Cách tính giá trị:
    /// - auto_sum: Tự động cộng tổng giá trị của các nhân viên (sử dụng SumScope, OrgLevel, SalaryComponentToSum)
    /// - formula: Tính theo công thức tự đặt (sử dụng ValueFormula)
    /// </summary>
    public string SalaryCompositionValueCalculation { get; set; } = "formula";

    /// <summary>
    /// Phạm vi cộng tổng - CHỈ áp dụng khi ValueCalculation = "auto_sum":
    /// - same_unit: Trong cùng đơn vị công tác
    /// - subordinate: Dưới quyền
    /// - org_structure: Thuộc cơ cấu tổ chức (hiển thị thêm OrgLevel)
    /// - null: Không áp dụng (ValueCalculation = "formula")
    /// </summary>
    public string? SalaryCompositionSumScope { get; set; }

    /// <summary>
    /// Cấp cơ cấu tổ chức - CHỈ áp dụng khi SumScope = "org_structure":
    /// - level_1, level_2, level_3, level_4
    /// - null: Không áp dụng (SumScope != "org_structure")
    /// </summary>
    public string? SalaryCompositionOrgLevel { get; set; }

    /// <summary>
    /// Mã thành phần lương để cộng giá trị - CHỈ áp dụng khi ValueCalculation = "auto_sum"
    /// - Chọn từ danh sách thành phần lương
    /// - null: Không áp dụng (ValueCalculation = "formula")
    /// </summary>
    public string? SalaryCompositionComponentToSum { get; set; }

    /// <summary>
    /// Công thức tính giá trị - CHỈ áp dụng khi ValueCalculation = "formula"
    /// - null: Không áp dụng (ValueCalculation = "auto_sum")
    /// </summary>
    public string? SalaryCompositionValueFormula { get; set; }

    /// <summary>
    /// Mô tả thành phần lương
    /// </summary>
    public string? SalaryCompositionDescription { get; set; }

    /// <summary>
    /// Hiển thị trên phiếu lương:
    /// - 1: Có (yes)
    /// - 2: Không (no)
    /// - 3: Chỉ hiển thị nếu giá trị khác 0 (if_not_zero)
    /// </summary>
    public int SalaryCompositionShowOnPayslip { get; set; } = 1;

    /// <summary>
    /// Nguồn tạo:
    /// - 1: Hệ thống (system) - do hệ thống tạo sẵn
    /// - 2: Tự thêm (manual) - người dùng tự tạo
    /// </summary>
    public int SalaryCompositionSource { get; set; } = 2;

    /// <summary>
    /// Trạng thái:
    /// - 1: Đang sử dụng
    /// - 0: Ngừng sử dụng
    /// </summary>
    public int SalaryCompositionStatus { get; set; } = 1;

    /// <summary>
    /// Công thức phần chịu thuế - CHỈ áp dụng khi TaxOption = "tax_exempt_partial"
    /// </summary>
    public string? SalaryCompositionTaxablePart { get; set; }

    /// <summary>
    /// Công thức phần miễn thuế - CHỈ áp dụng khi TaxOption = "tax_exempt_partial"
    /// </summary>
    public string? SalaryCompositionTaxExemptPart { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime SalaryCompositionCreatedDate { get; set; }

    /// <summary>
    /// Ngày sửa đổi lần cuối
    /// </summary>
    public DateTime SalaryCompositionModifiedDate { get; set; }
}
