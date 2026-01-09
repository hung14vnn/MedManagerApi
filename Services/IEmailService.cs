namespace MedManagerApi.Services;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string firstName, string verificationLink);
    Task SendPasswordResetAsync(string email, string firstName, string resetLink);
    Task SendLoginNotificationAsync(string email, string firstName);
    Task SendWelcomeEmailAsync(string email, string firstName);
}
