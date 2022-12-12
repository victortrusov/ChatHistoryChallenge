using Chat.Application.Services.History;
using Chat.Domain.Filters;
using Chat.Domain.Models;
using Chat.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Chat.Services.Tests;

public class HistoryServiceTests
{
    private HistoryService service;

    private List<ChatEvent> chatEventMock = new List<ChatEvent>() {
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

    private List<User> userMock = new List<User>() {
        new() { Id = 1, Name = "Bob" },
        new() { Id = 2, Name = "Alice" },
        new() { Id = 3, Name = "Victor" },
        new() { Id = 4, Name = "Diego" },
        new() { Id = 5, Name = "Lana" }
    };

    public HistoryServiceTests()
    {
        var chatRepository = new Mock<IChatRepository>();
        chatRepository
            .Setup(library => library.Get(It.IsAny<ChatEventFilter>()))
            .Returns(chatEventMock);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(library => library.Get(It.IsAny<IEnumerable<int>>()))
            .Returns(userMock);

        service = new HistoryService(
            chatRepository.Object,
            userRepository.Object
        );
    }

    [Fact]
    public void ShouldShowDefaultChat()
    {
        var result = service.Get(new()).Data;
        result.Count().Should().Be(chatEventMock.Count);
        result.First().Key.Should().Be(chatEventMock.First().DateTime.ToShortTimeString());
        result.Last().Key.Should().Be(chatEventMock.Last().DateTime.ToShortTimeString());
    }

    [Fact]
    public void ShouldShowHourlyAggregation()
    {
        var result = service.Get(new()
        {
            ViewType = HistoryViewType.Aggregation,
            AggregationType = HistoryAggregationType.Hours
        }).Data;

        result.Count().Should().Be(chatEventMock.Select(x => x.DateTime.Hour).Distinct().Count());

        var firstHour = chatEventMock.First().DateTime.Hour;
        result.First().Key.Should().Be(firstHour.ToString());
        result.First().Value.Count().Should().Be(
            chatEventMock.Where(x => x.DateTime.Hour == firstHour).Select(x => x.Type).Distinct().Count()
        );

        var lastHour = chatEventMock.Last().DateTime.Hour;
        result.Last().Key.Should().Be(chatEventMock.Last().DateTime.Hour.ToString());
        result.First().Value.Count().Should().Be(
            chatEventMock.Where(x => x.DateTime.Hour == lastHour).Select(x => x.Type).Distinct().Count()
        );
    }

    private HistoryService PrepareOneEvent(
        ChatEventType type,
        string userName,
        string comment = "",
        int targetUserId = 0
    )
    {
        var chatRepository = new Mock<IChatRepository>();
        chatRepository
            .Setup(library => library.Get(It.IsAny<ChatEventFilter>()))
            .Returns(new List<ChatEvent>() {
                new() {
                    Id = 1,
                    DateTime = new DateTime(2022,12,11,14,0,0),
                    UserId = 1,
                    Type = type,
                    Attributes = new() {
                        Message = comment,
                        TargetUserId = targetUserId
                    }
                },
            });

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(library => library.Get(It.IsAny<IEnumerable<int>>()))
            .Returns(new List<User>() {
                new() { Id = 1, Name = userName },
            });

        return new HistoryService(
            chatRepository.Object,
            userRepository.Object
        );
    }

    [Theory]
    [InlineData(ChatEventType.EnterTheRoom, "Bob", "Bob enters the room")]
    [InlineData(ChatEventType.LeaveTheRoom, "Bob", "Bob leaves the room")]
    public void ShouldRenderEnterLeaveRight(ChatEventType type, string userName, string resultString)
    {
        var service = PrepareOneEvent(type, userName);
        var result = service.Get(new()).Data;
        result.First().Value.First().Should().Be(resultString);
    }

    [Theory]
    [InlineData("Bob", "haha", "Bob comments: haha")]
    public void ShouldRenderCommentRight(string userName, string comment, string resultString)
    {
        var service = PrepareOneEvent(ChatEventType.Comment, userName, comment: comment);
        var result = service.Get(new()).Data;
        result.First().Value.First().Should().Be(resultString);
    }

    [Theory]
    [InlineData("Bob", "Bob high-fives Bob")]
    public void ShouldRenderHighFiveRight(string userName, string resultString)
    {
        var service = PrepareOneEvent(ChatEventType.HighFiveAnotherUser, userName, targetUserId: 1);
        var result = service.Get(new()).Data;
        result.First().Value.First().Should().Be(resultString);
    }

    // Sorry, I don't have time to write tests for aggregation render
    // but they should be there
}
