using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DomoticzToRouterSmsBot.Adapters;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
  internal class ToggleSwitch : Command
  {
    private readonly ILogger<ToggleSwitch> _logger;
    private readonly IDomoticz _domoticzAdapter;
    private static readonly Regex CommandPattern = new Regex("switch (?'name'\\w*) to (?'state'\\w*)");

    public ToggleSwitch(ILogger<ToggleSwitch> logger, IDomoticz domoticzAdapter)
    {
      _logger = logger;
      _domoticzAdapter = domoticzAdapter;
    }

    public override async Task MiddlewareHandler(Sms e)
    {
      var match = CommandPattern.Match(e.Message);
      if (match.Success)
      {
        var switchName = match.Groups["name"].Value;
        var state = match.Groups["state"].Value;
        if (!Enum.TryParse(typeof(SwitchState), state, true, out object switchState))
        {
          _logger.LogError($"State {state} is not supported. Only ON and OFF");
          return;
        }
        _logger.LogInformation($"Switching {switchName} to state {state}");
        await _domoticzAdapter.ToggleSwitchAsync(switchName, (SwitchState)switchState);
      }
    }
  }
}