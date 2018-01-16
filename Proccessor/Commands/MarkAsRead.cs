using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
    internal class MarkAsRead : Command
    {
      private readonly ILogger<MarkAsRead> _logger;
      private readonly ISmsUpdater _smsUpdater;

      public MarkAsRead(ILogger<MarkAsRead> logger, ISmsUpdater smsUpdater)
      {
        _logger = logger;
        _smsUpdater = smsUpdater;
      }

      public override void MiddlewareHandler(object sender, Sms e)
      {
        _logger.LogInformation($"SMS {e.Index} marked as read");
        _smsUpdater.MarkAsRead(e.Index);
      }
    }
}
