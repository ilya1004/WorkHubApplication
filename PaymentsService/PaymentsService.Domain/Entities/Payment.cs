using PaymentsService.Domain.Enums;
using PaymentsService.Domain.Primitives;

namespace PaymentsService.Domain.Entities;

public class Payment : Entity
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PaymentStatus Status { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}