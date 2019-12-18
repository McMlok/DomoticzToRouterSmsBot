using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
    internal class MarkAsRead : Command
    {
      private readonly ILogger<MarkAsRead> _logger;
      private readonly IConfiguration _configuration;

      public MarkAsRead(ILogger<MarkAsRead> logger, IConfiguration configuration)
      {
        _logger = logger;
        _configuration = configuration;
      }

      public override void MiddlewareHandler(object sender, Sms e)
      {
        _logger.LogInformation($"SMS {e.Index} marked as read");
        string json = JsonConvert.SerializeObject(new Proccessed{LastProccessedTime = e.RecievedTime}, Formatting.Indented);
        System.IO.File.WriteAllText(_configuration["lastProcessedFileName"], json);
      }
    }
}
