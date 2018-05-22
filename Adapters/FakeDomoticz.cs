using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Adapters
{
  class FakeDomoticz : IDomoticz
  {
    private readonly ILogger<FakeDomoticz> _logger;
    private readonly string _domoticzUri;
    public FakeDomoticz(ILogger<FakeDomoticz> logger)
    {
      _logger = logger;
    }

        public void ToggleSwitch(string name, SwitchState state)
        {
            _logger.LogInformation($"Switching {name} to {state}");
        }
    }
}