using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using MiddlewareAndElasticSearchProject.Extensions;
using MiddlewareAndElasticSearchProject.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Elastic.CommonSchema.Serilog;

namespace MiddlewareAndElasticSearchProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //namespace'i Microsoft.AspNetCore olanlar için min level
                .MinimumLevel.Override("System", LogEventLevel.Warning) //namespace'i System olanlar için min level
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Console(new JsonFormatter(renderMessage: true))
                .WriteTo.Debug(new JsonFormatter(renderMessage: true))
                .WriteTo.File("Log/application.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] (ThreadId:{ThreadId} [{SourceContext}]) {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Warning
                    )
                .WriteTo.Elasticsearch(new[] { new Uri("http://localhost:9200") }, opts =>
                {
                    opts.DataStream = new DataStreamName("Second", "Elastic", typeof(Program).Namespace);
                    opts.BootstrapMethod = BootstrapMethod.Failure;
                    opts.TextFormatting = new EcsTextFormatterConfiguration()
                    {
                        IncludeProcess=false,
                        IncludeUser = false,
                        IncludeHost=false,
                        
                    };
                })
                .CreateLogger();
            

            /*
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
            */

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSerilog(logger);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseMiddleware<JsonBodyMiddleware>();
            //app.UseMiddleware<BadWordsHandlerMiddleware>();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    // Bu þekilde sadece istediklerini ekle
                    diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                };
            });

            app.MapControllers();

            app.Run();
        }
    }
}
