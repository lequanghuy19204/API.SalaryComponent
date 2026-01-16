namespace Core.SalaryComponent.DTOs;

public class GridConfigDto
{
    public Guid GridConfigId { get; set; }
    public string GridConfigName { get; set; } = string.Empty;
    public string GridConfigColumnName { get; set; } = string.Empty;
    public int GridConfigColumnOrder { get; set; }
    public bool GridConfigIsVisible { get; set; } = true;
    public int GridConfigWidth { get; set; } = 100;
    public bool GridConfigIsPinned { get; set; }
}

public class GridConfigSaveDto
{
    public string GridConfigName { get; set; } = string.Empty;
    public List<GridConfigColumnDto> Columns { get; set; } = new();
}

public class GridConfigColumnDto
{
    public string DataField { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool Visible { get; set; } = true;
    public int Width { get; set; } = 100;
    public bool IsPinned { get; set; }
}
