namespace nutrition_app_backend.DTOs;

using System.Text.Json.Serialization;

public class ApiResponse<T>
{
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Success", string code = "200")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Code = code,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message, string code = "400", T? data = default)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Code = code,
            Message = message,
            Data = data
        };
    }
}