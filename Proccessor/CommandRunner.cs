﻿using System;
using System.Collections.Generic;
using System.Linq;
using DomoticzToRouterSmsBot.Loader;
using DomoticzToRouterSmsBot.Proccessor.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Proccessor
{
  internal class SmsRunner : ISmsRunner
  {
    private readonly ILogger<SmsRunner> _logger;
    private readonly ICommand _command;
    private readonly List<string> _allowedNumbers;
    private readonly ICommand command;

    public SmsRunner(ILogger<SmsRunner> logger, IConfigurationRoot configuration, ICommand command)
    {
      _logger = logger;
      _command = command;
      _allowedNumbers = configuration["AllowedNumbers"]?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ??
                        new List<string>();
      
    }

    public void Run(Sms sms)
    {
      if (!_allowedNumbers.Contains(sms.From))
      {
        _logger.LogError($"Number {sms.From} is not allowed to run any command");
        return;
      }
      _command.Handle(sms);
    }
  }
}