namespace Core.SalaryComponent.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    public BaseException(string message, int statusCode = 500, string errorCode = "INTERNAL_ERROR")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public class NotFoundException : BaseException
{
    public NotFoundException(string message = "Không tìm thấy dữ liệu")
        : base(message, 404, "NOT_FOUND") { }

    public static NotFoundException WithEntity(string entityName, object id)
        => new($"Không tìm thấy {entityName} với ID: {id}");
}

public class DuplicateException : BaseException
{
    public DuplicateException(string message = "Dữ liệu đã tồn tại")
        : base(message, 409, "DUPLICATE") { }

    public static DuplicateException WithField(string fieldName, object value)
        => new($"{fieldName} '{value}' đã tồn tại");
}

public class ValidationException : BaseException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message = "Dữ liệu không hợp lệ")
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Dữ liệu không hợp lệ", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public static ValidationException WithField(string fieldName, string error)
        => new(new Dictionary<string, string[]> { { fieldName, new[] { error } } });
}

public class BusinessException : BaseException
{
    public BusinessException(string message)
        : base(message, 400, "BUSINESS_ERROR") { }
}

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = "Không có quyền truy cập")
        : base(message, 401, "UNAUTHORIZED") { }
}

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = "Không được phép thực hiện hành động này")
        : base(message, 403, "FORBIDDEN") { }
}

public class ConflictException : BaseException
{
    public ConflictException(string message = "Xung đột dữ liệu")
        : base(message, 409, "CONFLICT") { }
}

public class DatabaseException : BaseException
{
    public DatabaseException(string message = "Lỗi cơ sở dữ liệu")
        : base(message, 500, "DATABASE_ERROR") { }
}
