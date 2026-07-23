namespace CRN.ProductAPI.Application.Comman
{
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the message describing the result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the operation.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="data">The data to return.</param>
        /// <param name="message">Optional message (default: "Success").</param>
        /// <param name="statusCode">Optional HTTP status code (default: 200).</param>
        /// <returns>A <see cref="Result{T}"/> indicating success.</returns>
        public static Result<T> Success(T data, string message = "Success", int statusCode = 200)
        {
            return new Result<T> { IsSuccess = true, StatusCode = statusCode, Message = message, Data = data };
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">Optional HTTP status code (default: 400).</param>
        /// <returns>A <see cref="Result{T}"/> indicating failure.</returns>
        public static Result<T> Failure(string message, int statusCode = 400)
        {
            return new Result<T> { IsSuccess = false, StatusCode = statusCode, Message = message };
        }
    }
}
