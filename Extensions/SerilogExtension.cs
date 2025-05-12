using Serilog;
using System.Reflection;

namespace MiddlewareAndElasticSearchProject.Extensions
{
    public static class SerilogExtension
    {

        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            if (environment == null)
                return null;


            var applicationName = Assembly.GetExecutingAssembly().GetName().Name;
            var appplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;

            /*
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.
                */


            //builder.Host.UseSerilog();

            return builder;
        }
    }
}
