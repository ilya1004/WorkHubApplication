using System.Text.Json.Serialization;

namespace ChatService.API.Contracts.ChatContracts;

public class GetAllChatsRequest
{
    [JsonPropertyName("PageNo")]
    public int PageNo { get; set; }

    [JsonPropertyName("PageSize")]
    public int PageSize { get; set; }
}
