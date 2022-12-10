using Chat.Database.Repositories;
using Chat.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Database;

public static class DatabaseInstallers
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IChatRepository, ChatRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        return services;
    }
}
