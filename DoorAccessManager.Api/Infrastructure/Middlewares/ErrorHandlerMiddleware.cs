using DoorAccessManager.Items.Exceptions;
using System.Net;
using System.Text.Json;

namespace DoorAccessManager.Api.Infrastructure.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                if (exception is BusinessException)
                {
                    _logger.LogError(exception.Message);
                }
                else
                {
                    _logger.LogError(exception, exception.Message);
                }

                await HandleErrorAsync(context, exception);
            }
        }

        private static Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            object response;

            if (exception is BusinessException applicationException)
            {
                context.Response.StatusCode = applicationException.StatusCode;

                response = new
                {
                    message = exception.Message
                };
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                response = new ErrorModel()
                {
                    Message = exception.Message,
                    Detail = exception.ToString()
                };
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
