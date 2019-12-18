using System.Threading.Tasks;

namespace DomoticzToRouterSmsBot.Adapters
{
  internal interface IDomoticz
  {
    Task ToggleSwitchAsync(string name, SwitchState state);
  }
}