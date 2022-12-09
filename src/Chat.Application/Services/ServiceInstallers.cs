using Chat.Application.Services.History;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Application.Services;

public static class ServiceInstallers
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IHistoryService, HistoryService>();

        return services;
    }
}
