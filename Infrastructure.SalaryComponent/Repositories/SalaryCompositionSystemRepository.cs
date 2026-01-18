using Dapper;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using Core.SalaryComponent.DTOs;
using Core.SalaryComponent.Interfaces.IRepository;

namespace Infrastructure.SalaryComponent.Repositories;

/// <summary>
/// Repository truy xuất dữ liệu thành phần lương hệ thống từ database
/// </summary>
public class SalaryCompositionSystemRepository : ISalaryCompositionSystemRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// Khởi tạo repository với chuỗi kết nối
    /// </summary>
    /// <param name="configuration">Cấu hình ứng dụng chứa chuỗi kết nối</param>
    public SalaryCompositionSystemRepository(IConfiguration configuration)
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
    /// Lấy tất cả thành phần lương hệ thống
    /// </summary>
    /// <returns>Danh sách tất cả thành phần lương hệ thống</returns>
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

    /// <summary>
    /// Lấy thành phần lương hệ thống theo ID
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống</param>
    /// <returns>DTO thành phần lương hệ thống hoặc null nếu không tìm thấy</returns>
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

    /// <summary>
    /// Xóa thành phần lương hệ thống
    /// </summary>
    /// <param name="id">ID thành phần lương hệ thống cần xóa</param>
    /// <returns>True nếu xóa thành công</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM pa_salary_composition_system WHERE salary_composition_system_id = @Id";
        var affected = await connection.ExecuteAsync(sql, new { Id = id.ToString() });
        return affected > 0;
    }

    /// <summary>
    /// Lấy ID đơn vị gốc (parent_id = NULL)
    /// </summary>
    /// <returns>ID đơn vị gốc hoặc null nếu không tìm thấy</returns>
    public async Task<Guid?> GetRootOrganizationIdAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT organization_id FROM pa_organization WHERE parent_id IS NULL LIMIT 1";
        var result = await connection.QueryFirstOrDefaultAsync<string>(sql);
        return result == null ? null : Guid.Parse(result);
    }

    /// <summary>
    /// Kiểm tra mã đã tồn tại trong pa_salary_composition
    /// </summary>
    /// <param name="code">Mã cần kiểm tra</param>
    /// <returns>True nếu mã đã tồn tại</returns>
    public async Task<bool> IsCodeExistsInCompositionAsync(string code)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM pa_salary_composition WHERE salary_composition_code = @Code";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Code = code });
        return count > 0;
    }

    /// <summary>
    /// Lấy ID thành phần lương theo mã
    /// </summary>
    /// <param name="code">Mã thành phần lương</param>
    /// <returns>ID thành phần lương hoặc null nếu không tìm thấy</returns>
    public async Task<Guid?> GetCompositionIdByCodeAsync(string code)
    {
        using var connection = CreateConnection();
        var sql = "SELECT salary_composition_id FROM pa_salary_composition WHERE salary_composition_code = @Code LIMIT 1";
        var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Code = code });
        return result == null ? null : Guid.Parse(result);
    }

    /// <summary>
    /// Lấy danh sách có phân trang với bộ lọc nâng cao
    /// </summary>
    /// <param name="request">DTO chứa thông tin phân trang và bộ lọc</param>
    /// <returns>Kết quả phân trang thành phần lương hệ thống</returns>
    public async Task<PagedResultDto<SalaryCompositionSystemDto>> GetPagedAsync(SystemPagingRequestDto request)
    {
        using var connection = CreateConnection();

        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            whereClauses.Add("(salary_composition_system_code LIKE @SearchText OR salary_composition_system_name LIKE @SearchText)");
            parameters.Add("SearchText", $"%{request.SearchText}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            whereClauses.Add("salary_composition_system_type = @Type");
            parameters.Add("Type", request.Type);
        }

        // Helper method để build SQL condition dựa trên FilterConditionDto
        void AddFilterCondition(FilterConditionDto? filter, string columnName, string paramName)
        {
            if (filter == null) return;
            
            switch (filter.Condition)
            {
                case "contains":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"{columnName} LIKE @{paramName}");
                        parameters.Add(paramName, $"%{filter.Value}%");
                    }
                    break;
                case "notContains":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"({columnName} NOT LIKE @{paramName} OR {columnName} IS NULL)");
                        parameters.Add(paramName, $"%{filter.Value}%");
                    }
                    break;
                case "equals":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"{columnName} = @{paramName}");
                        parameters.Add(paramName, filter.Value);
                    }
                    break;
                case "notEquals":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"({columnName} != @{paramName} OR {columnName} IS NULL)");
                        parameters.Add(paramName, filter.Value);
                    }
                    break;
                case "startsWith":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"{columnName} LIKE @{paramName}");
                        parameters.Add(paramName, $"{filter.Value}%");
                    }
                    break;
                case "endsWith":
                    if (!string.IsNullOrWhiteSpace(filter.Value))
                    {
                        whereClauses.Add($"{columnName} LIKE @{paramName}");
                        parameters.Add(paramName, $"%{filter.Value}");
                    }
                    break;
                case "empty":
                    whereClauses.Add($"({columnName} IS NULL OR {columnName} = '')");
                    break;
                case "notEmpty":
                    whereClauses.Add($"({columnName} IS NOT NULL AND {columnName} != '')");
                    break;
            }
        }

        // Apply advanced filters with conditions
        AddFilterCondition(request.SalaryCompositionSystemCodeFilter, "salary_composition_system_code", "SalaryCompositionSystemCode");
        AddFilterCondition(request.SalaryCompositionSystemNameFilter, "salary_composition_system_name", "SalaryCompositionSystemName");
        AddFilterCondition(request.SalaryCompositionTypeFilter, "salary_composition_system_type", "SalaryCompositionSystemType");
        AddFilterCondition(request.SalaryCompositionNatureFilter, "salary_composition_system_nature", "SalaryCompositionSystemNature");
        AddFilterCondition(request.QuotaFilter, "salary_composition_system_quota", "Quota");
        AddFilterCondition(request.ValueTypeFilter, "salary_composition_system_value_type", "ValueType");
        AddFilterCondition(request.ValueFilter, "salary_composition_system_value_formula", "Value");
        AddFilterCondition(request.DescriptionFilter, "salary_composition_system_description", "Description");

        // Filter by taxable với condition (IS NOT NULL / IS NULL)
        if (request.IsTaxableFilter != null)
        {
            switch (request.IsTaxableFilter.Condition)
            {
                case "equals":
                    if (request.IsTaxableFilter.Value == "true")
                        whereClauses.Add("salary_composition_system_tax_option IS NOT NULL");
                    else if (request.IsTaxableFilter.Value == "false")
                        whereClauses.Add("salary_composition_system_tax_option IS NULL");
                    break;
                case "notEquals":
                    if (request.IsTaxableFilter.Value == "true")
                        whereClauses.Add("salary_composition_system_tax_option IS NULL");
                    else if (request.IsTaxableFilter.Value == "false")
                        whereClauses.Add("salary_composition_system_tax_option IS NOT NULL");
                    break;
            }
        }

        // Filter by tax deductible với condition (1/0)
        if (request.IsTaxDeductibleFilter != null)
        {
            switch (request.IsTaxDeductibleFilter.Condition)
            {
                case "equals":
                    if (request.IsTaxDeductibleFilter.Value == "true")
                        whereClauses.Add("salary_composition_system_tax_deduction = 1");
                    else if (request.IsTaxDeductibleFilter.Value == "false")
                        whereClauses.Add("salary_composition_system_tax_deduction = 0");
                    break;
                case "notEquals":
                    if (request.IsTaxDeductibleFilter.Value == "true")
                        whereClauses.Add("salary_composition_system_tax_deduction = 0");
                    else if (request.IsTaxDeductibleFilter.Value == "false")
                        whereClauses.Add("salary_composition_system_tax_deduction = 1");
                    break;
            }
        }

        // Filter by show on payslip với condition (yes=1, no=2, if_not_zero=3)
        if (request.ShowOnPayslipFilter != null && !string.IsNullOrWhiteSpace(request.ShowOnPayslipFilter.Value))
        {
            int showOnPayslipValue = request.ShowOnPayslipFilter.Value switch
            {
                "yes" => 1,
                "no" => 2,
                "if_not_zero" => 3,
                _ => 0
            };
            if (showOnPayslipValue > 0)
            {
                switch (request.ShowOnPayslipFilter.Condition)
                {
                    case "equals":
                        whereClauses.Add("salary_composition_system_show_on_payslip = @ShowOnPayslip");
                        parameters.Add("ShowOnPayslip", showOnPayslipValue);
                        break;
                    case "notEquals":
                        whereClauses.Add("salary_composition_system_show_on_payslip != @ShowOnPayslip");
                        parameters.Add("ShowOnPayslip", showOnPayslipValue);
                        break;
                }
            }
        }

        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        var countSql = $"SELECT COUNT(1) FROM pa_salary_composition_system {whereClause}";
        var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var offset = (request.PageNumber - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

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
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Chuyển đổi giá trị hiển thị phiếu lương
    /// </summary>
    /// <param name="value">Giá trị số</param>
    /// <returns>Giá trị chuỗi tương ứng</returns>
    private static string ConvertShowOnPayslipToString(int value) => value switch
    {
        1 => "yes",
        2 => "no",
        3 => "if_not_zero",
        _ => "yes"
    };

    /// <summary>
    /// Chuyển đổi entity sang DTO
    /// </summary>
    /// <param name="entity">Entity cần chuyển đổi</param>
    /// <returns>DTO thành phần lương hệ thống</returns>
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

    /// <summary>
    /// Entity ánh xạ với bảng pa_salary_composition_system
    /// </summary>
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
