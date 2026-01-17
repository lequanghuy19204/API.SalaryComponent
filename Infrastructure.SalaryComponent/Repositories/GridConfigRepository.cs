using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

/// <summary>
/// Repository truy xuất dữ liệu cấu hình lưới từ database
/// </summary>
public class GridConfigRepository : IGridConfigRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Khởi tạo repository với chuỗi kết nối
    /// </summary>
    /// <param name="configuration">Cấu hình ứng dụng chứa chuỗi kết nối</param>
    public GridConfigRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    /// <summary>
    /// Tạo kết nối MySQL
    /// </summary>
    /// <returns>Đối tượng kết nối MySQL</returns>
    private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

    /// <summary>
    /// Lấy danh sách cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên lưới cần lấy cấu hình</param>
    /// <returns>Danh sách cấu hình lưới</returns>
    public async Task<IEnumerable<GridConfigDto>> GetByNameAsync(string gridName)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT grid_config_id, grid_config_name, grid_config_column_name,
                   grid_config_column_order, grid_config_is_visible, grid_config_width,
                   grid_config_is_pinned
            FROM pa_grid_config 
            WHERE grid_config_name = @Name
            ORDER BY grid_config_column_order";

        var results = await connection.QueryAsync<GridConfigEntity>(sql, new { Name = gridName });
        return results.Select(MapToDto);
    }

    /// <summary>
    /// Lưu cấu hình lưới (xóa cũ và thêm mới theo transaction)
    /// </summary>
    /// <param name="dto">DTO chứa thông tin cấu hình lưới cần lưu</param>
    public async Task SaveAsync(GridConfigSaveDto dto)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await connection.ExecuteAsync(
                "DELETE FROM pa_grid_config WHERE grid_config_name = @Name",
                new { Name = dto.GridConfigName }, transaction);

            if (dto.Columns.Count > 0)
            {
                var sql = @"
                    INSERT INTO pa_grid_config 
                    (grid_config_id, grid_config_name, grid_config_column_name,
                     grid_config_column_order, grid_config_is_visible, grid_config_width,
                     grid_config_is_pinned, grid_config_created_date, grid_config_modified_date)
                    VALUES 
                    (@Id, @Name, @ColumnName, @Order, @IsVisible, @Width, @IsPinned, NOW(), NOW())";

                var order = 0;
                foreach (var col in dto.Columns)
                {
                    await connection.ExecuteAsync(sql, new
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = dto.GridConfigName,
                        ColumnName = col.DataField,
                        Order = order++,
                        IsVisible = col.Visible,
                        Width = col.Width,
                        IsPinned = col.IsPinned
                    }, transaction);
                }
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Xóa cấu hình lưới theo tên
    /// </summary>
    /// <param name="gridName">Tên lưới cần xóa cấu hình</param>
    public async Task DeleteByNameAsync(string gridName)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM pa_grid_config WHERE grid_config_name = @Name",
            new { Name = gridName });
    }

    /// <summary>
    /// Chuyển đổi entity sang DTO
    /// </summary>
    /// <param name="entity">Entity cần chuyển đổi</param>
    /// <returns>DTO cấu hình lưới</returns>
    private static GridConfigDto MapToDto(GridConfigEntity entity)
    {
        return new GridConfigDto
        {
            GridConfigId = Guid.Parse(entity.GridConfigId),
            GridConfigName = entity.GridConfigName,
            GridConfigColumnName = entity.GridConfigColumnName,
            GridConfigColumnOrder = entity.GridConfigColumnOrder,
            GridConfigIsVisible = entity.GridConfigIsVisible,
            GridConfigWidth = entity.GridConfigWidth,
            GridConfigIsPinned = entity.GridConfigIsPinned
        };
    }

    /// <summary>
    /// Entity ánh xạ với bảng pa_grid_config
    /// </summary>
    private class GridConfigEntity
    {
        public string GridConfigId { get; set; } = string.Empty;
        public string GridConfigName { get; set; } = string.Empty;
        public string GridConfigColumnName { get; set; } = string.Empty;
        public int GridConfigColumnOrder { get; set; }
        public bool GridConfigIsVisible { get; set; }
        public int GridConfigWidth { get; set; }
        public bool GridConfigIsPinned { get; set; }
    }
}
