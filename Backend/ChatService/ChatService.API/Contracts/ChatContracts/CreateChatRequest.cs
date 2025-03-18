namespace ChatService.API.Contracts.ChatContracts;

public sealed record CreateChatRequest(Guid EmployerId, Guid FreelancerId, Guid ProjectId);