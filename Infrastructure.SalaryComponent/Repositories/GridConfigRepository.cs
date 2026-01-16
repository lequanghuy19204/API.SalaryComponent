using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

public class GridConfigRepository : IGridConfigRepository
{
    private readonly string _connectionString;

    public GridConfigRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

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

    public async Task DeleteByNameAsync(string gridName)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM pa_grid_config WHERE grid_config_name = @Name",
            new { Name = gridName });
    }

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
