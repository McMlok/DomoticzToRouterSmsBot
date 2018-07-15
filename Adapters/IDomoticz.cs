using System.Threading.Tasks;

namespace DomoticzToRouterSmsBot.Adapters
{
  internal interface IDomoticz
  {
    Task ToggleSwitch(string name, SwitchState state);
  }
}