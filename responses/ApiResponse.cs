namespace JapaneseLearningApi.Responses;

public class ApiResponse<T>
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(int code, string message)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default
        };
    }
}

public class ApiResponse
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public static ApiResponse NoContent(string message = "Change successful")
    {
        return new ApiResponse
        {
            Code = 200,
            Message = message
        };
    }

    public static ApiResponse Fail(int code, string message)
    {
        return new ApiResponse
        {
            Code = code,
            Message = message
        };
    }
}