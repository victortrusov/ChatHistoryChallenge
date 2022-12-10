using Chat.Domain.Filters;
using Chat.Domain.Models;

namespace Chat.Domain.Repositories;

public interface IChatRepository
{
    IEnumerable<ChatEvent> Get(ChatEventFilter filter);
}
