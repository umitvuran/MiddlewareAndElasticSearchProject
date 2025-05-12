using System.Text;

namespace MiddlewareAndElasticSearchProject.Middlewares
{
    public class JsonBodyMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsAvailableForFilter(context))
            {
                var jsonRequestBody = await GetJsonRequestBody(context);
                if (!string.IsNullOrEmpty(jsonRequestBody))
                {
                    ReplaceRequestBodyWithJsonStream(context, jsonRequestBody);
                    SaveJsonToContextItem(context, jsonRequestBody);
                }
            }
            await _next(context);
        }

        private bool IsAvailableForFilter(HttpContext context)
        {
            return (IsPostRequest(context) || IsPutRequest(context))
                   && IsJsonRequest(context);
        }

        private async Task<string> GetJsonRequestBody(HttpContext context)
        {
            using var reader = new StreamReader(context.Request.Body);
            return await reader.ReadToEndAsync();
        }

        private void SaveJsonToContextItem(HttpContext context, string jsonRequestBody)
        {
            context.Items["jsonBody"] = jsonRequestBody;
        }

        private void ReplaceRequestBodyWithJsonStream(HttpContext context, string requestBody)
        {
            var content = Encoding.UTF8.GetBytes(requestBody);
            var requestBodyStream = new MemoryStream();
            requestBodyStream.Write(content, 0, content.Length);
            context.Request.Body = requestBodyStream;
            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }

        private bool IsPostRequest(HttpContext context)
        {
            return context.Request.Method == HttpMethods.POST;
        }
        private bool IsPutRequest(HttpContext context)
        {
            return context.Request.Method == HttpMethods.PUT;
        }
        private bool IsJsonRequest(HttpContext context)
        {
            return context.Request.ContentType.StartsWith("application/json");
        }

        class HttpMethods
        {
            public const string POST = "POST";
            public const string PUT = "PUT";
        }
    }
}
