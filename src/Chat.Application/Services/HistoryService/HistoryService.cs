using Chat.Domain.Models;
using Chat.Domain.Repositories;

namespace Chat.Application.Services.History;

public class HistoryService : IHistoryService
{
    private readonly IChatRepository chatRepository;
    private readonly IUserRepository userRepository;

    public HistoryService(
        IChatRepository chatRepository,
        IUserRepository userRepository
    )
    {
        this.chatRepository = chatRepository;
        this.userRepository = userRepository;
    }

    public HistoryResponse Get(HistoryRequest request)
    {
        var events = chatRepository.Get(new()
        {
            DateTimeMin = request.DateTimeMin,
            DateTimeMax = request.DateTimeMax,
        });

        return request.ViewType switch
        {
            HistoryViewType.Default => GetDefaultResult(events),
            _ => throw new Exception($"View type {request.ViewType.ToString()} is not supported")
        };
    }

    private HistoryResponse GetDefaultResult(IEnumerable<ChatEvent> events)
    {
        var usersDict = userRepository.Get(
            events.Select(x => x.UserId).Concat(
                events.Select(x => x.Attributes.TargetUserId).Where(x => x.HasValue).Cast<int>()
            )
        ).ToDictionary(x => x.Id);

        return new()
        {
            Data = events.Select(x => new KeyValuePair<string, IEnumerable<string>>(
                x.DateTime.ToShortTimeString(),
                new List<string>() { RenderEvent(x, usersDict) }
            ))
        };
    }

    private string RenderEvent(ChatEvent chatEvent, IDictionary<int, User> users) =>
        chatEvent.Type switch
        {
            ChatEventType.EnterTheRoom => $"{users[chatEvent.UserId].Name} enters the room",
            ChatEventType.LeaveTheRoom => $"{users[chatEvent.UserId].Name} leaves the room",
            ChatEventType.Comment => $"{users[chatEvent.UserId].Name} comments: {chatEvent.Attributes.Message}",
            ChatEventType.HighFiveAnotherUser when chatEvent.Attributes.TargetUserId.HasValue =>
                $"{users[chatEvent.UserId].Name} high-fives {users[chatEvent.Attributes.TargetUserId.Value].Name}",
            _ => throw new Exception($"Render of chat event type {chatEvent.Type.ToString()} is not supported")
        };

}
