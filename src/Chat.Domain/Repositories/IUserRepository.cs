using Chat.Domain.Models;

namespace Chat.Domain.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// Returns users by theirs ids
    /// </summary>
    IEnumerable<User> Get(IEnumerable<int> ids);
}
