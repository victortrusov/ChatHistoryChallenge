namespace Chat.Application.Services.History;

public interface IHistoryService
{
    HistoryResponse Get(HistoryRequest request);
}

