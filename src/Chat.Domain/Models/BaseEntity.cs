namespace Chat.Domain.Models;

public record BaseEntity
{
    // I used int to make it easier to mock objects, but better use Guid
    public int Id { get; init; }
}
