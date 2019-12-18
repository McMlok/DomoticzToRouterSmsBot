using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomoticzToRouterSmsBot.Loader
{
  internal interface ISmsLoader
  {
    Task<ICollection<Sms>> LoadAsync();
  }
}