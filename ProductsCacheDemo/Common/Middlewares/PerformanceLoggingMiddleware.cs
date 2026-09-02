using System.Diagnostics;

namespace ProductsCacheDemo.Common.Middlewares
{
    public class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public PerformanceLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var method = context.Request.Method;
            var path = context.Request.Path;

            Console.WriteLine($"===> [PERFORMANCE] {method} {path} completed in {elapsedMs} ms");
        }
    }
}
