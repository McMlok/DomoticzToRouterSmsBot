using System.Collections.Generic;

namespace DomoticzToRouterSmsBot.Loader
{
  internal interface ISmsLoader
  {
    ICollection<Sms> Load();
  }
}