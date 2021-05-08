using DailyStudent.Api.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace DailyStudent.Api.Extensions
{
    public static class ExceptionsMiddleware
    {
        public static void UseExceptionsMiddleware(this IApplicationBuilder app)
        {
            app.Use(WriteExceptionAsync);
        }

        private async static Task WriteExceptionAsync(HttpContext context, Func<Task> next)
        {
            var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionDetails?.Error;

            if (exception != null)
            {
                context.Response.ContentType = "application/json";

                dynamic response = new { };
                if (exception is MessageException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        errors = exception.Message
                    };
                }
                else  if (exception is Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        errors = exception.Message
                    };
                }

                // Others custom exception

                var stream = context.Response.Body;
                await JsonSerializer.SerializeAsync(stream, response);
            }

        }
    }
}
