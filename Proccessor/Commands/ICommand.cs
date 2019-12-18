using System.Threading.Tasks;
using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor.Commands
{
    internal interface ICommand
    {
      Task Handle(Sms sms);
    }
}
