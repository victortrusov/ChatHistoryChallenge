namespace Chat.Domain.Models;

public record ChatEvent
{
    public Guid Id { get; init; }
    public DateTime DateTime { get; init; }
    public Guid UserId { get; init; }
    public ChatEventType Type { get; init; }
    public ChatEventAttributes Attributes { get; init; } = new();
}

public record ChatEventAttributes
{
    public Guid? TargetUserId { get; init; }
    public string? Message { get; init; }
}
