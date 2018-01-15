using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
    internal interface ICommand
    {
      void Handle(Sms sms);
    }
}
