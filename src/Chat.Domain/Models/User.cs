namespace Chat.Domain.Models;

public record User : BaseEntity
{
    public required string Name { get; init; }
}
