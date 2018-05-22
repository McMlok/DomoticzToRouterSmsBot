using System.Net.Http;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        string uri = GetCommandUri($"type=command&param=switchlight&idx={idx.Value}&switchcmd={state}");
        var result = client.GetAsync(uri).GetAwaiter().GetResult();
        var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        _logger.LogInformation($"Swith {idx.Value} to state {state} result with {content}");
      }
    }

    private string GetCommandUri(string command)
    {
      return $"{_domoticzUri}/json.htm?{command}";
    }

    private int? GetIdByName(string name)
    {
      using (var client = new HttpClient())
      {
        string uri = GetCommandUri($"type=devices&filter=light");
        _logger.LogInformation($"Loading data from {uri}");
        var result = client.GetAsync(uri).GetAwaiter().GetResult();
        var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        dynamic json = JsonConvert.DeserializeObject(content);
        foreach(var device in json["result"]){
            var idx = 0;
            if(device["Name"] == name && int.TryParse(device["idx"]?.ToString(), out idx)){
                return idx;
            }
        }
        return null;
      }
    }
  }
}