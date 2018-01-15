﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DomoticzToRouterSmsBot.Loader
{
  internal class SmsParser : ISmsParser
  {
    private static readonly Regex SmsHeader = new Regex("^\\[(\\d[,]){5}\\d\\]\\d");
    private static readonly Regex SmsFrom = new Regex("from=(?'value'\\d*)");
    private static readonly Regex SmsMessage = new Regex("content=(?'value'.*)");
    private static readonly Regex SmsUnread = new Regex("unread=(?'value'\\d*)");
    private static readonly Regex SmsIndex = new Regex("index=(?'value'\\d*)");

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
        if(TryGetValue(SmsMessage, line, out string message))
          newSms.Message = message;
        if(TryGetValue(SmsUnread, line, out string unread))
          newSms.Unread = unread == "1";
        if(TryGetValue(SmsIndex, line, out string index))
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
        value = message.Groups["value"]?.Value;
        return true;
      }

      value = null;
      return false;
    }
  }
}