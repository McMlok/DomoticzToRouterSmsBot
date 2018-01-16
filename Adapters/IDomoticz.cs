namespace DomoticzToRouterSmsBot.Adapters
{
  internal interface IDomoticz
  {
    void ToggleSwitch(string name, SwitchState state);
  }
}