using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

public class SalaryCompositionRepository : ISalaryCompositionRepository
{
    private readonly string _connectionString;

    public SalaryCompositionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

    public async Task<Guid> CreateAsync(SalaryCompositionCreateDto dto)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var id = Guid.NewGuid();
            var showOnPayslip = ConvertShowOnPayslip(dto.SalaryCompositionShowOnPayslip);
            var source = dto.SalaryCompositionSource == "system" ? 1 : 2;

            var sql = @"
                INSERT INTO pa_salary_composition 
                (salary_composition_id, salary_composition_code, salary_composition_name, 
                 salary_composition_type, salary_composition_nature, salary_composition_tax_option, 
                 salary_composition_tax_deduction, salary_composition_quota, salary_composition_allow_exceed_quota, 
                 salary_composition_value_type, salary_composition_value_calculation, salary_composition_sum_scope, 
                 salary_composition_org_level, salary_composition_component_to_sum, salary_composition_value_formula, 
                 salary_composition_description, salary_composition_show_on_payslip, salary_composition_source, 
                 salary_composition_status, salary_composition_taxable_part, salary_composition_tax_exempt_part,
                 salary_composition_created_date, salary_composition_modified_date)
                VALUES 
                (@Id, @Code, @Name, @Type, @Nature, @TaxOption, @TaxDeduction, @Quota, @AllowExceedQuota,
                 @ValueType, @ValueCalculation, @SumScope, @OrgLevel, @ComponentToSum, @ValueFormula,
                 @Description, @ShowOnPayslip, @Source, @Status, @TaxablePart, @TaxExemptPart, NOW(), NOW())";

            await connection.ExecuteAsync(sql, new
            {
                Id = id.ToString(),
                Code = dto.SalaryCompositionCode,
                Name = dto.SalaryCompositionName,
                Type = dto.SalaryCompositionType,
                Nature = dto.SalaryCompositionNature,
                TaxOption = dto.SalaryCompositionTaxOption,
                TaxDeduction = dto.SalaryCompositionTaxDeduction,
                Quota = dto.SalaryCompositionQuota,
                AllowExceedQuota = dto.SalaryCompositionAllowExceedQuota,
                ValueType = dto.SalaryCompositionValueType,
                ValueCalculation = dto.SalaryCompositionValueCalculation,
                SumScope = dto.SalaryCompositionSumScope,
                OrgLevel = dto.SalaryCompositionOrgLevel,
                ComponentToSum = dto.SalaryCompositionComponentToSum,
                ValueFormula = dto.SalaryCompositionValueFormula,
                Description = dto.SalaryCompositionDescription,
                ShowOnPayslip = showOnPayslip,
                Source = source,
                Status = dto.SalaryCompositionStatus,
                TaxablePart = dto.SalaryCompositionTaxablePart,
                TaxExemptPart = dto.SalaryCompositionTaxExemptPart
            }, transaction);

            if (dto.OrganizationIds.Count > 0)
            {
                var orgSql = @"
                    INSERT INTO pa_salary_composition_organization 
                    (salary_composition_organization_id, salary_composition_id, organization_id, 
                     salary_composition_organization_created_date)
                    VALUES (@Id, @SalaryCompositionId, @OrganizationId, NOW())";

                foreach (var orgId in dto.OrganizationIds)
                {
                    await connection.ExecuteAsync(orgSql, new
                    {
                        Id = Guid.NewGuid().ToString(),
                        SalaryCompositionId = id.ToString(),
                        OrganizationId = orgId.ToString()
                    }, transaction);
                }
            }

            await transaction.CommitAsync();
            return id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SalaryCompositionDto?> GetByIdAsync(Guid id)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT salary_composition_id, salary_composition_code, salary_composition_name,
                   salary_composition_type, salary_composition_nature, salary_composition_tax_option,
                   salary_composition_tax_deduction, salary_composition_quota, salary_composition_allow_exceed_quota,
                   salary_composition_value_type, salary_composition_value_calculation, salary_composition_sum_scope,
                   salary_composition_org_level, salary_composition_component_to_sum, salary_composition_value_formula,
                   salary_composition_description, salary_composition_show_on_payslip, salary_composition_source,
                   salary_composition_status, salary_composition_taxable_part, salary_composition_tax_exempt_part,
                   salary_composition_created_date, salary_composition_modified_date
            FROM pa_salary_composition 
            WHERE salary_composition_id = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<SalaryCompositionEntity>(sql, new { Id = id.ToString() });
        if (result == null) return null;

        var dto = MapToDto(result);

        var orgSql = "SELECT organization_id FROM pa_salary_composition_organization WHERE salary_composition_id = @Id";
        var orgIds = await connection.QueryAsync<string>(orgSql, new { Id = id.ToString() });
        dto.OrganizationIds = orgIds.Select(Guid.Parse).ToList();

        return dto;
    }

    public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT salary_composition_id, salary_composition_code, salary_composition_name,
                   salary_composition_type, salary_composition_nature, salary_composition_tax_option,
                   salary_composition_tax_deduction, salary_composition_quota, salary_composition_allow_exceed_quota,
                   salary_composition_value_type, salary_composition_value_calculation, salary_composition_sum_scope,
                   salary_composition_org_level, salary_composition_component_to_sum, salary_composition_value_formula,
                   salary_composition_description, salary_composition_show_on_payslip, salary_composition_source,
                   salary_composition_status, salary_composition_taxable_part, salary_composition_tax_exempt_part,
                   salary_composition_created_date, salary_composition_modified_date
            FROM pa_salary_composition 
            ORDER BY salary_composition_created_date DESC";

        var results = await connection.QueryAsync<SalaryCompositionEntity>(sql);
        var dtos = results.Select(MapToDto).ToList();

        var orgSql = "SELECT salary_composition_id, organization_id FROM pa_salary_composition_organization";
        var orgMappings = await connection.QueryAsync<OrgMappingEntity>(orgSql);
        var orgDict = orgMappings
            .GroupBy(x => x.SalaryCompositionId)
            .ToDictionary(g => g.Key, g => g.Select(x => Guid.Parse(x.OrganizationId)).ToList());

        foreach (var dto in dtos)
        {
            if (orgDict.TryGetValue(dto.SalaryCompositionId.ToString(), out var organizationIds))
            {
                dto.OrganizationIds = organizationIds;
            }
        }

        return dtos;
    }

    public async Task<bool> UpdateAsync(Guid id, SalaryCompositionCreateDto dto)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var showOnPayslip = ConvertShowOnPayslip(dto.SalaryCompositionShowOnPayslip);
            var source = dto.SalaryCompositionSource == "system" ? 1 : 2;

            var sql = @"
                UPDATE pa_salary_composition SET
                    salary_composition_code = @Code,
                    salary_composition_name = @Name,
                    salary_composition_type = @Type,
                    salary_composition_nature = @Nature,
                    salary_composition_tax_option = @TaxOption,
                    salary_composition_tax_deduction = @TaxDeduction,
                    salary_composition_quota = @Quota,
                    salary_composition_allow_exceed_quota = @AllowExceedQuota,
                    salary_composition_value_type = @ValueType,
                    salary_composition_value_calculation = @ValueCalculation,
                    salary_composition_sum_scope = @SumScope,
                    salary_composition_org_level = @OrgLevel,
                    salary_composition_component_to_sum = @ComponentToSum,
                    salary_composition_value_formula = @ValueFormula,
                    salary_composition_description = @Description,
                    salary_composition_show_on_payslip = @ShowOnPayslip,
                    salary_composition_source = @Source,
                    salary_composition_status = @Status,
                    salary_composition_taxable_part = @TaxablePart,
                    salary_composition_tax_exempt_part = @TaxExemptPart,
                    salary_composition_modified_date = NOW()
                WHERE salary_composition_id = @Id";

            var affected = await connection.ExecuteAsync(sql, new
            {
                Id = id.ToString(),
                Code = dto.SalaryCompositionCode,
                Name = dto.SalaryCompositionName,
                Type = dto.SalaryCompositionType,
                Nature = dto.SalaryCompositionNature,
                TaxOption = dto.SalaryCompositionTaxOption,
                TaxDeduction = dto.SalaryCompositionTaxDeduction,
                Quota = dto.SalaryCompositionQuota,
                AllowExceedQuota = dto.SalaryCompositionAllowExceedQuota,
                ValueType = dto.SalaryCompositionValueType,
                ValueCalculation = dto.SalaryCompositionValueCalculation,
                SumScope = dto.SalaryCompositionSumScope,
                OrgLevel = dto.SalaryCompositionOrgLevel,
                ComponentToSum = dto.SalaryCompositionComponentToSum,
                ValueFormula = dto.SalaryCompositionValueFormula,
                Description = dto.SalaryCompositionDescription,
                ShowOnPayslip = showOnPayslip,
                Source = source,
                Status = dto.SalaryCompositionStatus,
                TaxablePart = dto.SalaryCompositionTaxablePart,
                TaxExemptPart = dto.SalaryCompositionTaxExemptPart
            }, transaction);

            await connection.ExecuteAsync(
                "DELETE FROM pa_salary_composition_organization WHERE salary_composition_id = @Id",
                new { Id = id.ToString() }, transaction);

            if (dto.OrganizationIds.Count > 0)
            {
                var orgSql = @"
                    INSERT INTO pa_salary_composition_organization 
                    (salary_composition_organization_id, salary_composition_id, organization_id, 
                     salary_composition_organization_created_date)
                    VALUES (@Id, @SalaryCompositionId, @OrganizationId, NOW())";

                foreach (var orgId in dto.OrganizationIds)
                {
                    await connection.ExecuteAsync(orgSql, new
                    {
                        Id = Guid.NewGuid().ToString(),
                        SalaryCompositionId = id.ToString(),
                        OrganizationId = orgId.ToString()
                    }, transaction);
                }
            }

            await transaction.CommitAsync();
            return affected > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await connection.ExecuteAsync(
                "DELETE FROM pa_salary_composition_organization WHERE salary_composition_id = @Id",
                new { Id = id.ToString() }, transaction);

            var sql = "DELETE FROM pa_salary_composition WHERE salary_composition_id = @Id";
            var affected = await connection.ExecuteAsync(sql, new { Id = id.ToString() }, transaction);

            await transaction.CommitAsync();
            return affected > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null)
    {
        using var connection = CreateConnection();

        var sql = excludeId.HasValue
            ? "SELECT COUNT(1) FROM pa_salary_composition WHERE salary_composition_code = @Code AND salary_composition_id != @ExcludeId AND salary_composition_status = 1"
            : "SELECT COUNT(1) FROM pa_salary_composition WHERE salary_composition_code = @Code AND salary_composition_status = 1";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code, ExcludeId = excludeId?.ToString() });
        return count > 0;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE pa_salary_composition SET salary_composition_status = @Status, salary_composition_modified_date = NOW() WHERE salary_composition_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id.ToString(), Status = status });
        return affected > 0;
    }

    public async Task BulkUpdateStatusAsync(List<Guid> ids, int status)
    {
        if (ids.Count == 0) return;

        using var connection = CreateConnection();
        var idStrings = ids.Select(id => id.ToString()).ToList();
        var sql = "UPDATE pa_salary_composition SET salary_composition_status = @Status, salary_composition_modified_date = NOW() WHERE salary_composition_id IN @Ids";
        await connection.ExecuteAsync(sql, new { Ids = idStrings, Status = status });
    }

    public async Task<PagedResultDto<SalaryCompositionDto>> GetPagedAsync(PagingRequestDto request)
    {
        using var connection = CreateConnection();

        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            whereClauses.Add("(salary_composition_code LIKE @SearchText OR salary_composition_name LIKE @SearchText)");
            parameters.Add("SearchText", $"%{request.SearchText}%");
        }

        if (request.Status.HasValue)
        {
            whereClauses.Add("salary_composition_status = @Status");
            parameters.Add("Status", request.Status.Value);
        }

        // Filter by organization IDs - find salary compositions that belong to any of the selected organizations
        if (request.OrganizationIds != null && request.OrganizationIds.Count > 0)
        {
            var orgIdStrings = request.OrganizationIds.Select(id => id.ToString()).ToList();
            whereClauses.Add(@"salary_composition_id IN (
                SELECT DISTINCT salary_composition_id 
                FROM pa_salary_composition_organization 
                WHERE organization_id IN @OrganizationIds
            )");
            parameters.Add("OrganizationIds", orgIdStrings);
        }

        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        var countSql = $"SELECT COUNT(1) FROM pa_salary_composition {whereClause}";
        var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var offset = (request.PageNumber - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        var dataSql = $@"
            SELECT salary_composition_id, salary_composition_code, salary_composition_name,
                   salary_composition_type, salary_composition_nature, salary_composition_tax_option,
                   salary_composition_tax_deduction, salary_composition_quota, salary_composition_allow_exceed_quota,
                   salary_composition_value_type, salary_composition_value_calculation, salary_composition_sum_scope,
                   salary_composition_org_level, salary_composition_component_to_sum, salary_composition_value_formula,
                   salary_composition_description, salary_composition_show_on_payslip, salary_composition_source,
                   salary_composition_status, salary_composition_taxable_part, salary_composition_tax_exempt_part,
                   salary_composition_created_date, salary_composition_modified_date
            FROM pa_salary_composition 
            {whereClause}
            ORDER BY salary_composition_created_date DESC
            LIMIT @PageSize OFFSET @Offset";

        var results = await connection.QueryAsync<SalaryCompositionEntity>(dataSql, parameters);
        var dtos = results.Select(MapToDto).ToList();

        if (dtos.Count > 0)
        {
            var ids = dtos.Select(d => d.SalaryCompositionId.ToString()).ToList();
            var orgSql = "SELECT salary_composition_id, organization_id FROM pa_salary_composition_organization WHERE salary_composition_id IN @Ids";
            var orgMappings = await connection.QueryAsync<OrgMappingEntity>(orgSql, new { Ids = ids });
            var orgDict = orgMappings
                .GroupBy(x => x.SalaryCompositionId)
                .ToDictionary(g => g.Key, g => g.Select(x => Guid.Parse(x.OrganizationId)).ToList());

            foreach (var dto in dtos)
            {
                if (orgDict.TryGetValue(dto.SalaryCompositionId.ToString(), out var organizationIds))
                {
                    dto.OrganizationIds = organizationIds;
                }
            }
        }

        return new PagedResultDto<SalaryCompositionDto>
        {
            Data = dtos,
            TotalRecords = totalRecords,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private static int ConvertShowOnPayslip(string value) => value switch
    {
        "yes" => 1,
        "no" => 2,
        "if_not_zero" => 3,
        _ => 1
    };

    private static string ConvertShowOnPayslipToString(int value) => value switch
    {
        1 => "yes",
        2 => "no",
        3 => "if_not_zero",
        _ => "yes"
    };

    private static string ConvertSourceToString(int value) => value switch
    {
        1 => "system",
        2 => "manual",
        _ => "manual"
    };

    private static SalaryCompositionDto MapToDto(SalaryCompositionEntity entity)
    {
        return new SalaryCompositionDto
        {
            SalaryCompositionId = Guid.Parse(entity.SalaryCompositionId),
            SalaryCompositionCode = entity.SalaryCompositionCode,
            SalaryCompositionName = entity.SalaryCompositionName,
            SalaryCompositionType = entity.SalaryCompositionType,
            SalaryCompositionNature = entity.SalaryCompositionNature,
            SalaryCompositionTaxOption = entity.SalaryCompositionTaxOption ?? "taxable",
            SalaryCompositionTaxDeduction = entity.SalaryCompositionTaxDeduction,
            SalaryCompositionQuota = entity.SalaryCompositionQuota,
            SalaryCompositionAllowExceedQuota = entity.SalaryCompositionAllowExceedQuota,
            SalaryCompositionValueType = entity.SalaryCompositionValueType ?? "currency",
            SalaryCompositionValueCalculation = entity.SalaryCompositionValueCalculation ?? "formula",
            SalaryCompositionSumScope = entity.SalaryCompositionSumScope,
            SalaryCompositionOrgLevel = entity.SalaryCompositionOrgLevel,
            SalaryCompositionComponentToSum = entity.SalaryCompositionComponentToSum,
            SalaryCompositionValueFormula = entity.SalaryCompositionValueFormula,
            SalaryCompositionDescription = entity.SalaryCompositionDescription,
            SalaryCompositionShowOnPayslip = ConvertShowOnPayslipToString(entity.SalaryCompositionShowOnPayslip),
            SalaryCompositionSource = ConvertSourceToString(entity.SalaryCompositionSource),
            SalaryCompositionStatus = entity.SalaryCompositionStatus,
            SalaryCompositionTaxablePart = entity.SalaryCompositionTaxablePart,
            SalaryCompositionTaxExemptPart = entity.SalaryCompositionTaxExemptPart,
            SalaryCompositionCreatedDate = entity.SalaryCompositionCreatedDate,
            SalaryCompositionModifiedDate = entity.SalaryCompositionModifiedDate
        };
    }

    private class SalaryCompositionEntity
    {
        public string SalaryCompositionId { get; set; } = string.Empty;
        public string SalaryCompositionCode { get; set; } = string.Empty;
        public string SalaryCompositionName { get; set; } = string.Empty;
        public string SalaryCompositionType { get; set; } = string.Empty;
        public string SalaryCompositionNature { get; set; } = string.Empty;
        public string? SalaryCompositionTaxOption { get; set; }
        public bool SalaryCompositionTaxDeduction { get; set; }
        public string? SalaryCompositionQuota { get; set; }
        public bool SalaryCompositionAllowExceedQuota { get; set; }
        public string? SalaryCompositionValueType { get; set; }
        public string? SalaryCompositionValueCalculation { get; set; }
        public string? SalaryCompositionSumScope { get; set; }
        public string? SalaryCompositionOrgLevel { get; set; }
        public string? SalaryCompositionComponentToSum { get; set; }
        public string? SalaryCompositionValueFormula { get; set; }
        public string? SalaryCompositionDescription { get; set; }
        public int SalaryCompositionShowOnPayslip { get; set; }
        public int SalaryCompositionSource { get; set; }
        public int SalaryCompositionStatus { get; set; }
        public string? SalaryCompositionTaxablePart { get; set; }
        public string? SalaryCompositionTaxExemptPart { get; set; }
        public DateTime SalaryCompositionCreatedDate { get; set; }
        public DateTime SalaryCompositionModifiedDate { get; set; }
    }

    private class OrgMappingEntity
    {
        public string SalaryCompositionId { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
    }
}
