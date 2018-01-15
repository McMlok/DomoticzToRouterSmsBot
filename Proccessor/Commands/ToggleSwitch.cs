using System.Text.RegularExpressions;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
  internal class ToggleSwitch : Command
  {
    private readonly ILogger<ToggleSwitch> _logger;
    private static readonly Regex CommandPattern = new Regex("switch (?'name'\\w*) to (?'state'\\w*)");

    public ToggleSwitch(ILogger<ToggleSwitch> logger)
    {
      _logger = logger;
    }

    public override void MiddlewareHandler(object sender, Sms e)
    {
      var match = CommandPattern.Match(e.Message);
      if (match.Success)
      {
        var switchName = match.Groups["name"].Value;
        var state = match.Groups["state"].Value;
        _logger.LogInformation($"Switching {switchName} to state {state}");
        //TODO: Call domoticz GET command to togle switch
      }
    }
  }
}