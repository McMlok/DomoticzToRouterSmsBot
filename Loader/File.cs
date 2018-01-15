using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Loader
{
    internal class File : ISmsLoader
    {
      private readonly ILogger<File> _logger;
      private readonly ISmsParser _parser;

      public File(ILogger<File> logger, ISmsParser parser)
      {
        _logger = logger;
        _parser = parser;
      }

      public ICollection<Sms> Load()
      {
        _logger.LogInformation("loading data from file");
        var data = System.IO.File.ReadAllLines("SampleData/sampleSMS.txt");
        return _parser.Parse(data);
      }
    }
}
