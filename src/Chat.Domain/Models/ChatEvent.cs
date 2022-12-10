namespace Chat.Domain.Models;

public record ChatEvent : BaseEntity
{
    public DateTime DateTime { get; init; }
    public int UserId { get; init; }
    public ChatEventType Type { get; init; }
    public ChatEventAttributes Attributes { get; init; } = new();
}

public record ChatEventAttributes
{
    public int? TargetUserId { get; init; }
    public string? Message { get; init; }
}
