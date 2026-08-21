using backend.Endpoints;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var supabaseUrl = builder.Configuration["Supabase:Url"];

if (string.IsNullOrWhiteSpace(supabaseUrl))
{
    throw new InvalidOperationException(
        "Supabase:Url is required. Configure it with dotnet user-secrets before starting the backend.");
}

var supabaseAuthority = $"{supabaseUrl.TrimEnd('/')}/auth/v1";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = supabaseAuthority;
        options.Audience = "authenticated";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = supabaseAuthority,
            ValidAudience = "authenticated",
            NameClaimType = "email",
            RoleClaimType = "role",
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<EmployeeManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IWorkplaceMemberService, WorkplaceMemberService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IAvailabilitySlotService, AvailabilitySlotService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
builder.Services.AddScoped<IWorkspaceDashboardService, WorkspaceDashboardService>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Hello World!").AllowAnonymous();
app.MapAppUserEndpoints();
app.MapWorkplaceMemberEndpoints();
app.MapInvitationEndpoints();
app.MapAvailabilitySlotEndpoints();
app.MapShiftEndpoints();
app.MapWorkLogEndpoints();
app.MapWorkplaceEndpoints();
app.MapDashboardEndpoints();

app.Run();
