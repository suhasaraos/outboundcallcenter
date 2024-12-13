# Outbound Call Center

This solution is a .NET application that uses Azure Communication Services (ACS) and Azure OpenAI to handle outbound calls and media streaming. The application is designed to initiate outbound calls, handle callback events, and process media streaming through WebSockets.
It uses the preview of bidirectional audio streaming for Azure Communication Services Call Automation SDK, which unlocks new possibilities for developers and businesses. This capability results in seamless, low-latency, real-time communication when integrated with services like Azure Open AI and the real-time voice APIs, significantly enhancing how businesses can build and deploy conversational AI solutions.

**References**
1. [Azure Communication Services Bidirectional preview](https://techcommunity.microsoft.com/blog/azurecommunicationservicesblog/ignite-2024-bidirectional-real-time-audio-streaming-with-azure-communication-ser/4304588)
2. [Azure OpenAI realtime audio preview](https://learn.microsoft.com/en-us/azure/ai-services/openai/realtime-audio-reference)


## License

## Features

- **Outbound Call Initiation**: Initiates outbound calls using Azure Communication Services.
- **Callback Handling**: Handles callback events from ACS.
- **Media Streaming**: Streams media content through WebSockets and processes it using Azure OpenAI.

## Prerequisites

- .NET 8 SDK
- Azure Communication Services account
- Azure OpenAI account

## Configuration

The application configuration is stored in the `appsettings.json` file. You need to replace the placeholders with your actual Azure Communication Services and Azure OpenAI credentials.


## Project Structure

- **Program.cs**: The main entry point of the application. It sets up the web application, reads configuration values, and defines the endpoints for outbound calls and callback handling.
- **AcsMediaStreamingHandler.cs**: Handles media streaming from ACS and forwards it to Azure OpenAI for processing.

## Running the Application

1. Clone the repository.
2. Update the `appsettings.json` file with your Azure Communication Services and Azure OpenAI credentials.
3. Open the solution in Visual Studio 2022.
4. Build and run the application.

## Endpoints

- **POST /outboundCall**: Initiates an outbound call.
- **POST /api/callbacks/{contextId}**: Handles callback events from ACS.

## Dependencies

The project uses the following NuGet packages:

- `Azure.AI.OpenAI` (Version 2.1.0-beta.2)
- `Azure.Communication.CallAutomation` (Version 1.4.0-beta.1)
- `Azure.Messaging.EventGrid` (Version 4.28.0)
- `Swashbuckle.AspNetCore` (Version 7.2.0)

## License

This project is licensed under the MIT License.