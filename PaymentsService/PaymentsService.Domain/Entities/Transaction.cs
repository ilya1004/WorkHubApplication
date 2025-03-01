using PaymentsService.Domain.Enums;
using PaymentsService.Domain.Primitives;

namespace PaymentsService.Domain.Entities;

public class Transaction : Entity
{
    public Guid PaymentId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}