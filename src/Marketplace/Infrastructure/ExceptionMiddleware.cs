using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Marketplace.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure
{
    public class ExceptionMiddleware
    {
        static readonly Serilog.ILogger Log = Serilog.Log.ForContext<ExceptionMiddleware>();

        readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                await HandleException(ex);
            }

            Task HandleException(Exception ex)
            {
                Log.Error(ex, "Something went wrong: {message}", ex.Message);

                var exceptionType = ex.GetType();
                var errorKey = exceptionType.Name.Replace("Exception", "");

                switch (ex)
                {
                    case WrongExpectedStreamVersionException _:
                        ctx.Response.StatusCode = (int) HttpStatusCode.Conflict;
                        break;

                    case EntityNotFoundException _:
                        ctx.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        break;

                    case DomainException _:
                        ctx.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        break;

                    default:
                        ctx.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        break;
                }

                ctx.Response.ContentType = "application/json";

                var errorDetails = JsonConvert.SerializeObject(
                    new ErrorDetails { Key = errorKey, Message = ex.Message });

                return ctx.Response.WriteAsync(errorDetails, Encoding.UTF8);
            }
        }

        public class ErrorDetails
        {
            public string Key { get; set; }
            public string Message { get; set; }
        }
    }

    public static class ExceptionMiddlewareAppExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof (app));

            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
