using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
    internal class MarkAsRead : Command
    {
      private readonly ILogger<MarkAsRead> _logger;

      public MarkAsRead(ILogger<MarkAsRead> logger)
      {
        _logger = logger;
      }

      public override void MiddlewareHandler(object sender, Sms e)
      {
        _logger.LogInformation($"SMS {e.Index} marked as read");
        //TODO: Set sms as read on TP-LINK
      }
    }
}
