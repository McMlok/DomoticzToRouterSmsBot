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

    private const string LoadSmsRequestData = "";

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

        var baseAddress = new Uri(_baseUri);
        var cookieContainer = new CookieContainer();
        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
        using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
        {
          cookieContainer.Add(baseAddress, new Cookie("Authorization", $"Basic {authString}"));
          var result = client.PostAsync("/cgi?5", new StringContent(LoadSmsRequestData)).GetAwaiter().GetResult();
          result.EnsureSuccessStatusCode();
          var content = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
          _logger.LogInformation(content);
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
