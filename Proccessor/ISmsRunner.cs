using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor
{
  internal interface ISmsRunner
  {
    void Run(Sms sms);
  }
}