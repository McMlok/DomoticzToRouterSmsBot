using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DomoticzToRouterSmsBot.Adapters
{
  class FakeDomoticz : IDomoticz
  {
        private readonly ILogger<FakeDomoticz> _logger;
        public FakeDomoticz(ILogger<FakeDomoticz> logger)
        {
            _logger = logger;
        }
        public Task ToggleSwitchAsync(string name, SwitchState state)
        {
            _logger.LogInformation($"Switching {name} to {state}");
            return Task.CompletedTask;
        }
    }
}