using Chat.ConsoleApp;
using Chat.ConsoleApp.Configurations;
using Chat.ConsoleApp.Output;
using Chat.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(AddServices)
    .RunConsoleAsync();

static void AddServices(HostBuilderContext context, IServiceCollection services)
{
    services.Configure<AppConfiguration>(
        context.Configuration.GetSection(AppConfiguration.SectionName)
    );

    services.AddServices();
    services.AddSingleton<IConsole, DefaultConsole>();

    services.AddHostedService<Application>();

    services.Configure<HostOptions>(hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });
}

