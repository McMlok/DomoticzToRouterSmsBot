using System;

namespace DomoticzToRouterSmsBot.Loader
{
  internal class Sms
  {
    public int Index { get; set; }

    public string From { get; set; }

    public string Message { get; set; }
    public bool Unread { get; set; }

    public DateTime RecievedTime {get;set;}
  }
}