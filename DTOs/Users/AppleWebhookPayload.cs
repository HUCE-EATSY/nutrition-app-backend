using System;

namespace nutrition_app_backend.DTOs.Users;

public class AppleWebhookPayload
{
    public string NotificationType { get; set; } = null!;
    public string Subtype { get; set; } = null!;
    public string TransactionId { get; set; } = null!;
    public string OriginalTransactionId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public DateTime ExpiresDate { get; set; }
    public Guid? AppAccountToken { get; set; }
}
