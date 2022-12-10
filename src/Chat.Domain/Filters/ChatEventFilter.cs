namespace Chat.Domain.Filters;

public record ChatEventFilter
{
    public DateTime? DateTimeMin { get; init; }
    public DateTime? DateTimeMax { get; init; }
}
