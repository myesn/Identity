using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Upo.Identity.Admin.ApiHost
{
    public class AutomaticExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger = Log.ForContext<AutomaticExceptionFilterAttribute>();

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            var code = (int)HttpStatusCode.BadRequest;
            if (exception is UnauthorizedAccessException)
                code = (int)HttpStatusCode.Unauthorized;

            var innerException = exception.InnerException?.Message;
            var stackTrace = exception.StackTrace;

            var errorResponse = new
            {
                //status = false,
                message = exception.Message,

                innerException,
                stackTrace
            };
            context.Result = new BadRequestObjectResult(errorResponse);
            context.HttpContext.Response.StatusCode = code;

            await LogRequestInfoAsync(context.HttpContext.Request, errorResponse).ConfigureAwait(false);
        }

        private async Task LogRequestInfoAsync(HttpRequest request, object error)
        {
            var requestPath = request.Path.ToString();
            var queryString = request.QueryString.ToString();
            var requestMethod = request.Method.ToLower();
            var isHttpGet = requestMethod == "get";
            var requestCookie = string.Join(';', request.Cookies?.Select(x => $"{x.Key}={x.Value}"));
            var requestBody = string.Empty;
            if (!isHttpGet)
            {
                if (request.Body != null && request.Body.CanRead)
                {
                    try
                    {
                        request.Body.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(request.Body))
                        {
                            requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                    catch { }
                }
            }

            var requestInfo = new
            {
                requestPath,
                queryString,
                requestMethod,
                requestCookie,
                requestBody
            };

            _logger.Error("捕捉到全局异常, 请求信息: {@requestInfo}, 返回信息：{@error}", requestInfo, error);
        }
    }
}
