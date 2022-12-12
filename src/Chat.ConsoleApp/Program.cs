using Chat.ConsoleApp;
using Chat.ConsoleApp.Configurations;
using Chat.ConsoleApp.Output;
using Chat.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Chat.Database;
using CommandLine;
using Microsoft.Extensions.Options;

Parser.Default.ParseArguments<ArgumentOptions>(args)
    .WithParsed<ArgumentOptions>(argOptions =>
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) => AddServices(context, services, argOptions))
            .RunConsoleAsync();
    });

static void AddServices(HostBuilderContext context, IServiceCollection services, ArgumentOptions argOptions)
{
    services.Configure<AppConfiguration>(
        context.Configuration.GetSection(AppConfiguration.SectionName)
    );

    services.AddSingleton<IOptions<ArgumentOptions>>(_ => Options.Create(argOptions));

    services.AddServices();
    services.AddDatabase();
    services.AddSingleton<IConsole, DefaultConsole>();

    services.AddHostedService<Application>();

    services.Configure<HostOptions>(hostOptions =>
        {
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        });
}

