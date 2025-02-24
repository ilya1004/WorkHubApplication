namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveMessage(string text);
}