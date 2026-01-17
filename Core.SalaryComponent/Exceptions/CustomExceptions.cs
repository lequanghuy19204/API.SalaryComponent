namespace Core.SalaryComponent.Exceptions;

/// <summary>
/// Lớp ngoại lệ cơ sở cho ứng dụng.
/// Tất cả các ngoại lệ tùy chỉnh khác đều kế thừa từ lớp này.
/// </summary>
public class BaseException : Exception
{
    /// <summary>
    /// Mã trạng thái HTTP.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Mã lỗi để xác định loại lỗi.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="BaseException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi.</param>
    /// <param name="statusCode">Mã trạng thái HTTP (mặc định: 500).</param>
    /// <param name="errorCode">Mã lỗi (mặc định: INTERNAL_ERROR).</param>
    public BaseException(string message, int statusCode = 500, string errorCode = "INTERNAL_ERROR")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Ngoại lệ khi không tìm thấy dữ liệu.
/// Mã trạng thái HTTP: 404.
/// </summary>
public class NotFoundException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="NotFoundException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Không tìm thấy dữ liệu").</param>
    public NotFoundException(string message = "Không tìm thấy dữ liệu")
        : base(message, 404, "NOT_FOUND") { }

    /// <summary>
    /// Tạo ngoại lệ với thông tin thực thể và ID cụ thể.
    /// </summary>
    /// <param name="entityName">Tên thực thể.</param>
    /// <param name="id">ID của thực thể.</param>
    /// <returns>Instance của <see cref="NotFoundException"/>.</returns>
    public static NotFoundException WithEntity(string entityName, object id)
        => new($"Không tìm thấy {entityName} với ID: {id}");
}

/// <summary>
/// Ngoại lệ khi dữ liệu bị trùng lặp.
/// Mã trạng thái HTTP: 409.
/// </summary>
public class DuplicateException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="DuplicateException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Dữ liệu đã tồn tại").</param>
    public DuplicateException(string message = "Dữ liệu đã tồn tại")
        : base(message, 409, "DUPLICATE") { }

    /// <summary>
    /// Tạo ngoại lệ với thông tin trường và giá trị bị trùng.
    /// </summary>
    /// <param name="fieldName">Tên trường bị trùng.</param>
    /// <param name="value">Giá trị bị trùng.</param>
    /// <returns>Instance của <see cref="DuplicateException"/>.</returns>
    public static DuplicateException WithField(string fieldName, object value)
        => new($"{fieldName} '{value}' đã tồn tại");
}

/// <summary>
/// Ngoại lệ khi dữ liệu không hợp lệ.
/// Mã trạng thái HTTP: 400.
/// </summary>
public class ValidationException : BaseException
{
    /// <summary>
    /// Dictionary chứa các lỗi validation theo từng trường.
    /// </summary>
    public Dictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ValidationException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Dữ liệu không hợp lệ").</param>
    public ValidationException(string message = "Dữ liệu không hợp lệ")
        : base(message, 400, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ValidationException"/> với danh sách lỗi.
    /// </summary>
    /// <param name="errors">Dictionary chứa các lỗi validation.</param>
    public ValidationException(Dictionary<string, string[]> errors)
        : base("Dữ liệu không hợp lệ", 400, "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    /// <summary>
    /// Tạo ngoại lệ với lỗi cho một trường cụ thể.
    /// </summary>
    /// <param name="fieldName">Tên trường bị lỗi.</param>
    /// <param name="error">Thông báo lỗi.</param>
    /// <returns>Instance của <see cref="ValidationException"/>.</returns>
    public static ValidationException WithField(string fieldName, string error)
        => new(new Dictionary<string, string[]> { { fieldName, new[] { error } } });
}

/// <summary>
/// Ngoại lệ nghiệp vụ.
/// Sử dụng khi vi phạm các quy tắc nghiệp vụ của ứng dụng.
/// Mã trạng thái HTTP: 400.
/// </summary>
public class BusinessException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="BusinessException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi nghiệp vụ.</param>
    public BusinessException(string message)
        : base(message, 400, "BUSINESS_ERROR") { }
}

/// <summary>
/// Ngoại lệ khi không có quyền truy cập.
/// Sử dụng khi người dùng chưa xác thực hoặc token không hợp lệ.
/// Mã trạng thái HTTP: 401.
/// </summary>
public class UnauthorizedException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="UnauthorizedException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Không có quyền truy cập").</param>
    public UnauthorizedException(string message = "Không có quyền truy cập")
        : base(message, 401, "UNAUTHORIZED") { }
}

/// <summary>
/// Ngoại lệ khi bị cấm thực hiện hành động.
/// Sử dụng khi người dùng đã xác thực nhưng không có quyền thực hiện hành động.
/// Mã trạng thái HTTP: 403.
/// </summary>
public class ForbiddenException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ForbiddenException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Không được phép thực hiện hành động này").</param>
    public ForbiddenException(string message = "Không được phép thực hiện hành động này")
        : base(message, 403, "FORBIDDEN") { }
}

/// <summary>
/// Ngoại lệ khi xảy ra xung đột dữ liệu.
/// Sử dụng khi có xung đột về trạng thái dữ liệu (ví dụ: cập nhật đồng thời).
/// Mã trạng thái HTTP: 409.
/// </summary>
public class ConflictException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="ConflictException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Xung đột dữ liệu").</param>
    public ConflictException(string message = "Xung đột dữ liệu")
        : base(message, 409, "CONFLICT") { }
}

/// <summary>
/// Ngoại lệ liên quan đến cơ sở dữ liệu.
/// Sử dụng khi xảy ra lỗi kết nối, truy vấn hoặc thao tác với database.
/// Mã trạng thái HTTP: 500.
/// </summary>
public class DatabaseException : BaseException
{
    /// <summary>
    /// Khởi tạo một instance mới của <see cref="DatabaseException"/>.
    /// </summary>
    /// <param name="message">Thông báo lỗi (mặc định: "Lỗi cơ sở dữ liệu").</param>
    public DatabaseException(string message = "Lỗi cơ sở dữ liệu")
        : base(message, 500, "DATABASE_ERROR") { }
}
