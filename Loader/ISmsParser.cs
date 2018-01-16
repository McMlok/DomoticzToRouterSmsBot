using System.Collections.Generic;

namespace DomoticzToRouterSmsBot.Loader
{
  internal interface ISmsParser
  {
    ICollection<Sms> Parse(string[] data);
    ICollection<Sms> Parse(string result);
  }
}