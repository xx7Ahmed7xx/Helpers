using Microsoft.AspNetCore.Builder;

namespace AAM.Helpers.Backend
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        /// <summary>
        /// Custom global exception handling, with enhanced error handling by MrX7. (Shows inner exception) <br></br>
        /// <a href="https://www.github.com/xx7Ahmed7xx"></a>
        /// </summary>
        /// <param name="app">The object representing current AppBuilder.</param>
        public static void UseExceptionHandlerInnerMrX7(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddlewareInner>();
        }

        /// <summary>
        /// Custom global exception handling, with enhanced error handling by MrX7. (Doesn't show inner exception)<br></br>
        /// <a href="https://www.github.com/xx7Ahmed7xx"></a>
        /// </summary>
        /// <param name="app">The object representing current AppBuilder.</param>
        public static void UseExceptionHandlerMrX7(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
