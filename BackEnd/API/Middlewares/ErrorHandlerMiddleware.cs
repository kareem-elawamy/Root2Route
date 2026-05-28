using Core.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middlewares
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
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case UnauthorizedAccessException e:
                        // Custom application error
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel.StatusCode = HttpStatusCode.Unauthorized;
                        break;
                    case KeyNotFoundException e:
                        // Not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.StatusCode = HttpStatusCode.NotFound;
                        break;
                    case InvalidOperationException e:
                        // Handled validation or logic error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.StatusCode = HttpStatusCode.BadRequest;
                        break;
                    default:
                        // Unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.StatusCode = HttpStatusCode.InternalServerError;
                        _logger.LogError(error, error?.Message);
                        break;
                }

                var result = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await response.WriteAsync(result);
            }
        }
    }
}
