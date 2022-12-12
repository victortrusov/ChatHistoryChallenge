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

        // Better to use Strategy pattern but I decided to save some time and do it like this
        return request.ViewType switch
        {
            HistoryViewType.Default => GetDefaultResult(events),
            HistoryViewType.Aggregation when !request.AggregationType.HasValue =>
                throw new Exception($"Empty aggregation type"),
            HistoryViewType.Aggregation => GetTimeAggregationResult(events, request.AggregationType.Value),
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

    private HistoryResponse GetTimeAggregationResult(IEnumerable<ChatEvent> events, HistoryAggregationType type)
    {
        Func<ChatEvent, object> aggregationFunc = type switch
        {
            HistoryAggregationType.Hours => x => x.DateTime.Hour,
            _ => throw new Exception($"Aggregation type {type.ToString()} is not supported")
        };

        var aggregationGroup = events.GroupBy(aggregationFunc);

        return new()
        {
            Data = aggregationGroup.Select(x => new KeyValuePair<string, IEnumerable<string>>(
                x.Key.ToString()!,
                x.OrderBy(x => x.Type).GroupBy(x => x.Type).Select(RenderAggregation)
            ))
        };
    }

    private string RenderAggregation(IGrouping<ChatEventType, ChatEvent> group)
    {
        var count = group.Count();
        return group.Key switch
        {
            ChatEventType.EnterTheRoom => $"{count} {NaivePluralize("person", count)} entered",
            ChatEventType.LeaveTheRoom => $"{count} {NaivePluralize("person", count)} left",
            ChatEventType.Comment => $"{count} {NaivePluralize("comment", count)}",
            ChatEventType.HighFiveAnotherUser when group.Any() => RenderHighFiveAggregation(count, group),
            _ => throw new Exception($"Aggregation render of chat event type {group.Key.ToString()} is not supported")
        };
    }

    private string RenderHighFiveAggregation(int count, IGrouping<ChatEventType, ChatEvent> group)
    {
        var otherUsersCount = group.Select(x => x.Attributes.TargetUserId).Distinct().Count();
        return $"{group.Count()} {NaivePluralize("comment", count)} high-fived {otherUsersCount} other {NaivePluralize("person", count)}";
    }

    private string NaivePluralize(string word, int number) => number == 1
        ? word
        : word + "s";
}
