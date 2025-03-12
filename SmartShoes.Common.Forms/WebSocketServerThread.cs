using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using WebSocketSharp.Server;
using WebSocketSharp;
using System.IO;

public class WebSocketServerThread
{
	// 싱글톤 인스턴스
	private static WebSocketServerThread _instance;
	private static readonly object _lock = new object();

	// 싱글톤 인스턴스 접근자
	public static WebSocketServerThread Instance
	{
		get
		{
			if (_instance == null)
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new WebSocketServerThread();
					}
				}
			}
			return _instance;
		}
	}

	private WebSocketServer wss;
	private Dictionary<string, ServerBehavior> connectedClients = new Dictionary<string, ServerBehavior>();
	private Action<string> logCallback;
	private string _host;
	private int _port;

	// 서버 실행 상태를 추적하는 변수 추가
	private bool _isRunning = false;

	public Action<string> OnClientConnected { get; set; }

	// 기본 생성자를 private으로 변경
	private WebSocketServerThread()
	{
	}

	// 초기화 메서드 추가
	public void Initialize(string host, int port)
	{
        // 이미 실행 중인 경우 무시
        if (_isRunning)
        {
            LogMessage("WebSocket 서버가 이미 실행 중입니다.");
            return;
        }
        _host = host;
		_port = port;
		wss = new WebSocketServer($"ws://{host}:{port}");
		wss.AddWebSocketService<ServerBehavior>("/smartShoes/ws/chat", () =>
		{
			var behavior = new ServerBehavior();
			behavior.SetHandlers(OnNewClient, OnClientLeft, OnMessageReceived);
			return behavior;
		});
	}

	public void Start()
	{
		if (wss == null)
		{
			LogMessage("WebSocket 서버가 초기화되지 않았습니다. Initialize 메서드를 먼저 호출하세요.");
			return;
		}

		// 이미 실행 중인 경우 무시
		if (_isRunning)
		{
			LogMessage("WebSocket 서버가 이미 실행 중입니다.");
			return;
		}

		wss.Start();
		_isRunning = true;
		LogMessage("WebSocket 서버가 시작되었습니다.");
	}

	public void Stop()
	{
		if (!_isRunning)
		{
			LogMessage("WebSocket 서버가 이미 중지되었습니다.");
			return;
		}

		wss.Stop();
		_isRunning = false;
		LogMessage("WebSocket 서버가 중지되었습니다.");
	}

	public void SetLogCallback(Action<string> callback)
	{
		logCallback = callback;
	}

	private void LogMessage(string message)
	{
		logCallback?.Invoke(message);
		Console.WriteLine(message);
	}

	private void OnNewClient(WebSocketBehavior behavior)
	{
		try
		{
			LogMessage("새 클라이언트가 연결되었습니다.");
			//behavior.Context.WebSocket.Send("ID를 할당해주세요.");
		}
		catch (Exception ex)
		{
			LogMessage($"새 클라이언트 처리 중 오류 발생: {ex.Message}");
		}
	}

	private void OnClientLeft(WebSocketBehavior behavior)
	{
		try
		{
			var session = behavior.Context;
			string cameraId = session.Headers["camera_id"];
			if (!string.IsNullOrEmpty(cameraId) && connectedClients.ContainsKey(cameraId))
			{
				connectedClients.Remove(cameraId);
				LogMessage($"클라이언트({cameraId})가 연결을 끊었습니다.");
			}
		}
		catch (Exception ex)
		{
			LogMessage($"클라이언트 연결 종료 처리 중 오류 발생: {ex.Message}");
		}
	}

	private void OnMessageReceived(WebSocketBehavior behavior, string message)
	{
		try
		{
			var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
			if (data != null && data.TryGetValue("statusCode", out var statusCode))
			{
				if (statusCode == "connect")
				{
					HandleConnect(behavior as ServerBehavior, data);
				}
				else
				{
					HandleMessage(behavior as ServerBehavior, data);
				}
			}
			else
			{
				LogMessage("잘못된 메시지 형식 또는 null 메시지: " + message);
			}
		}
		catch (JsonException jsonEx)
		{
			LogMessage("JSON 파싱 오류: " + jsonEx.Message);
		}
		catch (Exception ex)
		{
			LogMessage("메시지 처리 중 오류 발생: " + ex.Message);
		}
	}

	private void HandleConnect(ServerBehavior behavior, Dictionary<string, string> data)
	{
		try
		{
			if (behavior == null)
			{
				LogMessage("HandleConnect: behavior가 null입니다.");
				return;
			}

			if (data == null)
			{
				LogMessage("HandleConnect: data가 null입니다.");
				return;
			}

			if (data.TryGetValue("sender", out var cameraId))
			{
				if (string.IsNullOrEmpty(cameraId))
				{
					LogMessage("HandleConnect: cameraId가 null이거나 비어있습니다.");
					return;
				}

				if (connectedClients.ContainsKey(cameraId))
				{
					var response = new { sender = "server", statusCode = "error", message = "ID가 이미 사용 중입니다." };
					behavior.Context.WebSocket.Send(JsonConvert.SerializeObject(response));
				}
				else
				{
					behavior.Context.Headers.Add("camera_id", cameraId);
					connectedClients[cameraId] = behavior;
					LogMessage($"ID {cameraId}로 클라이언트가 연결되었습니다.");
					var welcomeMessage = new { sender = "server", statusCode = "connect", message = $"" };
					behavior.Context.WebSocket.Send(JsonConvert.SerializeObject(welcomeMessage));

					// 클라이언트 연결됨을 알리는 콜백 호출
					OnClientConnected?.Invoke($"{cameraId}");
				}
			}
			else
			{
				LogMessage("HandleConnect: sender 키가 데이터에 없습니다.");
			}
		}
		catch (Exception ex)
		{
			LogMessage($"클라이언트 연결 처리 중 오류 발생: {ex.Message}");
		}
	}

	private void HandleMessage(ServerBehavior behavior, Dictionary<string, string> data)
	{
		try
		{
			string cameraId = behavior.Context.Headers["camera_id"];
			if (!string.IsNullOrEmpty(cameraId))
			{
				LogMessage($"클라이언트 {cameraId}로부터 메시지 수신: {JsonConvert.SerializeObject(data)}");
				// 추가적인 메시지 처리 로직 구현
			}
			else
			{
				LogMessage("클라이언트 ID가 아직 할당되지 않았습니다.");
			}
		}
		catch (Exception ex)
		{
			LogMessage($"메시지 처리 중 오류 발생: {ex.Message}");
		}
	}

	public void BroadcastMessage(string signal, string folderName)
	{
		// signal : start/stop
		// folderName : 폴더명
		try
		{
			var welcomeMessage = new { sender = "server", statusCode = signal, message = string.Concat(folderName.Split(Path.GetInvalidFileNameChars())) };
			string jsonData = JsonConvert.SerializeObject(welcomeMessage);
			foreach (var client in connectedClients.Values)
			{
				client.Context.WebSocket.Send(jsonData);
			}
		}
		catch (Exception ex)
		{
			LogMessage($"브로드캐스트 메시지 전송 중 오류 발생: {ex.Message}");
		}
	}
}

public class ServerBehavior : WebSocketBehavior
{
	private Action<WebSocketBehavior> onNewClient;
	private Action<WebSocketBehavior> onClientLeft;
	private Action<WebSocketBehavior, string> onMessageReceived;

	public void SetHandlers(
		Action<WebSocketBehavior> newClientHandler,
		Action<WebSocketBehavior> clientLeftHandler,
		Action<WebSocketBehavior, string> messageReceivedHandler)
	{
		onNewClient = newClientHandler;
		onClientLeft = clientLeftHandler;
		onMessageReceived = messageReceivedHandler;
	}

	protected override void OnOpen()
	{
		onNewClient?.Invoke(this);
	}

	protected override void OnClose(CloseEventArgs e)
	{
		onClientLeft?.Invoke(this);
	}

	protected override void OnMessage(MessageEventArgs e)
	{
		if (e.IsText)
		{
			onMessageReceived?.Invoke(this, e.Data);
		}
	}
}