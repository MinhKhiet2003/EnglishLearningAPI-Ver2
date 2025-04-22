using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Response
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        // Static Success Method
        public static ApiResponse<T> Success(int statusCode, string message, T data)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }

        // Static Error Method
        public static ApiResponse<object> error(int statusCode, string message, string error)
        {
            return new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = message,
                Data = null,
                Error = error
            };
        }
    }
}
