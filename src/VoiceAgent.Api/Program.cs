using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VoiceAgent.Api.Realtime;
using VoiceAgent.Application.Abstractions;
using VoiceAgent.Application.Common.Behaviors;
using VoiceAgent.Application.Memory;
using VoiceAgent.Application.Tools;
using VoiceAgent.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VoiceAgent.Application.Calls.Commands.StartCallSessionCommand).Assembly));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddSingleton<IMemoryManager, ContextMemoryManager>();
builder.Services.AddSingleton<ToolRegistry>();
builder.Services.AddSingleton<ToolExecutionEngine>();
builder.Services.AddSingleton<ICallMonitoringNotifier, SignalRCallMonitoringNotifier>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<VoiceAgent.Api.Hubs.CallMonitoringHub>("/hubs/calls");

app.Run();
