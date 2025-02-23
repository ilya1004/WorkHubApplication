using ChatService.Domain.Enums;
using ChatService.Domain.Primitives;

namespace ChatService.Domain.Entities;

public class Message : Entity
{
    public string? Text { get; set; }
    public Guid? FileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid ChatId { get; set; }
    public MessageType Type { get; set; }
}