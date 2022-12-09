namespace Chat.Domain.Models;

public record User
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
}
