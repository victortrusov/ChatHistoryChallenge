namespace Chat.Application.Services.History;

public interface IHistoryService
{
    /// <summary>
    /// Returns chat history by filters and view type
    /// </summary>
    HistoryResponse Get(HistoryRequest request);
}

