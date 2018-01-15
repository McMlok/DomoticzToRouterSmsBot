using System;
using System.Linq;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      //setup our DI
      var serviceProvider = new ServiceCollection()
        .AddLogging(c=>c.AddConsole())
        .AddScoped<ISmsLoader, File>()
        .AddScoped<ISmsParser, SmsParser>()
        .BuildServiceProvider();

      var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
      logger.LogInformation("Starting application");

      var sms = serviceProvider.GetService<ISmsLoader>().Load();
      foreach (var smsToProccess in sms.Where(s=>s.Unread).OrderBy(s=>s.Index))
      {
        logger.LogInformation($"Proccessing sms {smsToProccess.Index} with message {smsToProccess.Message}");
      }

      logger.LogInformation("All done!");
      ((IDisposable)serviceProvider)?.Dispose();
    }
  }
}