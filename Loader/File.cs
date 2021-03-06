using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Loader
{
    internal class File : ISmsLoader
    {
      private readonly ILogger<File> _logger;
      private readonly ISmsParser _parser;
      private readonly IConfiguration _configuration;

      public File(ILogger<File> logger, ISmsParser parser, IConfiguration configuration)
      {
        _logger = logger;
        _parser = parser;
        _configuration = configuration;
      }

      public async Task<ICollection<Sms>> LoadAsync()
      {
        var fileName = _configuration["DataFilePath"];
        _logger.LogInformation($"loading data from file {fileName}");
        var data = await System.IO.File.ReadAllLinesAsync(fileName);
        return _parser.Parse(data);
      }
    }
}
