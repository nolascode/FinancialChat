using System.Net;
using FinancialChat.Core.DTOs.Common;
using FinancialChat.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace FinancialChat.API.Middlewares;

public static class ExceptionMiddlewareExtensions
{
    private const string ContentType = "application/json";

    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = ContentType;

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature is not null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        BadRequestException => StatusCodes.Status400BadRequest,
                        NotFoundException => StatusCodes.Status404NotFound,
                        ForbiddenException => StatusCodes.Status403Forbidden,
                        ConflictException => StatusCodes.Status409Conflict,
                        _ => StatusCodes.Status500InternalServerError,
                    };

                    await context.Response.WriteAsync(
                        new ErrorResponse
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                        }.ToString());
                }
            });
        });
    }
}
