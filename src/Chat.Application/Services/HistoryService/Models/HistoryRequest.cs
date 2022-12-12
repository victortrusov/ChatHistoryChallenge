namespace Chat.Application.Services.History;

public record HistoryRequest
{
    public HistoryViewType ViewType { get; init; } = HistoryViewType.Default;
    public HistoryAggregationType? AggregationType { get; init; }
    public DateTime? DateTimeMin { get; init; }
    public DateTime? DateTimeMax { get; init; }
}
