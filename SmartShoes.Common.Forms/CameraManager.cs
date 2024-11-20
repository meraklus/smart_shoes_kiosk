using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace SmartShoes.Common.Forms
{
	public class CameraManager
	{
		private static CameraManager _instance;
		private WebSocket _webSocket;
		private Dictionary<string, Action<string>> _messageHandlers;

		private CameraManager()
		{
			_messageHandlers = new Dictionary<string, Action<string>>();
		}

		public static CameraManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CameraManager();
				}
				return _instance;
			}
		}

		public void Connect(string url)
		{
			if (_webSocket == null)
			{
				_webSocket = new WebSocket(url);

				_webSocket.OnOpen += (sender, e) =>
				{
					Console.WriteLine("WebSocket connected.");
				};

				_webSocket.OnClose += (sender, e) =>
				{
					Console.WriteLine("WebSocket disconnected.");
					_webSocket = null; // 연결 종료 시 WebSocket 객체를 null로 설정
				};

				_webSocket.OnMessage += (sender, e) =>
				{
					if (e.IsText)
					{
						HandleMessage(e.Data);
					}
				};

				_webSocket.Connect();
			}
		}

		public void Disconnect()
		{
			if (_webSocket != null && _webSocket.IsAlive)
			{
				_webSocket.Close();
				_webSocket = null;
			}
		}

		public void SendMessage(string message)
		{
			if (_webSocket != null && _webSocket.IsAlive)
			{
				_webSocket.Send(message);
			}
		}

		public void RegisterCameraStatusUpdate(Action<string, string> updateStatus)
		{
			RegisterMessageHandler("connect", (message) =>
			{
				var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
				if (data != null && data.TryGetValue("sender", out var cameraId))
				{
					updateStatus(cameraId, "Connected");
				}
			});

			RegisterMessageHandler("disconnect", (message) =>
			{
				var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
				if (data != null && data.TryGetValue("sender", out var cameraId))
				{
					updateStatus(cameraId, "Disconnected");
				}
			});
		}

		public void RegisterMessageHandler(string key, Action<string> handler)
		{
			if (!_messageHandlers.ContainsKey(key))
			{
				_messageHandlers.Add(key, handler);
			}
		}

		public void UnregisterMessageHandler(string key)
		{
			if (_messageHandlers.ContainsKey(key))
			{
				_messageHandlers.Remove(key);
			}
		}

		private void HandleMessage(string message)
		{
			try
			{
				var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
				if (data != null && data.TryGetValue("statusCode", out var statusCode))
				{
					if (_messageHandlers.ContainsKey(statusCode))
					{
						_messageHandlers[statusCode].Invoke(message);
					}
				}
			}
			catch (JsonException jsonEx)
			{
				Console.WriteLine("JSON Parsing Error: " + jsonEx.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error Handling Message: " + ex.Message);
			}
		}


	}
}
