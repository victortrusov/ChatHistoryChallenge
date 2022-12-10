using Chat.ConsoleApp.Configurations;
using Chat.ConsoleApp.Output;
using Chat.Application.Services.History;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleApp;

public class Application : IHostedService
{
    private readonly IOptions<AppConfiguration> configurationProvider;
    private readonly IHistoryService historyService;
    private readonly IConsole console;

    public Application(
        IOptions<AppConfiguration> configurationProvider,
        IHistoryService historyService,
        IConsole console
    )
    {
        this.console = console;
        this.configurationProvider = configurationProvider;
        this.historyService = historyService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        console.WriteTable(historyService.Get(new()).Data.Select(x => new
        {
            Key = x.Key,
            Value = string.Join('\n', x.Value)
        }));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
