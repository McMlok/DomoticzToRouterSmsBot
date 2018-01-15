using System.Collections.Generic;

namespace DomoticzToRouterSmsBot.Loader
{
  internal interface ISmsParser
  {
    ICollection<Sms> Parse(string[] data);
  }
}