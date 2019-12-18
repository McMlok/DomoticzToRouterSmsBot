using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DomoticzToRouterSmsBot.Adapters
{
  class Domoticz : IDomoticz
  {
    private readonly ILogger<Domoticz> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _domoticzUri;
    public Domoticz(IConfiguration options, ILogger<Domoticz> logger, IHttpClientFactory httpClientFactory)
    {
      _logger = logger;
      _httpClient = httpClientFactory.CreateClient("domoticz");
      _domoticzUri = options["DomoticzUri"];
    }

    public async Task ToggleSwitchAsync(string name, SwitchState state)
    {
      var idx = await GetIdByName(name);
      if (!idx.HasValue)
      {
        _logger.LogError($"Device with name {name} not found");
        return;
      }

      string uri = GetCommandUri($"type=command&param=switchlight&idx={idx.Value}&switchcmd={state}");
      var result = await _httpClient.GetAsync(uri);
      var content = await result.Content.ReadAsStringAsync();
      _logger.LogInformation($"Swith {idx.Value} to state {state} result with {content}");
    }

    private string GetCommandUri(string command)
    {
      return $"{_domoticzUri}/json.htm?{command}";
    }

    private async Task<int?> GetIdByName(string name)
    {
      string uri = GetCommandUri($"type=devices&filter=light");
      _logger.LogInformation($"Loading data from {uri}");
      var result = await _httpClient.GetAsync(uri);
      var content = await result.Content.ReadAsStringAsync();
      JObject json = JObject.Parse(content);
      JToken idx = json.SelectToken($"$.result[?(@.Name == '{name}')].idx");
      if(idx != null){
        return int.Parse(idx.ToString());
      }
      return null;
    }
  }
}