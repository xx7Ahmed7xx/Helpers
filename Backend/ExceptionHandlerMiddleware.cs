using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace AAM.Helpers.Backend
{
    /// <summary>
    /// Global Exception Handling middleware Filter to show inner errors without confusing other middleware pipelines, by MrX7.
    /// </summary>
    public class ExceptionHandlerMiddleware    
    {    
        private readonly RequestDelegate _next;    
    
        /// <summary>
        /// Creates a new instance of the EHM class.
        /// </summary>
        /// <param name="next">The request Delegate next's object in the pipeline.</param>
        public ExceptionHandlerMiddleware(RequestDelegate next)    
        {    
            _next = next;    
        }

        /// <summary>
        /// Begins invoking the middleware.
        /// </summary>
        /// <param name="context">The http context which will be used to handle errors, if found.</param>
        /// <returns>Successfully moving to the next pipeline, or attempts to being exception handling operations.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(result);
        }
    }
}
