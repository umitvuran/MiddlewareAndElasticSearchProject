namespace MiddlewareAndElasticSearchProject.Middlewares
{
    public class BadWordsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public BadWordsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.Items.TryGetValue("jsonBody", out object? jsonBody))
            {
                var badwords = new List<string> { "pis", "kaka", "kötü", "deli" };
                var jsonBodyString = (string?)jsonBody;
                if (badwords.Any(jsonBodyString.Contains))
                {
                    await ResponseBadRequest(context);
                    return;
                }
            }
            await _next(context);
        }
        private static async Task ResponseBadRequest(HttpContext context)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "Bu gönderide hoş olmayan kelimeler var!" });
        }


    }
}
