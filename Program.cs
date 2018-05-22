using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace DomoticzToRouterSmsBot
{
  internal class Program
  {
    public static async Task Main(string[] args)
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
        .AddJsonFile("lastProccessedSMS.json", optional:true, reloadOnChange:true)
        .Build();
      //setup our DI
      var serviceCollection = new ServiceCollection()
        .AddLogging(c=>c.AddConsole())
        .AddScoped<ISmsParser, SmsParser>()
        .AddScoped<ISmsRunner, SmsRunner>()
        .AddScoped<ISmsUpdater, TpLinkRouter>()
        .AddScoped<ToggleSwitch>()
        .AddScoped<MarkAsRead>()
        .AddScoped<ICommand>(provider =>
        {
          var c1 = provider.GetService<ToggleSwitch>();
          ((Command) c1).Use(provider.GetService<MarkAsRead>());
          return c1;
        })
        .AddSingleton(provider => configuration);
        if(String.IsNullOrEmpty(configuration["DataFilePath"])){
          serviceCollection.AddScoped<IDomoticz, Domoticz>();
          serviceCollection.AddScoped<ISmsLoader, TpLinkRouter>();  
        }
        else{
          serviceCollection.AddScoped<IDomoticz, FakeDomoticz>();
          serviceCollection.AddScoped<ISmsLoader, File>();  
        }
        var serviceProvider = serviceCollection.BuildServiceProvider();

      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      logger.LogInformation("Starting application");

      var sms = await serviceProvider.GetService<ISmsLoader>().Load();
      logger.LogInformation($"SMS to process {sms?.Count()}");
      var runner = serviceProvider.GetService<ISmsRunner>();
      DateTime lastProccessedTime = DateTime.MinValue;
      if(!String.IsNullOrEmpty(configuration["LastProccessedTime"])){
        lastProccessedTime = Convert.ToDateTime(configuration["LastProccessedTime"]);
      }
      foreach (var smsToProccess in sms.Where(s => s.RecievedTime > lastProccessedTime).OrderBy(s => s.Index))
      {
        logger.LogInformation($"Proccessing sms with id {smsToProccess.Index}");
        runner.Run(smsToProccess);
      }
      logger.LogInformation("All done!");
      serviceProvider?.Dispose();
    }
  }

  public class Proccessed{
    public DateTime LastProccessedTime { get; set; }
  }
}