using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DomoticzToRouterSmsBot.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DomoticzToRouterSmsBot.Adapters
{
  class TpLinkRouter : ISmsLoader, ISmsUpdater
  {
    private readonly ISmsParser _parser;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _baseUri;
    private readonly ILogger<TpLinkRouter> _logger;

    private const string LoadSmsRequestData = "[LTE_SMS_RECVMSGENTRY#0,0,0,0,0,0#0,0,0,0,0,0]0,5\r\nindex\r\nfrom\r\ncontent\r\nreceivedTime\r\nunread";

    public TpLinkRouter(IConfigurationRoot configuration, ISmsParser parser, ILogger<TpLinkRouter> logger)
    {
      _parser = parser;
      _userName = configuration["TpLinkUserName"];
      _password = configuration["TpLinkPassword"];
      _baseUri = configuration["TpLinkUri"];
      _logger = logger;
    }

    public ICollection<Sms> Load()
    {
        //ADD BASIC AUTH
        var authByteArray = Encoding.ASCII.GetBytes($"{_password}");
        var authString = Convert.ToBase64String(authByteArray);
        var uri = CreateLoadUri();
        _logger.LogInformation($"Loading data from {uri}");

        using (var client = new HttpClient())
        {
          HttpRequestMessage message =
            new HttpRequestMessage(HttpMethod.Post, uri) {Content = new StringContent(LoadSmsRequestData) };
          message.Headers.Add("Cookie", $"Authorization=Basic {authString}");
          var result = client.SendAsync(message).GetAwaiter().GetResult();
          result.EnsureSuccessStatusCode();
          var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
          return _parser.Parse(content);
        }
    }

    private string CreateLoadUri()
    {
      return $"{_baseUri}cgi?5";
    }

    public void MarkAsRead(int index)
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
        client.SendAsync(message).Wait();
      }
    }
  }

  internal interface ISmsUpdater
  {
    void MarkAsRead(int index);
  }
}
