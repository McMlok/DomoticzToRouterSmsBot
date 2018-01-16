using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DomoticzToRouterSmsBot.Adapters
{
  class Domoticz : IDomoticz
  {
    private readonly ILogger<Domoticz> _logger;
    private readonly string _domoticzUri;
    public Domoticz(IConfigurationRoot options, ILogger<Domoticz> logger)
    {
      _logger = logger;
      _domoticzUri = options["DomoticzUri"];
    }

    public void ToggleSwitch(string name, SwitchState state)
    {
      var idx = GetIdByName(name);
      if (!idx.HasValue)
      {
        _logger.LogError($"Device with name {name} not found");
        return;
      }

      using (var client = new HttpClient())
      {
        string uri = GetCommandUri($"param=switchlight&idx={idx.Value}&switchcmd={state}");
        client.GetAsync(uri).Wait();
      }
    }

    private string GetCommandUri(string command)
    {
      return $"{_domoticzUri}/json/html?{command}";
    }

    private int? GetIdByName(string name)
    {
      using (var client = new HttpClient())
      {
        string uri = GetCommandUri($"type=devices&name={name}");
        var result = client.GetAsync(uri).Result;
        dynamic json = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result);
        if (int.TryParse(json["result"][0]["idx"]?.ToString(), out int idx))
        {
          return idx;
        }
        return null;
      }
    }
  }
}