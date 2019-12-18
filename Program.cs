using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using DomoticzToRouterSmsBot.Proccessor;
using DomoticzToRouterSmsBot.Proccessor.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Polly;

namespace DomoticzToRouterSmsBot
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile("lastProccessedSMS.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                    services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));
                    services.AddScoped<ISmsParser, SmsParser>()
                    .AddScoped<ISmsRunner, SmsRunner>()
                    .AddScoped<ISmsUpdater, TpLinkRouter>()
                    .AddScoped<ToggleSwitch>()
                    .AddScoped<MarkAsRead>()
                    .AddScoped<ICommand>(provider =>
                    {
                        var c1 = provider.GetService<ToggleSwitch>();
                        ((Command)c1).Use(provider.GetService<MarkAsRead>());
                        return c1;
                    });
                    if (String.IsNullOrEmpty(hostingContext.Configuration["DataFilePath"]))
                    {
                        services.AddScoped<IDomoticz, Domoticz>();
                        services.AddScoped<ISmsLoader, TpLinkRouter>();
                        services.AddHttpClient("router").AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));
                        services.AddHttpClient("domoticz").AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));
                    }
                    else
                    {
                        services.AddScoped<IDomoticz, Domoticz>(); 
                        services.AddHttpClient("domoticz").AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));
                        services.AddScoped<ISmsLoader, File>();
                    }
                    services.AddHostedService<WorkerService>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
                    NLog.LogManager.LoadConfiguration("nlog.config");
                });
            await builder.RunConsoleAsync();
        }
    }

    public class Proccessed
    {
        public DateTime LastProccessedTime { get; set; }
    }
}