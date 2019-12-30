using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomoticzToRouterSmsBot.Loader
{
  internal class SmsParser : ISmsParser
  {
    private static readonly Regex SmsHeader = new Regex("^\\[(\\d[,]){5}\\d\\]\\d");
    private static readonly Regex SmsFrom = new Regex("from=(?'value'[+]{0,1}\\d*)");
    private static readonly Regex SmsMessage = new Regex("content=(?'value'.*)");
    private static readonly Regex SmsUnread = new Regex("unread=(?'value'\\d*)");
    private static readonly Regex SmsIndex = new Regex("index=(?'value'\\d*)");

    private static readonly Regex SmsRecievedTime = new Regex("receivedTime=(?'value'[0-9 -:]*)");

    public ICollection<Sms> Parse(string[] data)
    {
      var sms = new List<Sms>();
      Sms newSms = null;
      foreach (var line in data)
      {
        if (SmsHeader.IsMatch(line))
        {
          if(newSms != null)
            sms.Add(newSms);
          newSms = new Sms();
          continue;
        }
        if(newSms == null)
          continue;
        if(TryGetValue(SmsFrom, line, out string from))
          newSms.From = from;
        if(line.ToLowerInvariant() == "from=info")
        {
          newSms.From = "Info";
        }
        if(TryGetValue(SmsMessage, line, out string message))
          newSms.Message = message;
        if(TryGetValue(SmsUnread, line, out string unread))
          newSms.Unread = unread == "1";
        if(TryGetValue(SmsIndex, line, out string index))
          newSms.Index = Convert.ToInt32(index);
        if(TryGetValue(SmsRecievedTime, line, out string receivedTime))
          newSms.RecievedTime = Convert.ToDateTime(receivedTime);
      }
      if (newSms != null)
        sms.Add(newSms);
      return sms;
    }

    public ICollection<Sms> Parse(string result)
    {
      return Parse(result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
    }

    private bool TryGetValue(Regex regex, string line, out string value)
    {
      var message = regex.Match(line);
      if (message.Success)
      {
        value = message.Groups["value"]?.Value;
        return true;
      }

      value = null;
      return false;
    }
  }
}
