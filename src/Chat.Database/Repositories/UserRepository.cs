using Chat.Domain.Models;
using Chat.Domain.Repositories;

namespace Chat.Database.Repositories;

public class UserRepository : IUserRepository
{
    // Mock
    private List<User> users = new() {
        new() { Id = 1, Name = "Bob" },
        new() { Id = 2, Name = "Alice" },
        new() { Id = 3, Name = "Victor" },
        new() { Id = 4, Name = "Diego" },
        new() { Id = 5, Name = "Lana" }
    };

    public IEnumerable<User> Get(IEnumerable<int> ids) => users.Where(x => ids.Contains(x.Id));
}
