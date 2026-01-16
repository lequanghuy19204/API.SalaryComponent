namespace Core.SalaryComponent.Entities;

/// <summary>
/// Cấu hình cột của grid - lưu thông tin hiển thị, thứ tự, ghim cột
/// </summary>
public class GridConfig
{
    public Guid GridConfigId { get; set; }
    public string GridConfigName { get; set; } = string.Empty;
    public string GridConfigColumnName { get; set; } = string.Empty;
    public int GridConfigColumnOrder { get; set; }
    public bool GridConfigIsVisible { get; set; } = true;
    public int GridConfigWidth { get; set; } = 100;
    public bool GridConfigIsPinned { get; set; }
    public DateTime GridConfigCreatedDate { get; set; }
    public DateTime GridConfigModifiedDate { get; set; }
}
