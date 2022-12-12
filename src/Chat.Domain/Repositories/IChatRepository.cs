using Chat.Domain.Filters;
using Chat.Domain.Models;

namespace Chat.Domain.Repositories;

public interface IChatRepository
{
    /// <summary>
    /// Returns chat events by filter
    /// </summary>
    IEnumerable<ChatEvent> Get(ChatEventFilter filter);
}
