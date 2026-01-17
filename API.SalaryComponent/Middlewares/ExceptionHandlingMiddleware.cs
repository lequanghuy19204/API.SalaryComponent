using System.Text.Json;
using Core.SalaryComponent.Exceptions;

namespace API.SalaryComponent.Middlewares;

/// <summary>
/// Middleware xử lý exception tập trung cho toàn bộ ứng dụng
/// Bắt tất cả exception và trả về response JSON thống nhất
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Khởi tạo middleware với request delegate và logger
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý request và bắt exception nếu có
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Xử lý exception và trả về response JSON
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case BaseException baseException:
                response.StatusCode = baseException.StatusCode;
                errorResponse.ErrorCode = baseException.ErrorCode;
                errorResponse.Message = baseException.Message;

                if (baseException is ValidationException validationException)
                {
                    errorResponse.Errors = validationException.Errors;
                }
                break;

            case InvalidOperationException:
                response.StatusCode = 400;
                errorResponse.ErrorCode = "INVALID_OPERATION";
                errorResponse.Message = exception.Message;
                break;

            default:
                _logger.LogError(exception, "Lỗi không xác định: {Message}", exception.Message);
                response.StatusCode = 500;
                errorResponse.ErrorCode = "INTERNAL_ERROR";
                errorResponse.Message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";
                break;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}

/// <summary>
/// Response trả về khi có lỗi
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Mã lỗi để frontend xử lý
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Thông báo lỗi hiển thị cho người dùng
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Chi tiết lỗi validation (nếu có)
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}

/// <summary>
/// Extension method để đăng ký middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Thêm Exception Handling Middleware vào pipeline
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
