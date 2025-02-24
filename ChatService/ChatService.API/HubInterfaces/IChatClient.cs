namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveTextMessage(string text);
    Task ReceiveFileMessage(IFormFile file);
}