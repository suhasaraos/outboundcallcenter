using Azure.Communication;
using Azure.Communication.CallAutomation;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read configuration values
var acsConnectionString = builder.Configuration["AzureCommunicationServices:ConnectionString"];
var acsPhonenumber = builder.Configuration["AzureCommunicationServices:PhoneNumber"];
var targetPhonenumber = builder.Configuration["AzureCommunicationServices:TargetPhoneNumber"];
var callbackUriHost = builder.Configuration["AzureCommunicationServices:CallbackUriHost"];

CallAutomationClient callAutomationClient = new CallAutomationClient(acsConnectionString);
var app = builder.Build();

app.MapPost("/outboundCall", async (ILogger<Program> logger) =>
{
    PhoneNumberIdentifier target = new PhoneNumberIdentifier(targetPhonenumber);
    PhoneNumberIdentifier caller = new PhoneNumberIdentifier(acsPhonenumber);
    CallInvite callInvite = new CallInvite(target, caller);

    var callbackUri = new Uri(new Uri(callbackUriHost), $"/api/callbacks/{Guid.NewGuid()}?targetId={target.RawId}");
    logger.LogInformation($"Callback Url: {callbackUri}");
    var websocketUri = callbackUriHost.Replace("https", "wss") + "/ws";
    logger.LogInformation($"WebSocket Url: {callbackUri}");

    var mediaStreamingOptions = new MediaStreamingOptions(
            new Uri(websocketUri),
            MediaStreamingContent.Audio,
            MediaStreamingAudioChannel.Mixed,
            startMediaStreaming: true
            )
    {
        EnableBidirectional = true,
        AudioFormat = AudioFormat.Pcm24KMono
    };

    var createCallOptions = new CreateCallOptions(callInvite, callbackUri)
    {
        MediaStreamingOptions = mediaStreamingOptions,
    };

    CreateCallResult createCallResult = await callAutomationClient.CreateCallAsync(createCallOptions);

    logger.LogInformation($"Created call with connection id: {createCallResult.CallConnectionProperties.CallConnectionId}");
});

// api to handle call back events
app.MapPost("/api/callbacks/{contextId}", async (
    [FromBody] CloudEvent[] cloudEvents,
    [FromRoute] string contextId,
    [Required] string callerId,
    ILogger<Program> logger) =>
{
    foreach (var cloudEvent in cloudEvents)
    {
        CallAutomationEventBase @event = CallAutomationEventParser.Parse(cloudEvent);
        logger.LogInformation($"Event received: {JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = true })}");
    }

    return Results.Ok();
});

app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            try
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var mediaService = new AcsMediaStreamingHandler(webSocket, builder.Configuration);

                // Set the single WebSocket connection
                await mediaService.ProcessWebSocketAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception received {ex}");
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
