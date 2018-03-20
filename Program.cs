using System.Linq;
using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using DomoticzToRouterSmsBot.Proccessor;
using DomoticzToRouterSmsBot.Proccessor.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomoticzToRouterSmsBot
{
  internal class Program
  {
    private static void Main()
    {
      var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
      //setup our DI
      var serviceProvider = new ServiceCollection()
        .AddLogging(c=>c.AddConsole())
        .AddScoped<ISmsLoader, TpLinkRouter>()
        .AddScoped<ISmsParser, SmsParser>()
        .AddScoped<ISmsRunner, SmsRunner>()
        .AddScoped<IDomoticz, Domoticz>()
        .AddScoped<ISmsUpdater, TpLinkRouter>()
        .AddScoped<ToggleSwitch>()
        .AddScoped<MarkAsRead>()
        .AddScoped<ICommand>(provider =>
        {
          var c1 = provider.GetService<ToggleSwitch>();
          ((Command) c1).Use(provider.GetService<MarkAsRead>());
          return c1;
        })
        .AddSingleton(provider => configuration)
        .BuildServiceProvider();

      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      logger.LogInformation("Starting application");

      var sms = serviceProvider.GetService<ISmsLoader>().Load();
      var runner = serviceProvider.GetService<ISmsRunner>();
      foreach (var smsToProccess in sms.Where(s => s.Unread).OrderBy(s => s.Index))
      {
        runner.Run(smsToProccess);
      }
      logger.LogInformation("All done!");
      serviceProvider?.Dispose();
    }
  }
}
