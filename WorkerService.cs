using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DomoticzToRouterSmsBot.Loader;
using DomoticzToRouterSmsBot.Proccessor;

internal class WorkerService : IHostedService
{
    private readonly ILogger _logger;
    private readonly ISmsLoader _loader;
    private readonly ISmsRunner _runner;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _lifeTime;

    public WorkerService(ILogger<WorkerService> logger, ISmsLoader loader, ISmsRunner runner, IConfiguration configuration, IHostApplicationLifetime lifeTime){
        _logger = logger;
        _loader = loader;
        _runner = runner;
        _configuration = configuration;
        _lifeTime = lifeTime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting application");

        var sms = await _loader.LoadAsync();
        _logger.LogInformation($"SMS to process {sms?.Count()}");
        DateTime lastProccessedTime = DateTime.MinValue;
        if (!String.IsNullOrEmpty(_configuration["LastProccessedTime"]))
        {
            lastProccessedTime = Convert.ToDateTime(_configuration["LastProccessedTime"]);
        }
        _logger.LogInformation($"Last proccessed time {lastProccessedTime}");
        foreach (var smsToProccess in sms.Where(s => s.RecievedTime > lastProccessedTime).OrderBy(s => s.Index))
        {
            if(cancellationToken.IsCancellationRequested)
                return;
            _logger.LogInformation($"Proccessing sms with id {smsToProccess.Index}");
            await _runner.RunAsync(smsToProccess);
        }
        _logger.LogInformation("All done!");
        _lifeTime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
