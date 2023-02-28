using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SerialConsole.Models;
using SerialConsole.Services;
using Serilog;

namespace SerialConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("App started - version 1.0.2");
            CreateHostBuilder(args).Build().Run();
            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    configuration
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile("appsettings.{Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\")}.json", true, true);

                    configuration.AddEnvironmentVariables();

                    if (args is { Length: > 0 })
                    {
                        configuration.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Set working directory if in production
                    if (!hostContext.HostingEnvironment.IsDevelopment())
                    {
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    }

                    var configuration = hostContext.Configuration;
                    services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();

                    Log.Information($"Environment: {hostContext.HostingEnvironment.EnvironmentName}");

                    services.AddSingleton(configuration);
                    services.AddHostedService<SerialWorker>();
                    services.AddSingleton<IConsoleSpinner, ConsoleSpinner>();
                })
                .UseSerilog();
        }
    }
}