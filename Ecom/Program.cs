using Ecom.Commands;
using Ecom.DbContext;

//using Ecom.Consumers;
using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static MassTransit.Logging.OperationName;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Newtonsoft.Json;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});


builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<DataContext>();

builder.Services.AddSingleton<ChatService>();


var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true).AllowCredentials());

app.UseAuthorization();

app.MapControllers();


app.MapGet("/ws", async (HttpContext context, ChatService chatService) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await chatService.HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected a WebSocket request");
    }
});

app.Run();

public class ChatService
{
    private readonly List<WebSocket> _sockets = new();

    public async Task HandleWebSocketConnection(WebSocket socket)
    {
        _sockets.Add(socket);
        var buffer = new byte[1024 * 2];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
                break;
            }

            foreach (var s in _sockets)
            {
                await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
            }
        }
        _sockets.Remove(socket);
    }
}
