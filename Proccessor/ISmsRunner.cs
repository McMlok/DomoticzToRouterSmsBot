using System.Threading.Tasks;
using DomoticzToRouterSmsBot.Loader;

namespace DomoticzToRouterSmsBot.Proccessor
{
  internal interface ISmsRunner
  {
    Task RunAsync(Sms sms);
  }
}