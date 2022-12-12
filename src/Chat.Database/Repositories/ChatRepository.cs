using Chat.Domain.Filters;
using Chat.Domain.Models;
using Chat.Domain.Repositories;

namespace Chat.Database.Repositories;

public class ChatRepository : IChatRepository
{
    // Mock
    private List<ChatEvent> chatEvents = new() {
        new() { Id = 1, DateTime = new DateTime(2022,12,11,14,0,0), UserId = 1, Type = ChatEventType.EnterTheRoom },
        new() { Id = 2, DateTime = new DateTime(2022,12,11,14,1,0), UserId = 2, Type = ChatEventType.EnterTheRoom },
        new() { Id = 3, DateTime = new DateTime(2022,12,11,14,5,0), UserId = 1, Type = ChatEventType.Comment, Attributes = new() { Message = "Hey" } },
        new() { Id = 4, DateTime = new DateTime(2022,12,11,15,0,0), UserId = 2, Type = ChatEventType.HighFiveAnotherUser, Attributes = new() { TargetUserId = 1 } },
        new() { Id = 5, DateTime = new DateTime(2022,12,11,15,10,0), UserId = 3, Type = ChatEventType.EnterTheRoom },
        new() { Id = 6, DateTime = new DateTime(2022,12,11,15,15,0), UserId = 3, Type = ChatEventType.HighFiveAnotherUser, Attributes = new() { TargetUserId = 2 } },
        new() { Id = 7, DateTime = new DateTime(2022,12,11,16,0,0), UserId = 1, Type = ChatEventType.LeaveTheRoom },
        new() { Id = 8, DateTime = new DateTime(2022,12,11,16,30,0), UserId = 3, Type = ChatEventType.Comment, Attributes = new() { Message = "Omg such a long mock" } },
        new() { Id = 9, DateTime = new DateTime(2022,12,11,16,35,0), UserId = 2, Type = ChatEventType.LeaveTheRoom },
    };

    public IEnumerable<ChatEvent> Get(ChatEventFilter filter) => chatEvents.Where(x =>
        (filter.DateTimeMin is null || x.DateTime >= filter.DateTimeMin) &&
        (filter.DateTimeMax is null || x.DateTime < filter.DateTimeMax)
    ).OrderBy(x => x.DateTime);
}
