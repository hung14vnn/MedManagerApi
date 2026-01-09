using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MedManagerApi.Configuration;

namespace MedManagerApi.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailVerificationAsync(string email, string firstName, string verificationLink)
    {
        var subject = "Verify Your Email - MedManager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4CAF50;'>Welcome to MedManager!</h2>
                    <p>Hi {firstName},</p>
                    <p>Thank you for registering. Please verify your email address by clicking the link below:</p>
                    <p style='margin: 30px 0;'>
                        <a href='{verificationLink}' 
                           style='background-color: #4CAF50; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 4px; display: inline-block;'>
                            Verify Email
                        </a>
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        If you didn't create this account, please ignore this email.
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        Link: {verificationLink}
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string firstName, string resetLink)
    {
        var subject = "Reset Your Password - MedManager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #FF9800;'>Password Reset Request</h2>
                    <p>Hi {firstName},</p>
                    <p>You requested to reset your password. Click the link below to proceed:</p>
                    <p style='margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #FF9800; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 4px; display: inline-block;'>
                            Reset Password
                        </a>
                    </p>
                    <p style='color: #d32f2f; font-weight: bold;'>
                        This link will expire in 1 hour.
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        If you didn't request this, please ignore this email and your password will remain unchanged.
                    </p>
                    <p style='color: #666; font-size: 12px;'>
                        Link: {resetLink}
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendLoginNotificationAsync(string email, string firstName)
    {
        var subject = "New Login to Your Account - MedManager";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2196F3;'>New Login Detected</h2>
                    <p>Hi {firstName},</p>
                    <p>We detected a new login to your MedManager account.</p>
                    <p><strong>Time:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    <p style='color: #666; font-size: 12px;'>
                        If this wasn't you, please reset your password immediately.
                    </p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName)
    {
        var subject = "Welcome to MedManager!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4CAF50;'>Welcome to MedManager!</h2>
                    <p>Hi {firstName},</p>
                    <p>Your email has been verified successfully. You can now access all features of MedManager.</p>
                    <p>Thank you for joining us!</p>
                </div>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Attempting to send email to {Email} with subject: {Subject}", toEmail, subject);
            _logger.LogDebug("SMTP Config - Host: {Host}, Port: {Port}, User: {User}, SSL: {SSL}", 
                _emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.SmtpUser, _emailSettings.EnableSsl);
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            // For Brevo SMTP on port 587, we need StartTls
            _logger.LogDebug("Connecting to SMTP server...");
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            
            _logger.LogDebug("Authenticating...");
            if (!string.IsNullOrEmpty(_emailSettings.SmtpUser) && !string.IsNullOrEmpty(_emailSettings.SmtpPassword))
            {
                await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);
            }
            else
            {
                _logger.LogWarning("SMTP credentials are empty - attempting to send without authentication");
            }
            
            _logger.LogDebug("Sending email...");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (AuthenticationException authEx)
        {
            _logger.LogError(authEx, "SMTP Authentication failed for {Email}. Check SMTP username and password.", toEmail);
            throw new Exception($"Email authentication failed: {authEx.Message}", authEx);
        }
        catch (SmtpCommandException smtpEx)
        {
            _logger.LogError(smtpEx, "SMTP command failed for {Email}. StatusCode: {StatusCode}", toEmail, smtpEx.StatusCode);
            throw new Exception($"SMTP error: {smtpEx.Message}", smtpEx);
        }
        catch (SmtpProtocolException protocolEx)
        {
            _logger.LogError(protocolEx, "SMTP protocol error for {Email}", toEmail);
            throw new Exception($"SMTP protocol error: {protocolEx.Message}", protocolEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email to {Email}. Error: {ErrorMessage}", toEmail, ex.Message);
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }
}
