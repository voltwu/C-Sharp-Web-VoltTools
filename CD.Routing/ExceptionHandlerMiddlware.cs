using CD.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CD.Common
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly String _errorfile;
        private readonly IDatabase _database;
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger,String errorfile, IDatabase idatabase )
        {
            _next = next;
            _logger = logger;
            _errorfile = errorfile;
            _database = idatabase;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                ErrorLog errorLog = new ErrorLog() {
                    occurtime = DateTime.UtcNow,
                    rawurl = httpContext.Features.Get<IHttpRequestFeature>().RawTarget,
                    url = httpContext.Request.GetEncodedUrl(),
                    message = e.Message.ToString(),
                    param = httpContext.Request.QueryString.ToString(),
                    body = httpContext.Request.Body.ToString(),
                    ip = httpContext.Connection.RemoteIpAddress.ToString(),
                    stackTrace = e.StackTrace.ToString()
                };
                ThreadPool.QueueUserWorkItem((obj)=> {
                    _database.InsertAErrorLog(errorLog);
                });
                
                httpContext.Response.Redirect(_errorfile);
            }
        }
    }
    public static class ExceptionHandlerMiddlewareExtension
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app, string errorfile)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>(errorfile);
        }
    }
}
