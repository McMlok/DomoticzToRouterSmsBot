using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DomoticzToRouterSmsBot.Adapters
{
  class TpLinkRouter : ISmsLoader, ISmsUpdater
  {
    private readonly ISmsParser _parser;
    private readonly string _password;
    private readonly string _baseUri;
    private readonly ILogger<TpLinkRouter> _logger;

    private const string LoadSmsRequestData = "[LTE_SMS_RECVMSGENTRY#0,0,0,0,0,0#0,0,0,0,0,0]0,5\r\nindex\r\nfrom\r\ncontent\r\nreceivedTime\r\nunread\r\n";
    private const string GetSmsPageRequestData = "[LTE_SMS_RECVMSGBOX#0,0,0,0,0,0#0,0,0,0,0,0]0,1\r\nPageNumber=1\r\n";

    public TpLinkRouter(IConfigurationRoot configuration, ISmsParser parser, ILogger<TpLinkRouter> logger)
    {
      _parser = parser;
      _password = configuration["TpLinkPassword"];
      _baseUri = configuration["TpLinkUri"];
      _logger = logger;
    }

    public async Task<ICollection<Sms>> Load()
    {
      using (var client = new HttpClient())
      {
        await GetDataFromRouter(client, CreatePagesUri(), GetSmsPageRequestData);
        var result =  await GetDataFromRouter(client, CreateLoadUri(), LoadSmsRequestData);
        return _parser.Parse(result);
      }
    }

    private async Task<string> GetDataFromRouter(HttpClient client, string uri, string data)
    {
      _logger.LogInformation($"Loading data from {uri}");
      
      HttpRequestMessage message =
        new HttpRequestMessage(HttpMethod.Post, uri) {Content = new StringContent(data) };
      message.Headers.Add("Cookie", $"Authorization=Basic {_password}");
      message.Headers.Add("Referer", _baseUri);
      var result = await client.SendAsync(message);
      result.EnsureSuccessStatusCode();
      var content = await result.Content.ReadAsStringAsync();
      _logger.LogInformation($"Request result: {content}");
      return content;
    }

    private string CreateLoadUri()
    {
      return $"{_baseUri}cgi?5";
    }

    private string CreatePagesUri()
    {
      return $"{_baseUri}cgi?2";
    }

    public async Task MarkAsRead(int index)
    {
      using (var client = new HttpClient())
      {
        //ADD BASIC AUTH
        var authByteArray = Encoding.ASCII.GetBytes($"{_password}");
        var authString = Convert.ToBase64String(authByteArray);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);

        HttpRequestMessage message =
          new HttpRequestMessage(HttpMethod.Post, CreateLoadUri()) { Content = new StringContent(LoadSmsRequestData) };
        //TODO: set correct data for mark as read
        await client.SendAsync(message);
      }
    }
  }

  internal interface ISmsUpdater
  {
    Task MarkAsRead(int index);
  }
}
