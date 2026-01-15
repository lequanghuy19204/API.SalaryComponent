using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly string _connectionString;

    public OrganizationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

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

    private class OrganizationFlatDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
    }

    private class ParentMappingDto
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string? ParentId { get; set; }
    }
}
