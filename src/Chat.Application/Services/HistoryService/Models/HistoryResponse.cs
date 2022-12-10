namespace Chat.Application.Services.History;

public record HistoryResponse
{
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Data { get; init; } = new List<KeyValuePair<string, IEnumerable<string>>>();
}
