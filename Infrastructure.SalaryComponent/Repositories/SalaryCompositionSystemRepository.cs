using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

public class SalaryCompositionSystemRepository : ISalaryCompositionSystemRepository
{
    private readonly string _connectionString;

    public SalaryCompositionSystemRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found");
    }

    private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

    public async Task<IEnumerable<SalaryCompositionSystemDto>> GetAllAsync()
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT 
                salary_composition_system_id,
                salary_composition_system_code,
                salary_composition_system_name,
                salary_composition_system_type,
                salary_composition_system_nature,
                salary_composition_system_tax_option,
                salary_composition_system_tax_deduction,
                salary_composition_system_quota,
                salary_composition_system_allow_exceed_quota,
                salary_composition_system_value_type,
                salary_composition_system_value_calculation,
                salary_composition_system_sum_scope,
                salary_composition_system_org_level,
                salary_composition_system_component_to_sum,
                salary_composition_system_value_formula,
                salary_composition_system_description,
                salary_composition_system_show_on_payslip,
                salary_composition_system_created_date
            FROM pa_salary_composition_system 
            ORDER BY salary_composition_system_created_date DESC";

        var results = await connection.QueryAsync<SalaryCompositionSystemEntity>(sql);
        return results.Select(MapToDto);
    }

    public async Task<SalaryCompositionSystemDto?> GetByIdAsync(Guid id)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT 
                salary_composition_system_id,
                salary_composition_system_code,
                salary_composition_system_name,
                salary_composition_system_type,
                salary_composition_system_nature,
                salary_composition_system_tax_option,
                salary_composition_system_tax_deduction,
                salary_composition_system_quota,
                salary_composition_system_allow_exceed_quota,
                salary_composition_system_value_type,
                salary_composition_system_value_calculation,
                salary_composition_system_sum_scope,
                salary_composition_system_org_level,
                salary_composition_system_component_to_sum,
                salary_composition_system_value_formula,
                salary_composition_system_description,
                salary_composition_system_show_on_payslip,
                salary_composition_system_created_date
            FROM pa_salary_composition_system 
            WHERE salary_composition_system_id = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<SalaryCompositionSystemEntity>(sql, new { Id = id.ToString() });
        return result == null ? null : MapToDto(result);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM pa_salary_composition_system WHERE salary_composition_system_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id.ToString() });
        return affected > 0;
    }

    public async Task<Guid?> GetRootOrganizationIdAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT organization_id FROM pa_organization WHERE parent_id IS NULL LIMIT 1";
        var result = await connection.QueryFirstOrDefaultAsync<string>(sql);
        return result == null ? null : Guid.Parse(result);
    }

    public async Task<bool> IsCodeExistsInCompositionAsync(string code)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM pa_salary_composition WHERE salary_composition_code = @Code";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code });
        return count > 0;
    }

    public async Task<Guid?> GetCompositionIdByCodeAsync(string code)
    {
        using var connection = CreateConnection();
        var sql = "SELECT salary_composition_id FROM pa_salary_composition WHERE salary_composition_code = @Code LIMIT 1";
        var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Code = code });
        return result == null ? null : Guid.Parse(result);
    }

    public async Task<PagedResultDto<SalaryCompositionSystemDto>> GetPagedAsync(int pageNumber, int pageSize, string? searchText = null, string? type = null)
    {
        using var connection = CreateConnection();

        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            whereClauses.Add("(salary_composition_system_code LIKE @SearchText OR salary_composition_system_name LIKE @SearchText)");
            parameters.Add("SearchText", $"%{searchText}%");
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            whereClauses.Add("salary_composition_system_type = @Type");
            parameters.Add("Type", type);
        }

        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        var countSql = $"SELECT COUNT(1) FROM pa_salary_composition_system {whereClause}";
        var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var offset = (pageNumber - 1) * pageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", pageSize);

        var dataSql = $@"
            SELECT 
                salary_composition_system_id,
                salary_composition_system_code,
                salary_composition_system_name,
                salary_composition_system_type,
                salary_composition_system_nature,
                salary_composition_system_tax_option,
                salary_composition_system_tax_deduction,
                salary_composition_system_quota,
                salary_composition_system_allow_exceed_quota,
                salary_composition_system_value_type,
                salary_composition_system_value_calculation,
                salary_composition_system_sum_scope,
                salary_composition_system_org_level,
                salary_composition_system_component_to_sum,
                salary_composition_system_value_formula,
                salary_composition_system_description,
                salary_composition_system_show_on_payslip,
                salary_composition_system_created_date
            FROM pa_salary_composition_system 
            {whereClause}
            ORDER BY salary_composition_system_created_date DESC
            LIMIT @PageSize OFFSET @Offset";

        var results = await connection.QueryAsync<SalaryCompositionSystemEntity>(dataSql, parameters);

        return new PagedResultDto<SalaryCompositionSystemDto>
        {
            Data = results.Select(MapToDto),
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static string ConvertShowOnPayslipToString(int value) => value switch
    {
        1 => "yes",
        2 => "no",
        3 => "if_not_zero",
        _ => "yes"
    };

    private static SalaryCompositionSystemDto MapToDto(SalaryCompositionSystemEntity entity)
    {
        return new SalaryCompositionSystemDto
        {
            SalaryCompositionSystemId = Guid.Parse(entity.SalaryCompositionSystemId),
            SalaryCompositionSystemCode = entity.SalaryCompositionSystemCode,
            SalaryCompositionSystemName = entity.SalaryCompositionSystemName,
            SalaryCompositionSystemType = entity.SalaryCompositionSystemType,
            SalaryCompositionSystemNature = entity.SalaryCompositionSystemNature,
            SalaryCompositionSystemTaxOption = entity.SalaryCompositionSystemTaxOption,
            SalaryCompositionSystemTaxDeduction = entity.SalaryCompositionSystemTaxDeduction,
            SalaryCompositionSystemQuota = entity.SalaryCompositionSystemQuota,
            SalaryCompositionSystemAllowExceedQuota = entity.SalaryCompositionSystemAllowExceedQuota,
            SalaryCompositionSystemValueType = entity.SalaryCompositionSystemValueType ?? "currency",
            SalaryCompositionSystemValueCalculation = entity.SalaryCompositionSystemValueCalculation ?? "formula",
            SalaryCompositionSystemSumScope = entity.SalaryCompositionSystemSumScope,
            SalaryCompositionSystemOrgLevel = entity.SalaryCompositionSystemOrgLevel,
            SalaryCompositionSystemComponentToSum = entity.SalaryCompositionSystemComponentToSum,
            SalaryCompositionSystemValueFormula = entity.SalaryCompositionSystemValueFormula,
            SalaryCompositionSystemDescription = entity.SalaryCompositionSystemDescription,
            SalaryCompositionSystemShowOnPayslip = ConvertShowOnPayslipToString(entity.SalaryCompositionSystemShowOnPayslip),
            SalaryCompositionSystemCreatedDate = entity.SalaryCompositionSystemCreatedDate
        };
    }

    private class SalaryCompositionSystemEntity
    {
        public string SalaryCompositionSystemId { get; set; } = string.Empty;
        public string SalaryCompositionSystemCode { get; set; } = string.Empty;
        public string SalaryCompositionSystemName { get; set; } = string.Empty;
        public string SalaryCompositionSystemType { get; set; } = string.Empty;
        public string SalaryCompositionSystemNature { get; set; } = string.Empty;
        public string? SalaryCompositionSystemTaxOption { get; set; }
        public bool SalaryCompositionSystemTaxDeduction { get; set; }
        public string? SalaryCompositionSystemQuota { get; set; }
        public bool SalaryCompositionSystemAllowExceedQuota { get; set; }
        public string? SalaryCompositionSystemValueType { get; set; }
        public string? SalaryCompositionSystemValueCalculation { get; set; }
        public string? SalaryCompositionSystemSumScope { get; set; }
        public string? SalaryCompositionSystemOrgLevel { get; set; }
        public string? SalaryCompositionSystemComponentToSum { get; set; }
        public string? SalaryCompositionSystemValueFormula { get; set; }
        public string? SalaryCompositionSystemDescription { get; set; }
        public int SalaryCompositionSystemShowOnPayslip { get; set; }
        public DateTime SalaryCompositionSystemCreatedDate { get; set; }
    }
}
