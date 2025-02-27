using System.Net;

namespace Miccore.Clean.Sample.Core.ApiModels
{
    /// <summary>
    /// Api response model
    /// </summary>
    public class ApiResponse<T> where T : class, new()
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public IEnumerable<ApiError>? Errors { get; set; }

        /// <summary>
        /// Creates a successful ApiResponse with the provided data.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <returns>An ApiResponse object with the data.</returns>
        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T>
            {
                Data = data
            };
        }

        /// <summary>
        /// Creates an error ApiResponse with a single error message.
        /// </summary>
        /// <param name="httpStatus">The HTTP status code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An ApiResponse object with the error message.</returns>
        public static ApiResponse<T> Error(HttpStatusCode httpStatus, string message)
        {
            return new ApiResponse<T>
            {
                Errors = new List<ApiError>
                {
                    new() {
                        Code = (int)httpStatus,
                        Message = message,
                    },
                },
            };
        }

        /// <summary>
        /// Creates an error ApiResponse with multiple error messages.
        /// </summary>
        /// <param name="httpStatus">The HTTP status code.</param>
        /// <param name="messages">The error messages.</param>
        /// <returns>An ApiResponse object with the error messages.</returns>
        public static ApiResponse<T> Error(HttpStatusCode httpStatus, IEnumerable<string> messages)
        {
            return new ApiResponse<T>
            {
                Errors = messages.Select(x => new ApiError
                {
                    Code = (int)httpStatus,
                    Message = x,
                }),
            };
        }

        /// <summary>
        /// Creates an error ApiResponse with multiple ApiError objects.
        /// </summary>
        /// <param name="httpStatus">The HTTP status code.</param>
        /// <param name="errors">The ApiError objects.</param>
        /// <returns>An ApiResponse object with the ApiError objects.</returns>
        public static ApiResponse<T> Error(IEnumerable<ApiError> errors)
        {
            return new ApiResponse<T>
            {
                Errors = errors,
            };
        }
    }
}