namespace Core.SalaryComponent.Entities;

/// <summary>
/// Đơn vị tổ chức - Cấu trúc cây phân cấp các đơn vị trong doanh nghiệp
/// Dùng để gán thành phần lương áp dụng cho từng đơn vị
/// </summary>
public class Organization
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// ID đơn vị cha (null nếu là đơn vị gốc)
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Mã đơn vị
    /// </summary>
    public string OrganizationCode { get; set; } = string.Empty;

    /// <summary>
    /// Tên đơn vị
    /// </summary>
    public string OrganizationName { get; set; } = string.Empty;

    /// <summary>
    /// Cấp đơn vị trong cây tổ chức (1-4)
    /// - Cấp 1: Công ty/Tổng công ty
    /// - Cấp 2: Chi nhánh/Khối
    /// - Cấp 3: Phòng ban
    /// - Cấp 4: Tổ/Nhóm
    /// </summary>
    public int OrganizationLevel { get; set; } = 1;

    /// <summary>
    /// Thứ tự sắp xếp trong cùng cấp
    /// </summary>
    public int OrganizationSortOrder { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool OrganizationIsActive { get; set; } = true;

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime OrganizationCreatedDate { get; set; }

    /// <summary>
    /// Ngày sửa đổi lần cuối
    /// </summary>
    public DateTime OrganizationModifiedDate { get; set; }

    /// <summary>
    /// Danh sách đơn vị con (dùng cho cấu trúc cây phía frontend)
    /// </summary>
    public List<Organization>? Items { get; set; }
}
