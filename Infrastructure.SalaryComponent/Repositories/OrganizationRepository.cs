using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

/// <summary>
/// Repository truy xuất dữ liệu đơn vị/tổ chức từ database
/// </summary>
public class OrganizationRepository : IOrganizationRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Khởi tạo repository với chuỗi kết nối
    /// </summary>
    /// <param name="configuration">Cấu hình ứng dụng chứa chuỗi kết nối</param>
    public OrganizationRepository(IConfiguration configuration)
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
    /// Lấy cây đơn vị tổ chức theo cấu trúc phân cấp
    /// </summary>
    /// <returns>Danh sách cây đơn vị tổ chức</returns>
    public async Task<IEnumerable<OrganizationTreeDto>> GetTreeAsync()
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT organization_id, organization_name
            FROM pa_organization
            WHERE organization_is_active = 1
            ORDER BY organization_level, organization_sort_order, organization_name";

        var allOrgs = (await connection.QueryAsync<OrganizationFlatDto>(sql)).ToList();

        var parentSql = @"
            SELECT organization_id, parent_id
            FROM pa_organization
            WHERE organization_is_active = 1";
        
        var parentMappings = (await connection.QueryAsync<ParentMappingDto>(parentSql)).ToList();
        var parentDict = parentMappings.ToDictionary(x => x.OrganizationId, x => x.ParentId);

        return BuildTree(allOrgs, parentDict, null);
    }

    /// <summary>
    /// Xây dựng cây từ danh sách phẳng
    /// </summary>
    /// <param name="allOrgs">Danh sách tất cả đơn vị dạng phẳng</param>
    /// <param name="parentDict">Từ điển ánh xạ ID đơn vị với ID đơn vị cha</param>
    /// <param name="parentId">ID đơn vị cha hiện tại</param>
    /// <returns>Danh sách cây đơn vị con</returns>
    private static List<OrganizationTreeDto> BuildTree(
        List<OrganizationFlatDto> allOrgs, 
        Dictionary<string, string?> parentDict, 
        string? parentId)
    {
        return allOrgs
            .Where(o => parentDict.TryGetValue(o.OrganizationId, out var pid) && pid == parentId)
            .Select(o => new OrganizationTreeDto
            {
                OrganizationId = o.OrganizationId,
                OrganizationName = o.OrganizationName,
                Items = BuildTree(allOrgs, parentDict, o.OrganizationId) is { Count: > 0 } children ? children : null
            })
            .ToList();
    }

    /// <summary>
    /// DTO đơn vị dạng phẳng
    /// </summary>
    private class OrganizationFlatDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO ánh xạ quan hệ cha-con
    /// </summary>
    private class ParentMappingDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }
}
