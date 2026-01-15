namespace Core.SalaryComponent.Entities;

/// <summary>
/// Bảng trung gian liên kết Thành phần lương với Đơn vị tổ chức (Many-to-Many)
/// Áp dụng: Khi một thành phần lương được gán cho nhiều đơn vị tổ chức
/// </summary>
public class SalaryCompositionOrganization
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID thành phần lương (FK -> SalaryComposition)
    /// </summary>
    public Guid SalaryCompositionId { get; set; }

    /// <summary>
    /// ID đơn vị tổ chức được áp dụng thành phần lương này
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Ngày tạo bản ghi
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
