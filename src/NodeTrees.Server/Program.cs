using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using NLog;
using System.Text.Json;

namespace NodeTrees.Server
{
    using NodeTrees.Authentication;
    using NodeTrees.BusinessLogic.Repository;
    using NodeTrees.BusinessLogic.Services;
    using NodeTrees.DataAccess.Repository;
    using NodeTrees.Server.Middlewares;
    using NodeTrees.WebApi.User;

    public class Program
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {

            try
            {
                await RunApp(args);
            }
            catch (HostAbortedException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _log.Fatal(ex, "Application terminated unexpectedly");
            }
        }

        private static async Task RunApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            // необходимо, чтобы легаси фильтр увидел IConfiguration
            AppDomain.CurrentDomain.SetData("HostBuilderContextConfiguration",
                builder.Configuration);

            builder.ConfigureDatabase();
            builder.ConfigureNLog();
            builder.ConfigureUserContext();

            var controllersBuilder = builder.Services.AddControllers();
            controllersBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(JournalController).Assembly));
            controllersBuilder
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                })
                .AddMvcOptions(options =>
                {
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<ITreeService, TreeService>();

            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

            builder.Services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

            builder.ConfigureMapping();
            builder.ConfigureJwtAuthentication();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                // ignore
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<CurrentUserContextMiddleware>();
            app.UseAuthorization();

            await app.MigrateDatabaseAsync();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/{documentName}/swagger.json";
            });

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint($"/api/{description.GroupName}/swagger.json", $"NodeServer API {description.GroupName}");
                    opt.RoutePrefix = $"api/{description.GroupName.ToLower()}";
                    _log.Info("Hosted swagger at {RoutePrefix}", opt.RoutePrefix);
                });
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("Server", "NodeServer");
                await next.Invoke();
            });

            await app.RunAsync();
        }
    }
}
