using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaveWise.DataLayer.Sys.Exceptions;

namespace SaveWise.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        switch (contextFeature.Error)
                        {
                            case UnauthorizedAccessException _:
                                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                return;
                            case ArgumentException _:
                                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                                return;
                            case DocumentNotFoundException _:
                                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                                return;
                            default:
                                await context.Response.WriteAsync(new ErrorResult("Internal Server Error.").ToString());
                                break;
                        }
                    }
                });
            });
        }
    }
}