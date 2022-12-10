using Chat.Domain.Models;

namespace Chat.Domain.Repositories;

public interface IUserRepository
{
    IEnumerable<User> Get(IEnumerable<int> ids);
}
