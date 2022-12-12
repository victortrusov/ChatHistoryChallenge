using System.Linq;
using Chat.ConsoleApp.Configurations;
using Chat.ConsoleApp.Output;
using Chat.Application.Services.History;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleApp;

public class Application : IHostedService
{
    private readonly IOptions<AppConfiguration> configurationProvider;
    private readonly IOptions<ArgumentOptions> argumentOptionsProvider;
    private readonly IHistoryService historyService;
    private readonly IConsole console;

    public Application(
        IOptions<AppConfiguration> configurationProvider,
        IOptions<ArgumentOptions> argumentOptionsProvider,
        IHistoryService historyService,
        IConsole console
    )
    {
        this.console = console;
        this.configurationProvider = configurationProvider;
        this.argumentOptionsProvider = argumentOptionsProvider;
        this.historyService = historyService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var viewType = argumentOptionsProvider.Value.AggregationType is not null
            ? HistoryViewType.Aggregation
            : HistoryViewType.Default;

        HistoryAggregationType? aggregationType = argumentOptionsProvider.Value.AggregationType switch
        {
            null => null,
            "hourly" => HistoryAggregationType.Hours,
            _ => throw new ArgumentException("Unsupported aggregation type")
        };

        var result = historyService.Get(new()
        {
            ViewType = viewType,
            AggregationType = aggregationType
        });

        console.WriteTable(
            result.Data.SelectMany(x =>
                x.Value.Select((y, i) => new
                {
                    Key = i == 0 ? x.Key : "",
                    Value = y
                })
            )
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
