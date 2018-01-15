using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Loader
{
  internal class SmsParser : ISmsParser
  {
    private static readonly Regex smsHeader = new Regex("^\\[(\\d[,]){5}\\d\\]\\d");
    private static readonly Regex smsFrom = new Regex("from=(?'value'\\d*)");
    private static readonly Regex smsMessage = new Regex("content=(?'value'.*)");
    private static readonly Regex smsUnread = new Regex("unread=(?'value'\\d*)");
    private static readonly Regex smsIndex = new Regex("index=(?'value'\\d*)");
    private readonly ILogger<File> _logger;

    public SmsParser(ILogger<File> logger)
    {
      _logger = logger;
    }

    public ICollection<Sms> Parse(string[] data)
    {
      var sms = new List<Sms>();
      Sms newSms = null;
      foreach (var line in data)
      {
        if (smsHeader.IsMatch(line))
        {
          if(newSms != null)
            sms.Add(newSms);
          newSms = new Sms();
          continue;
        }

        if(TryGetValue(smsFrom, line, out string from))
          newSms.From = from;
        if(TryGetValue(smsMessage, line, out string message))
          newSms.Message = message;
        if(TryGetValue(smsUnread, line, out string unread))
          newSms.Unread = unread == "1" ? true : false;
        if(TryGetValue(smsIndex, line, out string index))
          newSms.Index = Convert.ToInt32(index);
      }
      if (newSms != null)
        sms.Add(newSms);
      return sms;
    }

    private bool TryGetValue(Regex regex, string line, out string value)
    {
      var message = regex.Match(line);
      if (message.Success)
      {
        value = message?.Groups["value"]?.Value;
        return true;
      }

      value = null;
      return false;
    }
  }
}