using backend.Endpoints;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeManagerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IWorkplaceMemberService, WorkplaceMemberService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IAvailabilitySlotService, AvailabilitySlotService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => "Hello World!");
app.MapAppUserEndpoints();
app.MapWorkplaceMemberEndpoints();
app.MapInvitationEndpoints();
app.MapAvailabilitySlotEndpoints();
app.MapShiftEndpoints();
app.MapWorkLogEndpoints();
app.MapWorkplaceEndpoints();

app.Run();
