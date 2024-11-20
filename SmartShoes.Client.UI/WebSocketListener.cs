using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WebSocketListener
{
	private readonly HttpListener _listener;
	private readonly ConcurrentDictionary<WebSocket, string> _connectedClients = new ConcurrentDictionary<WebSocket, string>();

	public WebSocketListener(string uriPrefix)
	{
		_listener = new HttpListener();
		_listener.Prefixes.Add(uriPrefix);
	}

	public async Task StartAsync()
	{
		_listener.Start();
		Console.WriteLine("WebSocket server started...");

		while (true)
		{
			HttpListenerContext context = await _listener.GetContextAsync();

			if (context.Request.IsWebSocketRequest)
			{
				await HandleWebSocketConnectionAsync(context);
			}
			else
			{
				context.Response.StatusCode = 400;
				context.Response.Close();
			}
		}
	}

	private async Task HandleWebSocketConnectionAsync(HttpListenerContext context)
	{
		WebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
		WebSocket webSocket = wsContext.WebSocket;

		Console.WriteLine("New client connected.");

		var buffer = new byte[1024 * 4];
		WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

		while (result.MessageType != WebSocketMessageType.Close)
		{
			var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
			HandleMessage(webSocket, message);

			result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}

		_connectedClients.TryRemove(webSocket, out _);
		await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
		Console.WriteLine("Client disconnected.");
	}

	private void HandleMessage(WebSocket webSocket, string message)
	{
		try
		{
			var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
			if (data.TryGetValue("statusCode", out var statusCode) && statusCode.ToString() == "connect")
			{
				HandleConnect(webSocket, data);
			}
			else
			{
				HandleDataMessage(webSocket, data);
			}
		}
		catch (JsonException ex)
		{
			Console.WriteLine($"Error parsing JSON: {ex.Message}");
		}
	}

	private void HandleConnect(WebSocket webSocket, Dictionary<string, object> data)
	{
		if (data.TryGetValue("sender", out var cameraId))
		{
			_connectedClients[webSocket] = cameraId.ToString();
			Console.WriteLine($"Client connected with ID {cameraId}");

			var welcomeMessage = new
			{
				sender = "server",
				statusCode = "connect",
				message = $"Welcome to the server, {cameraId}!"
			};
			SendMessageAsync(webSocket, JsonConvert.SerializeObject(welcomeMessage)).Wait();
		}
	}

	private void HandleDataMessage(WebSocket webSocket, Dictionary<string, object> data)
	{
		if (_connectedClients.TryGetValue(webSocket, out var cameraId))
		{
			Console.WriteLine($"Received message from client {cameraId}: {JsonConvert.SerializeObject(data)}");
			VerifyDataFolder(cameraId, data);
		}
		else
		{
			Console.WriteLine("Client ID not assigned yet.");
		}
	}

	private void VerifyDataFolder(string cameraId, Dictionary<string, object> data)
	{
		if (data.TryGetValue("message", out var datetimeStr))
		{
			string folderPath = $"c://camera/{datetimeStr}/camera/{cameraId.Substring(6)}";
			if (System.IO.Directory.Exists(folderPath) && System.IO.Directory.GetFiles(folderPath).Length > 0)
			{
				Console.WriteLine($"Data found in folder: {folderPath}");
			}
			else
			{
				Console.WriteLine($"No data found in folder: {folderPath}");
			}
		}
	}

	private async Task SendMessageAsync(WebSocket webSocket, string message)
	{
		var buffer = Encoding.UTF8.GetBytes(message);
		await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
	}
}
