using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;
using MedManagerApi.Configuration;
using MedManagerApi.DTOs;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;
    private readonly EmailSettings _emailSettings;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<AuthController> logger,
        IOptions<EmailSettings> emailSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
        _emailSettings = emailSettings.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "User with this email already exists" });

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        // Assign default "User" role
        await _userManager.AddToRoleAsync(user, AppRoles.User);

        // Generate email verification token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        // Use configured VerificationBaseUrl if available, otherwise use Request URL
        var baseUrl = !string.IsNullOrEmpty(_emailSettings.VerificationBaseUrl)
            ? _emailSettings.VerificationBaseUrl
            : $"{Request.Scheme}://{Request.Host}";
            
        var verificationLink = $"{baseUrl}/api/auth/verify-email?email={user.Email}&token={encodedToken}";
        

        // Send verification email
        var emailSent = false;
        var emailError = string.Empty;
        
        try
        {
            await _emailService.SendEmailVerificationAsync(user.Email, user.FirstName ?? "User", verificationLink);
            emailSent = true;
        }
        catch (Exception ex)
        {
            emailError = ex.Message;
        }

        return Ok(new 
        { 
            message = emailSent 
                ? "Registration successful. Please check your email to verify your account."
                : "Registration successful, but we couldn't send the verification email. Please contact support or try resending.",
            email = user.Email,
            emailSent,
            emailError = emailSent ? null : emailError
        });
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Invalid verification link" });

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest(new { message = "User not found" });

        if (user.EmailConfirmed)
            return Ok(new { message = "Email already verified" });

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
            return BadRequest(new { message = "Invalid or expired token", errors = result.Errors.Select(e => e.Description) });

        // Send welcome email
        try
        {
            await _emailService.SendWelcomeEmailAsync(user.Email!, user.FirstName ?? "User");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
        }

        return Ok(new { message = "Email verified successfully. You can now login." });
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Ok(new { message = "If the email exists, a verification link has been sent." });

        if (user.EmailConfirmed)
            return BadRequest(new { message = "Email is already verified" });

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        var baseUrl = !string.IsNullOrEmpty(_emailSettings.VerificationBaseUrl)
            ? _emailSettings.VerificationBaseUrl
            : $"{Request.Scheme}://{Request.Host}";
            
        var verificationLink = $"{baseUrl}/api/auth/verify-email?email={user.Email}&token={encodedToken}";

        try
        {
            await _emailService.SendEmailVerificationAsync(user.Email!, user.FirstName ?? "User", verificationLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend verification email to {Email}", user.Email);
        }

        return Ok(new { message = "If the email exists, a verification link has been sent." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized(new { message = "Invalid email or password" });

        if (!user.EmailConfirmed)
            return Unauthorized(new { message = "Please verify your email before logging in" });

        if (!user.IsActive)
            return Unauthorized(new { message = "Your account has been deactivated" });

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Generate JWT token
        var token = await _tokenService.GenerateJwtTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        // Send login notification (optional - async fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendLoginNotificationAsync(user.Email!, user.FirstName ?? "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send login notification to {Email}", user.Email);
            }
        });

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles.ToList(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        
        // Always return success to prevent email enumeration
        if (user == null || !user.EmailConfirmed)
            return Ok(new { message = "If the email exists, a password reset link has been sent." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        var baseUrl = !string.IsNullOrEmpty(_emailSettings.VerificationBaseUrl)
            ? _emailSettings.VerificationBaseUrl
            : $"{Request.Scheme}://{Request.Host}";
            
        var resetLink = $"{baseUrl}/api/auth/reset-password?email={user.Email}&token={encodedToken}";

        try
        {
            await _emailService.SendPasswordResetAsync(user.Email!, user.FirstName ?? "User", resetLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
        }

        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request" });

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(new { message = "Invalid or expired token", errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "Password reset successfully. You can now login with your new password." });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!AppRoles.GetAllRoles().Contains(dto.Role))
            return BadRequest(new { message = $"Invalid role. Valid roles are: {string.Join(", ", AppRoles.GetAllRoles())}" });

        if (await _userManager.IsInRoleAsync(user, dto.Role))
            return BadRequest(new { message = $"User already has the {dto.Role} role" });

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = $"Role {dto.Role} assigned to {user.Email} successfully" });
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!await _userManager.IsInRoleAsync(user, dto.Role))
            return BadRequest(new { message = $"User does not have the {dto.Role} role" });

        var result = await _userManager.RemoveFromRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = $"Role {dto.Role} removed from {user.Email} successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Email,
            user.FirstName,
            user.LastName,
            user.EmailConfirmed,
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt,
            Roles = roles
        });
    }

    // DEBUG ENDPOINT - Check token claims
    [Authorize]
    [HttpGet("debug-claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var isInSuperAdminRole = User.IsInRole(AppRoles.SuperAdmin);
        var hasRoleClaim = User.HasClaim(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == AppRoles.SuperAdmin);
        
        return Ok(new
        {
            claims,
            isInSuperAdminRole,
            hasRoleClaim,
            identity = new
            {
                isAuthenticated = User.Identity?.IsAuthenticated,
                authenticationType = User.Identity?.AuthenticationType,
                name = User.Identity?.Name
            }
        });
    }
}
