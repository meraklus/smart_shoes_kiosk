using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;
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

	// 카메라 데이터를 저장할 리스트 추가 (배열 형태로 저장)
	private List<object> cameraDataList = new List<object>();
	// 마지막으로 파일을 저장한 시간
	private DateTime lastSaveTime = DateTime.MinValue;
	// 마지막으로 데이터를 수신한 시간
	private DateTime lastDataCollectionTime = DateTime.MinValue;
	// 파일 저장 간격 (밀리초)
	private const int SaveIntervalMs = 100; // 100ms마다 저장
	// 데이터 수집 완료로 간주할 시간 간격 (밀리초)
	private const int DataCollectionTimeoutMs = 3000; // 3초 동안 데이터가 없으면 수집 완료로 간주
	// 파일 저장 경로
	private string cameraDataDirectory;
	// 데이터를 제공한 카메라 목록
	private HashSet<string> camerasWithData = new HashSet<string>();
	// 데이터 수집 완료 콜백
	public Action<List<object>, List<string>> OnDataCollectionCompleted;
	// 데이터 수집 중인지 여부
	private bool isCollectingData = false;

	public Action<string> OnClientConnected { get; set; }
	// 클라이언트 연결 해제 콜백 추가
	public Action<string> OnClientDisconnected { get; set; }

	// 기본 생성자를 private으로 변경
	private WebSocketServerThread()
	{
		// 카메라 데이터 저장 디렉토리 초기화
		cameraDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CameraData");
		if (!Directory.Exists(cameraDataDirectory))
		{
			Directory.CreateDirectory(cameraDataDirectory);
		}
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
			if (!string.IsNullOrEmpty(cameraId))
			{
				lock (_lock)
				{
					if (connectedClients.ContainsKey(cameraId))
					{
						connectedClients.Remove(cameraId);
						LogMessage($"클라이언트({cameraId})가 연결을 끊었습니다.");
						
						// 클라이언트 연결 해제 콜백 호출
						OnClientDisconnected?.Invoke(cameraId);
					}
				}
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
			var data = JsonConvert.DeserializeObject<dynamic>(message);
			
			// dynamic 타입은 TryGetValue를 사용할 수 없으므로 직접 속성에 접근
			if (data != null && data.statusCode != null)
			{
				var dataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
				HandleConnect(behavior as ServerBehavior, dataDict);
			}
			// 카메라 데이터 처리 (cameraId와 data 속성이 있는 경우)
			else if (data != null && data.cameraID != null && data.data != null)
			{
				// 카메라 ID 추출
				string cameraId = data.cameraID.ToString();
				
				// data 부분만 추출하여 리스트에 추가
				lock (_lock)
				{
					// data가 배열인 경우 각 항목을 개별적으로 추가
					if (data.data is Newtonsoft.Json.Linq.JArray dataArray)
					{
						int count = 0;
						foreach (var item in dataArray)
						{
							cameraDataList.Add(item);
							count++;
						}
						LogMessage($"카메라 {cameraId}로부터 {count}개의 데이터 항목 수신");
					}
					else // data가 단일 객체인 경우
					{
						cameraDataList.Add(data.data);
						LogMessage($"카메라 {cameraId}로부터 단일 데이터 항목 수신");
					}
				}
				
				// 데이터 수집 상태 업데이트
				UpdateDataCollectionStatus(cameraId);
				
				// 데이터 수집 완료 여부 확인 - 매 데이터 수신마다 확인
				CheckDataCollectionCompleted();
				
				LogMessage($"카메라 {cameraId}로부터 데이터 수신");
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

				bool isAlreadyConnected = false;
				lock (_lock)
				{
					isAlreadyConnected = connectedClients.ContainsKey(cameraId);
				}

				if (isAlreadyConnected)
				{
					var response = new { sender = "server", statusCode = "error", message = "ID가 이미 사용 중입니다." };
					behavior.Context.WebSocket.Send(JsonConvert.SerializeObject(response));
				}
				else
				{
					behavior.Context.Headers.Add("camera_id", cameraId);
					
					lock (_lock)
					{
						connectedClients[cameraId] = behavior;
					}
					
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
		// signal : start/stop/results
		// folderName : 폴더명
		try
		{
			// "results" 신호인 경우 데이터 수집 시작
			if (signal.Equals("results", StringComparison.OrdinalIgnoreCase))
			{
				// 데이터 수집 시작
				StartDataCollection();
			}
			
			var welcomeMessage = new { sender = "server", statusCode = signal, message = string.Concat(folderName.Split(Path.GetInvalidFileNameChars())) };
			string jsonData = JsonConvert.SerializeObject(welcomeMessage);
			
			// 스레드 안전을 위해 연결된 클라이언트 목록의 복사본 사용
			List<ServerBehavior> clients;
			lock (_lock)
			{
				clients = new List<ServerBehavior>(connectedClients.Values);
			}
			
			foreach (var client in clients)
			{
				client.Context.WebSocket.Send(jsonData);
			}
		}
		catch (Exception ex)
		{
			LogMessage($"브로드캐스트 메시지 전송 중 오류 발생: {ex.Message}");
		}
	}

	// 연결된 클라이언트 목록을 반환하는 함수 추가
	public List<string> GetConnectedClients()
	{
		// 스레드 안전을 위해 복사본 반환
		lock (_lock)
		{
			return new List<string>(connectedClients.Keys);
		}
	}

	// 특정 클라이언트가 연결되어 있는지 확인하는 함수 추가
	public bool IsClientConnected(string clientId)
	{
		lock (_lock)
		{
			return connectedClients.ContainsKey(clientId);
		}
	}

	// 서버가 실행 중인지 확인하는 속성 추가
	public bool IsRunning
	{
		get { return _isRunning; }
	}

	// 데이터 수집 완료 여부 확인
	private void CheckDataCollectionCompleted()
	{
		if (!isCollectingData) return;

		// 연결된 모든 클라이언트 ID 목록
		List<string> allClientIds;
		lock (_lock)
		{
			allClientIds = connectedClients.Keys.ToList();
		}

		// 모든 카메라(9대)에서 데이터를 수신했는지 확인
		bool allCamerasProvided = false;
		int expectedCameraCount = 9; // 예상되는 카메라 수 (9대)
		
		lock (_lock)
		{
			// 실제 연결된 카메라가 9대 미만일 경우, 연결된 모든 카메라에서 데이터를 받았는지 확인
			if (allClientIds.Count <= expectedCameraCount)
			{
				allCamerasProvided = camerasWithData.Count >= allClientIds.Count;
			}
			else
			{
				// 9대 이상 연결된 경우, 적어도 9대에서 데이터를 받았는지 확인
				allCamerasProvided = camerasWithData.Count >= expectedCameraCount;
			}
		}

		// 마지막 데이터 수신 후 일정 시간이 지났는지 확인
		TimeSpan elapsed = DateTime.Now - lastDataCollectionTime;
		bool timeoutElapsed = elapsed.TotalMilliseconds >= DataCollectionTimeoutMs;

		// 모든 카메라에서 데이터를 수신했거나, 타임아웃이 발생한 경우
		if (allCamerasProvided || timeoutElapsed)
		{
			// 데이터를 제공하지 않은 카메라 목록
			List<string> missingCameras = allClientIds
				.Where(id => !camerasWithData.Contains(id))
				.ToList();

			LogMessage($"데이터 수집 완료: {(allCamerasProvided ? "모든 카메라에서 데이터 수신" : $"타임아웃 발생 ({elapsed.TotalSeconds:F1}초 경과)")}");
			LogMessage($"연결된 카메라: {allClientIds.Count}대, 데이터 제공 카메라: {camerasWithData.Count}대, 데이터 미제공 카메라: {missingCameras.Count}대");
			
			if (missingCameras.Count > 0)
			{
				LogMessage($"데이터를 제공하지 않은 카메라: {string.Join(", ", missingCameras)}");
			}
			
			// 데이터 수집 완료 후 파일 저장
			SaveAllCameraDataToSingleFile();
			
			// 데이터 수집 완료 콜백 호출
			OnDataCollectionCompleted?.Invoke(cameraDataList, missingCameras);
			
			// 데이터 수집 상태 초기화
			isCollectingData = false;
		}
	}

	// 카메라 데이터를 파일로 저장하는 메서드
	private static readonly object _fileLock = new object(); // 파일 쓰기 작업을 위한 정적 락 객체
	private void SaveCameraDataToFile()
	{
		try
		{
			// 스레드 안전을 위해 데이터 복사
			List<object> dataCopy;
			lock (_lock)
			{
				dataCopy = new List<object>(cameraDataList);
			}
			
			// 데이터가 없으면 저장하지 않음
			if (dataCopy.Count == 0)
			{
				return;
			}
			
			// 고정된 파일명 사용
			string fileName = "merged_data.json";
			string filePath = Path.Combine(cameraDataDirectory, fileName);
			
			// JSON으로 변환
			string jsonData = JsonConvert.SerializeObject(dataCopy, Formatting.Indented);
			
			// 파일 쓰기 작업에 락을 걸어 한 번에 하나의 스레드만 접근하도록 함
			int retryCount = 0;
			const int maxRetries = 5; // 최대 재시도 횟수 증가
			bool success = false;
			
			while (!success && retryCount < maxRetries)
			{
				try
				{
					lock (_fileLock) // 모든 스레드 간 공유되는 락
					{
						// FileStream을 사용하여 더 세밀한 파일 제어
						using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
						using (StreamWriter writer = new StreamWriter(fs))
						{
							writer.Write(jsonData);
							writer.Flush();
						}
					}
					success = true;
					LogMessage($"카메라 데이터를 파일로 저장했습니다: {filePath}");
				}
				catch (IOException ex)
				{
					retryCount++;
					int waitTime = 200 * retryCount; // 점점 더 오래 대기
					LogMessage($"카메라 데이터 파일 저장 중 충돌 발생, 재시도 {retryCount}/{maxRetries}: {ex.Message}. {waitTime}ms 후 재시도합니다.");
					System.Threading.Thread.Sleep(waitTime);
				}
			}
			
			if (!success)
			{
				LogMessage($"카메라 데이터 파일 저장에 모든 재시도가 실패했습니다: {filePath}");
			}
		}
		catch (Exception ex)
		{
			LogMessage($"카메라 데이터 파일 저장 중 오류 발생: {ex.Message}");
		}
	}

	// 모든 카메라 데이터를 하나의 파일로 저장하는 메서드 (외부에서 호출 가능)
	public void SaveAllCameraDataToSingleFile(string filePath = null)
	{
		try
		{
			// 스레드 안전을 위해 데이터 복사
			List<object> dataCopy;
			lock (_lock)
			{
				dataCopy = new List<object>(cameraDataList);
			}
			
			// 데이터가 없으면 저장하지 않음
			if (dataCopy.Count == 0)
			{
				LogMessage("저장할 카메라 데이터가 없습니다.");
				return;
			}
			
			// 파일 경로가 지정되지 않은 경우 기본 경로 사용
			if (string.IsNullOrEmpty(filePath))
			{
				// 고정된 파일명 사용
				string fileName = "merged_data.json";
				filePath = Path.Combine(cameraDataDirectory, fileName);
			}
			
			// JSON으로 변환
			string jsonData = JsonConvert.SerializeObject(dataCopy, Formatting.Indented);
			
			// 파일 쓰기 작업에 락을 걸어 한 번에 하나의 스레드만 접근하도록 함
			int retryCount = 0;
			const int maxRetries = 5; // 최대 재시도 횟수 증가
			bool success = false;
			
			while (!success && retryCount < maxRetries)
			{
				try
				{
					lock (_fileLock) // 모든 스레드 간 공유되는 락
					{
						// FileStream을 사용하여 더 세밀한 파일 제어
						using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
						using (StreamWriter writer = new StreamWriter(fs))
						{
							writer.Write(jsonData);
							writer.Flush();
						}
					}
					success = true;
					LogMessage($"모든 카메라 데이터를 파일로 저장했습니다: {filePath}");
				}
				catch (IOException ex)
				{
					retryCount++;
					int waitTime = 200 * retryCount; // 점점 더 오래 대기
					LogMessage($"모든 카메라 데이터 파일 저장 중 충돌 발생, 재시도 {retryCount}/{maxRetries}: {ex.Message}. {waitTime}ms 후 재시도합니다.");
					System.Threading.Thread.Sleep(waitTime);
				}
			}
			
			if (!success)
			{
				LogMessage($"모든 카메라 데이터 파일 저장에 모든 재시도가 실패했습니다: {filePath}");
			}
		}
		catch (Exception ex)
		{
			LogMessage($"모든 카메라 데이터 파일 저장 중 오류 발생: {ex.Message}");
		}
	}

	// 카메라 데이터 초기화 메서드
	public void ClearCameraData()
	{
		lock (_lock)
		{
			cameraDataList.Clear();
			camerasWithData.Clear();
		}
		LogMessage("카메라 데이터를 초기화했습니다.");
	}

	// 데이터 수집 시작
	public void StartDataCollection()
	{
		lock (_lock)
		{
			// 데이터 초기화
			cameraDataList.Clear();
			camerasWithData.Clear();
			
			// 데이터 수집 시작 시간 설정
			lastDataCollectionTime = DateTime.Now;
			
			// 데이터 수집 중 상태로 설정
			isCollectingData = true;
			
			// 로그 출력
			LogMessage("카메라 데이터 수집을 시작합니다. 모든 카메라로부터 데이터를 기다리거나 3초 타임아웃을 기다립니다.");
		}
	}

	// OnMessageReceived 메서드 내부에서 데이터 처리 후 추가할 코드
	// 데이터 수집 시간 업데이트 및 카메라 ID 추가
	private void UpdateDataCollectionStatus(string cameraId)
	{
		lock (_lock)
		{
			lastDataCollectionTime = DateTime.Now;
			if (!string.IsNullOrEmpty(cameraId) && !camerasWithData.Contains(cameraId))
			{
				camerasWithData.Add(cameraId);
			}
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