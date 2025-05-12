using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace MiddlewareAndElasticSearchProject.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            DateTime requestTime = DateTime.Now;
            var requestBody = await ReadBodyFromRequest(context.Request);
            var httpMethod = context.Request.Method;
            var queryStrings = context.Request.QueryString.Value;
            var headers = FormatHeaders(context.Request.Headers);
            var requestPath = context.Request.Path.ToString();

            var originalResponseBody = context.Response.Body;
            using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            await _next.Invoke(context);
            newResponseBody.Seek(0, SeekOrigin.Begin);

            DateTime responseTime = DateTime.Now;
            var latency = (responseTime - requestTime).TotalMilliseconds;
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var statusCode = context.Response.StatusCode;


            if (statusCode == 200)
            {
                _logger.LogInformation("Source Context: {SourceContext}" +
                     "Http Method: {HttpMethod}" +
                     "Query Strings: {QueryStrings}" +
                     "Request Path: {RequestPath}" +
                     "Request Time: {RequestTime}" +
                     "Headers: {Headers}" +
                     "Request Body: {RequestBody}" +
                     "Response Status Code: {StatusCode}" +
                     "Latency: {Latency}" +
                     "Response Time: {ResponseTime}" +
                     "Response Body: {ResponseBody}" +
                     "Mahmut: {Mahmut}",
                     nameof(RequestResponseLoggingMiddleware),
                     httpMethod,
                     queryStrings,
                     requestPath,
                     requestTime,
                     FormatHeaders(context.Request.Headers),
                     requestBody,
                     statusCode,
                     latency,
                     responseTime,
                     responseBodyText,
                     "Mahmuat");
            }
            else
            {
                _logger.LogError("Source Context: {SourceContext}" +
                     "Http Method: {HttpMethod}" +
                     "Query Strings: {QueryStrings}" +
                     "Request Path: {RequestPath}" +
                     "Request Time: {RequestTime}" +
                     "Headers: {Headers}" +
                     "Request Body: {RequestBody}" +
                     "Response Status Code: {StatusCode}" +
                     "Latency: {Latency}" +
                     "Response Time: {ResponseTime}" +
                     "Response Body: {ResponseBody}",
                     nameof(RequestResponseLoggingMiddleware),
                     httpMethod,
                     queryStrings,
                     requestPath,
                     requestTime,
                     FormatHeaders(context.Request.Headers),
                     requestBody,
                     statusCode,
                     latency,
                     responseTime,
                     responseBodyText);
            }


            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);

        }

        private static string FormatHeaders(IHeaderDictionary headers) => string.Join(", ", headers.Select(kvp => $"{{{kvp.Key}: {string.Join(", ", kvp.Value)}}}"));

        private static async Task<string> ReadBodyFromRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var reader = new StreamReader(request.Body);
            var requestBody = await reader.ReadToEndAsync();

            request.Body.Position = 0;
            return requestBody;
        }
    }
}
