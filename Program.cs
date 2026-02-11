using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MedManagerApi.Data;
using MedManagerApi.Models;
using MedManagerApi.Services;
using MedManagerApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables (they override appsettings)
builder.Configuration.AddEnvironmentVariables();

// Configure forwarded headers for proxy support (including Cloudflare)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                               ForwardedHeaders.XForwardedProto | 
                               ForwardedHeaders.XForwardedHost;
    
    // Clear defaults to trust Cloudflare and other proxies
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    
    // Cloudflare specific: Use CF-Connecting-IP for real client IP
    options.ForwardedForHeaderName = "CF-Connecting-IP";
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedManager API",
        Version = "v1",
        Description = "MedManager API with JWT Authentication"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure PostgreSQL Database
builder.Services.AddDbContext<MedManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<MedManagerDbContext>()
.AddDefaultTokenProviders();

// Configure token lifespan (for email verification and password reset)
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}");
            logger.LogInformation("JWT Token validated successfully. Claims: {Claims}", string.Join(", ", claims ?? Array.Empty<string>()));
            return Task.CompletedTask;
        }
    };
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Configure settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register services
builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<IDiseaseService, DiseaseService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ISearchLogService, SearchLogService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IDosageFormService, DosageFormService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IMechanismService, MechanismService>();
builder.Services.AddScoped<ISearchLogCleanupService, SearchLogCleanupService>();

// Register background services
builder.Services.AddHostedService<MedManagerApi.BackgroundServices.SearchLogCleanupBackgroundService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Use forwarded headers BEFORE other middleware
app.UseForwardedHeaders();

// Log incoming requests for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Incoming request: {Method} {Path} from {RemoteIp}", 
        context.Request.Method, 
        context.Request.Path, 
        context.Connection.RemoteIpAddress);
    
    logger.LogDebug("Request Headers: Scheme={Scheme}, Host={Host}, Authorization={HasAuth}", 
        context.Request.Scheme,
        context.Request.Host,
        context.Request.Headers.ContainsKey("Authorization") ? "Present" : "Missing");
    
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        logger.LogDebug("Authorization header: {AuthHeader}", 
            authHeader.Length > 20 ? authHeader.Substring(0, 20) + "..." : authHeader);
    }
    
    await next();
});

// Log configuration sources (helpful for debugging)
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("JWT Key configured: {HasKey}", !string.IsNullOrEmpty(builder.Configuration["Jwt:Key"]));
startupLogger.LogInformation("SMTP Host configured: {Host}", builder.Configuration["EmailSettings:SmtpHost"]);
startupLogger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);

// Seed roles and super admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Ensure database schema is applied before seeding
        var dbContext = services.GetRequiredService<MedManagerDbContext>();
        startupLogger.LogInformation("Applying pending migrations (if any)...");
        await dbContext.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        
        await RoleSeeder.SeedRolesAsync(roleManager);
        
        // Use environment variable for super admin credentials in production
        var superAdminEmail = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL") 
            ?? "superadmin@medmanager.com";
        var superAdminPassword = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD") 
            ?? "SuperAdmin@123";
            
        await RoleSeeder.SeedSuperAdminAsync(userManager, superAdminEmail, superAdminPassword);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Production error handling
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedManager API V1");
    c.RoutePrefix = string.Empty; // serve swagger at app root
});

// Only enable HTTPS redirection when HTTPS is configured in URLs
var configuredUrls = builder.Configuration["ASPNETCORE_URLS"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrEmpty(configuredUrls) && configuredUrls.Contains("https", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}
else
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("HTTPS redirection is disabled because no HTTPS URL is configured (ASPNETCORE_URLS does not contain 'https').");
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
