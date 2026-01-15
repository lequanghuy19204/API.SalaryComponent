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
            var showOnPayslip = ConvertShowOnPayslip(dto.ShowOnPayslip);
            var source = dto.Source == "system" ? 1 : 2;

            var sql = @"
                INSERT INTO pa_salary_composition 
                (id, composition_code, composition_name, composition_type, nature, tax_option, 
                 tax_deduction, quota, allow_exceed_quota, value_type, value_calculation, 
                 sum_scope, org_level, salary_component_to_sum, value_formula, description, 
                 show_on_payslip, source, status, created_date, modified_date)
                VALUES 
                (@Id, @CompositionCode, @CompositionName, @CompositionType, @Nature, @TaxOption, 
                 @TaxDeduction, @Quota, @AllowExceedQuota, @ValueType, @ValueCalculation,
                 @SumScope, @OrgLevel, @SalaryComponentToSum, @ValueFormula, @Description,
                 @ShowOnPayslip, @Source, 1, NOW(), NOW())";

            await connection.ExecuteAsync(sql, new
            {
                Id = id.ToString(),
                CompositionCode = dto.Code,
                CompositionName = dto.Name,
                CompositionType = dto.Type,
                Nature = dto.Property,
                dto.TaxOption,
                TaxDeduction = dto.DeductWhenCalculatingTax,
                dto.Quota,
                dto.AllowExceedQuota,
                dto.ValueType,
                dto.ValueCalculation,
                dto.SumScope,
                dto.OrgLevel,
                dto.SalaryComponentToSum,
                dto.ValueFormula,
                dto.Description,
                ShowOnPayslip = showOnPayslip,
                Source = source
            }, transaction);

            if (dto.UnitIds.Count > 0)
            {
                var orgSql = @"
                    INSERT INTO pa_salary_composition_organization 
                    (id, salary_composition_id, organization_id, created_date)
                    VALUES (@Id, @SalaryCompositionId, @OrganizationId, NOW())";

                foreach (var orgId in dto.UnitIds)
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
            SELECT id, composition_code, composition_name, composition_type, nature, tax_option,
                   tax_deduction, quota, allow_exceed_quota, value_type, value_calculation, 
                   sum_scope, org_level, salary_component_to_sum, value_formula, description, 
                   show_on_payslip, source, status, created_date, modified_date
            FROM pa_salary_composition 
            WHERE id = @Id";

        var result = await connection.QueryFirstOrDefaultAsync<SalaryCompositionEntity>(sql, new { Id = id.ToString() });
        if (result == null) return null;

        var dto = MapToDto(result);

        var orgSql = "SELECT organization_id FROM pa_salary_composition_organization WHERE salary_composition_id = @Id";
        var orgIds = await connection.QueryAsync<string>(orgSql, new { Id = id.ToString() });
        dto.UnitIds = orgIds.Select(Guid.Parse).ToList();

        return dto;
    }

    public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT id, composition_code, composition_name, composition_type, nature, tax_option,
                   tax_deduction, quota, allow_exceed_quota, value_type, value_calculation, 
                   sum_scope, org_level, salary_component_to_sum, value_formula, description, 
                   show_on_payslip, source, status, created_date, modified_date
            FROM pa_salary_composition 
            ORDER BY created_date DESC";

        var results = await connection.QueryAsync<SalaryCompositionEntity>(sql);
        var dtos = results.Select(MapToDto).ToList();

        var orgSql = "SELECT salary_composition_id, organization_id FROM pa_salary_composition_organization";
        var orgMappings = await connection.QueryAsync<OrgMappingEntity>(orgSql);
        var orgDict = orgMappings
            .GroupBy(x => x.SalaryCompositionId)
            .ToDictionary(g => g.Key, g => g.Select(x => Guid.Parse(x.OrganizationId)).ToList());

        foreach (var dto in dtos)
        {
            if (orgDict.TryGetValue(dto.Id.ToString(), out var unitIds))
            {
                dto.UnitIds = unitIds;
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
            var showOnPayslip = ConvertShowOnPayslip(dto.ShowOnPayslip);
            var source = dto.Source == "system" ? 1 : 2;

            var sql = @"
                UPDATE pa_salary_composition SET
                    composition_code = @CompositionCode,
                    composition_name = @CompositionName,
                    composition_type = @CompositionType,
                    nature = @Nature,
                    tax_option = @TaxOption,
                    tax_deduction = @TaxDeduction,
                    quota = @Quota,
                    allow_exceed_quota = @AllowExceedQuota,
                    value_type = @ValueType,
                    value_calculation = @ValueCalculation,
                    sum_scope = @SumScope,
                    org_level = @OrgLevel,
                    salary_component_to_sum = @SalaryComponentToSum,
                    value_formula = @ValueFormula,
                    description = @Description,
                    show_on_payslip = @ShowOnPayslip,
                    source = @Source,
                    modified_date = NOW()
                WHERE id = @Id";

            var affected = await connection.ExecuteAsync(sql, new
            {
                Id = id.ToString(),
                CompositionCode = dto.Code,
                CompositionName = dto.Name,
                CompositionType = dto.Type,
                Nature = dto.Property,
                dto.TaxOption,
                TaxDeduction = dto.DeductWhenCalculatingTax,
                dto.Quota,
                dto.AllowExceedQuota,
                dto.ValueType,
                dto.ValueCalculation,
                dto.SumScope,
                dto.OrgLevel,
                dto.SalaryComponentToSum,
                dto.ValueFormula,
                dto.Description,
                ShowOnPayslip = showOnPayslip,
                Source = source
            }, transaction);

            await connection.ExecuteAsync(
                "DELETE FROM pa_salary_composition_organization WHERE salary_composition_id = @Id",
                new { Id = id.ToString() }, transaction);

            if (dto.UnitIds.Count > 0)
            {
                var orgSql = @"
                    INSERT INTO pa_salary_composition_organization 
                    (id, salary_composition_id, organization_id, created_date)
                    VALUES (@Id, @SalaryCompositionId, @OrganizationId, NOW())";

                foreach (var orgId in dto.UnitIds)
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

            var sql = "DELETE FROM pa_salary_composition WHERE id = @Id";
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
            ? "SELECT COUNT(1) FROM pa_salary_composition WHERE composition_code = @Code AND id != @ExcludeId AND status = 1"
            : "SELECT COUNT(1) FROM pa_salary_composition WHERE composition_code = @Code AND status = 1";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code, ExcludeId = excludeId?.ToString() });
        return count > 0;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE pa_salary_composition SET status = @Status, modified_date = NOW() WHERE id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id.ToString(), Status = status });
        return affected > 0;
    }

    public async Task BulkUpdateStatusAsync(List<Guid> ids, int status)
    {
        if (ids.Count == 0) return;

        using var connection = CreateConnection();
        var idStrings = ids.Select(id => id.ToString()).ToList();
        var sql = "UPDATE pa_salary_composition SET status = @Status, modified_date = NOW() WHERE id IN @Ids";
        await connection.ExecuteAsync(sql, new { Ids = idStrings, Status = status });
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
            Id = Guid.Parse(entity.Id),
            Code = entity.CompositionCode,
            Name = entity.CompositionName,
            Type = entity.CompositionType,
            Property = entity.Nature,
            TaxOption = entity.TaxOption ?? "taxable",
            DeductWhenCalculatingTax = entity.TaxDeduction,
            Quota = entity.Quota,
            AllowExceedQuota = entity.AllowExceedQuota,
            ValueType = entity.ValueType ?? "currency",
            ValueCalculation = entity.ValueCalculation ?? "formula",
            SumScope = entity.SumScope,
            OrgLevel = entity.OrgLevel,
            SalaryComponentToSum = entity.SalaryComponentToSum,
            ValueFormula = entity.ValueFormula,
            Description = entity.Description,
            ShowOnPayslip = ConvertShowOnPayslipToString(entity.ShowOnPayslip),
            Source = ConvertSourceToString(entity.Source),
            Status = entity.Status,
            CreatedDate = entity.CreatedDate,
            ModifiedDate = entity.ModifiedDate
        };
    }

    private class SalaryCompositionEntity
    {
        public string Id { get; set; } = string.Empty;
        public string CompositionCode { get; set; } = string.Empty;
        public string CompositionName { get; set; } = string.Empty;
        public string CompositionType { get; set; } = string.Empty;
        public string Nature { get; set; } = string.Empty;
        public string? TaxOption { get; set; }
        public bool TaxDeduction { get; set; }
        public string? Quota { get; set; }
        public bool AllowExceedQuota { get; set; }
        public string? ValueType { get; set; }
        public string? ValueCalculation { get; set; }
        public string? SumScope { get; set; }
        public string? OrgLevel { get; set; }
        public string? SalaryComponentToSum { get; set; }
        public string? ValueFormula { get; set; }
        public string? Description { get; set; }
        public int ShowOnPayslip { get; set; }
        public int Source { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    private class OrgMappingEntity
    {
        public string SalaryCompositionId { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
    }
}
