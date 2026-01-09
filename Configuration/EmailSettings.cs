namespace MedManagerApi.Configuration;

public class EmailSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    
    // SMTP Settings (works with Brevo, Mailgun, Gmail, etc.)
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    
    // Optional: Frontend URL for email links (if different from API)
    public string? FrontendUrl { get; set; }
    
    // Optional: Base URL for email verification links (if empty, uses Request URL)
    public string? VerificationBaseUrl { get; set; }
}
