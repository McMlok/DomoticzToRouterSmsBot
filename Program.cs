using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using DomoticzToRouterSmsBot.Proccessor;
using DomoticzToRouterSmsBot.Proccessor.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using NLog.Extensions.Logging;

namespace DomoticzToRouterSmsBot
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile("lastProccessedSMS.json", optional: true, reloadOnChange: true)
              .Build();
            //setup our DI
 
            var serviceProvider = BuildDi(configuration);

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting application");

            var sms = await serviceProvider.GetService<ISmsLoader>().Load();
            logger.LogInformation($"SMS to process {sms?.Count()}");
            var runner = serviceProvider.GetService<ISmsRunner>();
            DateTime lastProccessedTime = DateTime.MinValue;
            if (!String.IsNullOrEmpty(configuration["LastProccessedTime"]))
            {
                lastProccessedTime = Convert.ToDateTime(configuration["LastProccessedTime"]);
            }
            foreach (var smsToProccess in sms.Where(s => s.RecievedTime > lastProccessedTime).OrderBy(s => s.Index))
            {
                logger.LogInformation($"Proccessing sms with id {smsToProccess.Index}");
                runner.Run(smsToProccess);
            }
            logger.LogInformation("All done!");
        }

        private static IServiceProvider BuildDi(IConfigurationRoot configuration)
        {
            var services = new ServiceCollection();

            //Runner is the custom class
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
              })
              .AddSingleton(provider => configuration);
            if (String.IsNullOrEmpty(configuration["DataFilePath"]))
            {
                services.AddScoped<IDomoticz, Domoticz>();
                services.AddScoped<ISmsLoader, TpLinkRouter>();
                services.AddHttpClient("router").AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));
                services.AddHttpClient("domoticz").AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));
            }
            else
            {
                services.AddScoped<IDomoticz, FakeDomoticz>();
                services.AddScoped<ISmsLoader, File>();
            }
            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            return serviceProvider;
        }
    }

    public class Proccessed
    {
        public DateTime LastProccessedTime { get; set; }
    }
}