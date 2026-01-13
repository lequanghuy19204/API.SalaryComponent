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
            SELECT id, parent_id, organization_name AS name
            FROM pa_organization
            WHERE is_active = 1
            ORDER BY level, sort_order, organization_name";

        var allOrgs = (await connection.QueryAsync<OrganizationFlatDto>(sql)).ToList();

        return BuildTree(allOrgs, null);
    }

    private static List<OrganizationTreeDto> BuildTree(List<OrganizationFlatDto> allOrgs, string? parentId)
    {
        return allOrgs
            .Where(o => o.ParentId == parentId)
            .Select(o => new OrganizationTreeDto
            {
                Id = o.Id,
                Name = o.Name,
                Items = BuildTree(allOrgs, o.Id) is { Count: > 0 } children ? children : null
            })
            .ToList();
    }

    private class OrganizationFlatDto
    {
        public string Id { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
