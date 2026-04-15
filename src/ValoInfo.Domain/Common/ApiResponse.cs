namespace ValoInfo.Domain.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data,
        StatusCode = 200
    };

    public static ApiResponse<T> NotFound(string message) => new()
    {
        Success = false,
        Data = default,
        Message = message,
        StatusCode = 404
    };

    public static ApiResponse<T> BadRequest(string message) => new()
    {
        Success = false,
        Data = default,
        Message = message,
        StatusCode = 400
    };

    public static ApiResponse<T> Error(string message) => new()
    {
        Success = false,
        Data = default,
        Message = message,
        StatusCode = 500
    };
}
