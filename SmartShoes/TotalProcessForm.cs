using SmartShoes.Common.Forms;
using System;
using System.Windows.Forms;
using System.Timers;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MathNet.Filtering;
using MathNet.Filtering.FIR;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
using MathNet.Filtering.IIR;
using OxyPlot.Axes;
using OxyPlot.SkiaSharp;
using CsvHelper;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using System.Data;


namespace SmartShoes
{
	public partial class TotalProcessForm : Form
	{
		// About Mat Control
		private DelphiHelper dph = new DelphiHelper(@"ghwlongdllG_64.dll");
		private WebSocketServerThread wsst;
		private System.Timers.Timer closeTimer;
		private bool exitFlag = false;
		private string folderPath = "";
		private string folderName = "";

		private UserData oUserData = null;
		private Dictionary<string, object> oMatData = null;

		// About Sensor Device 
		private static BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();

		private static InTheHand.Bluetooth.BluetoothDevice _deviceR;
		private static InTheHand.Bluetooth.BluetoothDevice _deviceL;

		private static List<double[]> transformedAccelL = new List<double[]>();
		private static List<double[]> transformedAccelR = new List<double[]>();

		private readonly Guid serviceUUID = new Guid("6e400001-b5a3-f393-e0a9-e50e24dcca9e");
		private readonly Guid _notifyUuid = new Guid("6e400003-b5a3-f393-e0a9-e50e24dcca9e");
		private readonly Guid _writeUuid = new Guid("6e400002-b5a3-f393-e0a9-e50e24dcca9e");

		private string sensorIdL = "F8:5B:07:62:14:87";
		private string sensorIdR = "D7:AA:69:8E:C0:06";

		private bool _flagL = false;
		private bool _flagR = false;

		private bool _connFlagL = false;
		private bool _connFlagR = false;

		public class UserData
		{
			public string Name { get; set; }
			public string Birth { get; set; }
			public string Gender { get; set; }
			public string Height { get; set; }
			public string Weight { get; set; }
			public string ShoeSize { get; set; }
			public string MeasureTime { get; set; }
		}

		public TotalProcessForm()
		{
			InitializeComponent();

			if (genderCmb.Items.Count > 0) // 아이템이 있는지 확인
			{
				genderCmb.SelectedIndex = 0;
			}

			this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

		}

		/// <summary>
		/// 폼 종료
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.exitFlag = true;

			SuccessMeasure();
			dph.Dispose();
		}

		#region :: Button Click Events ::
		private string RemoveKoreanCharacters(string text)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in text)
			{
				// 한글 유니코드 범위는 U+AC00 ~ U+D7A3
				if (!(c >= '\uAC00' && c <= '\uD7A3'))
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		private void txtName_KeyPress(object sender, KeyPressEventArgs e)
		{
			bool isAllowed = (e.KeyChar >= '0' && e.KeyChar <= '9') ||  // 숫자
					 (e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||  // 대문자
					 (e.KeyChar >= 'a' && e.KeyChar <= 'z') ||  // 소문자
					 e.KeyChar == '\b' ||                       // 백스페이스
					 e.KeyChar == '\t';                         // 탭

			// 한글 자모 유니코드 범위 U+1100 ~ U+11FF, 완성형 한글 U+AC00 ~ U+D7A3 차단
			bool isKorean = (e.KeyChar >= '\u1100' && e.KeyChar <= '\u11FF') ||
							(e.KeyChar >= '\uAC00' && e.KeyChar <= '\uD7A3');

			// 허용된 문자 외에 모든 입력 차단
			if (!isAllowed || isKorean)
			{
				e.Handled = true;
			}
		}

		/// <summary>
		/// 카메라 동영상 테스트 시작
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCameraTestStart_Click(object sender, EventArgs e)
		{
			if (wsst == null) { MessageBox.Show("WebSocket 서버가 연결되어있지 않습니다."); return; }
			if (wsst == null) { txtTest.AppendText("WebSocket 서버가 연결되어있지 않습니다.\n"); return; }
			wsst.BroadcastMessage("start", "test");
			txtTest.AppendText("Camera test start");
		}

		/// <summary>
		/// 카메라 동영상 테스트 종료
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCameraTestStop_Click(object sender, EventArgs e)
		{
			if (wsst == null) { MessageBox.Show("WebSocket 서버가 연결되어있지 않습니다."); return; }
			if (wsst == null) { txtTest.AppendText("WebSocket 서버가 연결되어있지 않습니다.\n"); return; }
			wsst.BroadcastMessage("stop", "test");
			txtTest.AppendText("Camera test stop");
		}

		/// <summary>
		/// 카메라 커넥트 시작
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCameraStart_Click(object sender, EventArgs e)
		{
			//if (wsst == null)
			//{
			//	wsst = new WebSocketServerThread("0.0.0.0", 8080);
			//	wsst.SetLogCallback(LogMessage);
			//	wsst.Start();

			//	wsst.OnClientConnected = (message) => {
			//		// GUI 스레드에서 txtTest를 업데이트하기 위해 Invoke 사용
			//		if (txtTest.InvokeRequired)
			//		{
			//			txtTest.Invoke((Action)(() =>
			//			{
			//				txtTest.AppendText(message + Environment.NewLine);
			//			}));
			//		}
			//		else
			//		{
			//			txtTest.AppendText(message + Environment.NewLine);
			//		}

			//		UpdateCameraStatus(message, "connect");
			//	};

			//	txtTest.AppendText("WebSocket 서버가 시작되었습니다.\n");

			//	btnCameraStop.Enabled = true;
			//	btnCameraTestStart.Enabled = true;
			//	btnCameraTestStop.Enabled = true;
			//}
			//else
			//{
			//	txtTest.AppendText("WebSocket 서버가 이미 실행 중입니다.\n");
			//}
		}

		/// <summary>
		/// 카메라 커넥트 종료
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCameraStop_Click(object sender, EventArgs e)
		{

			if (wsst != null)
			{
				wsst.Stop();
				wsst = null;
				string[] cameraDivice = { "camera01", "camera02", "camera03", "camera04", "camera05", "camera06", "camera07", "camera08", "camera09" };
				foreach (string cameraId in cameraDivice)
				{
					UpdateCameraStatus(cameraId, "disconnect");
				}

				txtTest.AppendText("WebSocket 서버가 종료되었습니다.\n");
				btnCameraStop.Enabled = false;
				btnCameraTestStart.Enabled = false;
				btnCameraTestStop.Enabled = false;
			}
			else
			{
				txtTest.AppendText("WebSocket 서버가 실행 중이 아닙니다.\n");
			}

		}

		/// <summary>
		/// 검지매트 설정 시작
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMatSetup_Click(object sender, EventArgs e)
		{
			this.btnMeasureStart.Enabled = false;
			this.btnMeasureStop.Enabled = false;
			this.btnConnectSql.Enabled = false;

			var showForm = dph.GetFunction<DelphiHelper.TShowForm>("ShowForm");
			showForm(panel1.Handle, 0, 0, 0, 0, true);
		}

		/// <summary>
		/// 검지매트 설정 종료
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMatClose_Click(object sender, EventArgs e)
		{

			try
			{
				var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
				closeForm();
			}
			catch (Exception ex)
			{
				txtTest.AppendText($"Exception occurred while calling CloseForm: {ex.Message}\n");
				// 추가적인 예외 처리나 리소스 정리를 여기에 포함
			}


			this.btnMeasureStart.Enabled = true;
			this.btnMeasureStop.Enabled = true;
			this.btnConnectSql.Enabled = true;
		}

		/// <summary>
		/// 측정 시작
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMeasureStart_Click(object sender, EventArgs e)
		{
			this.btnMatSetup.Enabled = false;
			this.btnMatSetupClose.Enabled = false;
			this.btnConnectSql.Enabled = false;

			if (wsst == null) { MessageBox.Show("WebSocket 서버가 연결되어있지 않습니다."); return; }
			if (dph != null)
			{
				var showForm = dph.GetFunction<DelphiHelper.TShowForm>("ShowForm");
				showForm(panel1.Handle, 0, 0, 0, 0, true);

				var measurestart = dph.GetFunction<DelphiHelper.TMeasurestart>("Measurestart");
				measurestart(60);

				this.btnMeasureStop.Enabled = true;
				this.btnMeasureStart.Enabled = false;

				StartCloseTimer();

				measureTimer.Text = "측정중...";
			}

			// "Gcon Measure Data" 폴더 경로 지정
			string baseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Gcon Measure Data");
			Directory.CreateDirectory(baseFolderPath);

			string txtNameInput = txtName.Text.ToString();
			string cleanedName = string.Concat(txtNameInput.Split(Path.GetInvalidFileNameChars()));
			this.folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + cleanedName;
			this.folderPath = Path.Combine(baseFolderPath, folderName);
			Directory.CreateDirectory(folderPath);

			StartCamera(folderName);
		}

		/// <summary>
		/// 측정 종료
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnMeasureStop_Click(object sender, EventArgs e)
		{
			oUserData = null;
			oMatData = null;
			if (wsst == null) { MessageBox.Show("WebSocket 서버가 연결되어있지 않습니다."); return; }
			SuccessMeasure();
		}

		/// <summary>
		/// sqlite 연결확인
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnConnectSql_Click(object sender, EventArgs e)
		{
			try
			{
				txtTest.Text = "";

				Sqlite3Helper dbHelper = new Sqlite3Helper();
				var records = dbHelper.GetQueryRecords("SELECT * FROM ghwgaitchk1 ORDER BY ID DESC LIMIT 1;");
				foreach (var record in records)
				{
					foreach (var kvp in record)
					{
						txtTest.Text += $"{kvp.Key}: {kvp.Value} || ";
					}
					txtTest.Text += "\n---------------------------------------------------\n";
				}
				txtTest.Text += "================== 연결완료 =======================\n";
			}
			catch (Exception ex)
			{
				txtTest.Text += "================== 연결실패 =======================\n";
			}
		}

		#endregion

		#region :: Custom Method ::
		private void UpdateCameraStatus(string cameraId, string status)
		{
			Label[] cameraStatusLabels = new Label[9] { cameraStatus1, cameraStatus2, cameraStatus3, cameraStatus4, cameraStatus5, cameraStatus6, cameraStatus7, cameraStatus8, cameraStatus9 };
			int index = GetCameraIndex(cameraId);
			if (index >= 0 && index < cameraStatusLabels.Length)
			{
				if (cameraStatusLabels[index].InvokeRequired)
				{
					cameraStatusLabels[index].Invoke((Action)(() =>
					{
						cameraStatusLabels[index].Text = status;
					}));
				}
				else
				{
					cameraStatusLabels[index].Text = status;
				}
			}
		}

		private int GetCameraIndex(string cameraId)
		{
			// 예시로 cameraId가 "camera01" 형식이라면
			if (cameraId.StartsWith("camera"))
			{
				if (int.TryParse(cameraId.Substring(6), out int cameraNumber) && cameraNumber >= 1 && cameraNumber <= 9)
				{
					return cameraNumber - 1; // 배열 인덱스는 0부터 시작하므로 1을 뺌
				}
			}
			return -1; // 유효하지 않은 인덱스
		}

		private void LogMessage(string message)
		{
			// 로그 메시지를 처리하는 메서드
			Console.WriteLine(message);
		}

		private void StartCloseTimer()
		{
			closeTimer = new System.Timers.Timer(60000);
			closeTimer.Elapsed += OnTimedEvent;
			closeTimer.AutoReset = false; // 한 번만 실행하도록 설정
			closeTimer.Start();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Action(() => OnTimedEvent(sender, e)));
			}
			else
			{
				SuccessMeasure();
			}
		}

		private void SuccessMeasure()
		{
			this.btnMatSetup.Enabled = true;
			this.btnMatSetupClose.Enabled = true;
			this.btnConnectSql.Enabled = true;

			if (closeTimer != null)
			{
				// 타이머 중지 및 해제
				closeTimer.Stop();
				closeTimer.Dispose();
				closeTimer = null;
			}

			var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
			measurestop(true);

			var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
			closeForm();

			this.btnMeasureStop.Enabled = false;
			this.btnMeasureStart.Enabled = true;

			this.measureTimer.Text = "측정 대기";

			if (this.exitFlag) { return; }
			SaveData();
			
			if(oUserData == null || oMatData == null) { return; }
			PrintTestForm ptf = new PrintTestForm(oUserData, oMatData);
			ptf.ShowDialog();
		}



		private void SaveData()
		{
			try
			{
				txtTest.Text = "";

				SaveUserData(folderPath);
				SaveMatData(folderPath);
				MoveDirectory(folderPath);
				StopCamera(folderName);

				MessageBox.Show(folderName + "측정 데이터 저장완료", "측정 저장완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error : " + ex);
			}
		}

		private void SaveUserData(string folderPath)
		{
			Directory.CreateDirectory(folderPath);

			var userData = new
			{
				Name = txtName.Text,
				Birth = dteBirth.Value.ToString("yyyy-MM-dd"),
				Gender = genderCmb.Text,
				Height = txtHeight.Text,
				Weight = txtWeight.Text,
				ShoeSize = txtShoeSize.Text,
				MeasureTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			};

			// 인스턴스 생성 및 데이터 할당
			oUserData = new UserData
			{
				Name = txtName.Text,
				Birth = dteBirth.Value.ToString("yyyy-MM-dd"),
				Gender = genderCmb.Text,
				Height = txtHeight.Text,
				Weight = txtWeight.Text, 
				ShoeSize = txtShoeSize.Text,
				MeasureTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			};

			string jsonData = JsonConvert.SerializeObject(userData, Formatting.Indented);
			string userFilePath = Path.Combine(folderPath, "User Inform.txt");

			File.WriteAllText(userFilePath, jsonData);
		}

		private void StartCamera(string folderPath)
		{
			if (wsst == null) { txtTest.AppendText("WebSocket 서버가 연결되어있지 않습니다."); return; }
			wsst.BroadcastMessage("start", folderPath);
			txtTest.AppendText("Camera start");
		}

		private void StopCamera(string folderPath)
		{
			if (wsst == null) { txtTest.AppendText("WebSocket 서버가 연결되어있지 않습니다."); return; }
			wsst.BroadcastMessage("stop", folderPath);
			txtTest.AppendText("Camera stop");
		}

		private void SaveMatData(string folderPath)
		{
			Sqlite3Helper dbHelper = new Sqlite3Helper();
			var records = dbHelper.GetQueryRecords("SELECT * FROM ghwgaitchk1 ORDER BY ID DESC LIMIT 1;");

			List<Dictionary<string, object>> recordList = new List<Dictionary<string, object>>();

			#region ::: key value Division :::
			foreach (var record in records)
			{
				Dictionary<string, object> recordDict = new Dictionary<string, object>();

				foreach (var kvp in record)
				{
					if (kvp.Key.Length > 3)
					{
						if (kvp.Key.Substring(0, 3).Equals("val"))
						{
							string keywordg = "";
							switch (kvp.Key.Substring(3, 2))
							{
								case "01":
									keywordg = "StepLength";
									break;
								case "02":
									keywordg = "StrideLength";
									break;
								case "03":
									keywordg = "SingleStepTime";
									break;
								case "04":
									keywordg = "StepAngle";
									break;
								case "05":
									keywordg = "StepCount";
									break;
								case "06":
									keywordg = "BaseOfGait";
									break;
								case "07":
									keywordg = "StepForce";
									break;
								case "08":
									keywordg = "StancePhase";
									break;
								case "09":
									keywordg = "SwingPhase";
									break;
								case "10":
									keywordg = "SingleSupport";
									break;
								case "11":
									keywordg = "TotalDoubleSupport";
									break;
								case "12":
									keywordg = "LoadResponce";
									break;
								case "13":
									keywordg = "PreSwing";
									break;
								case "14":
									keywordg = "StepPosition";
									break;
								case "15":
									keywordg = "StrideTime";
									break;
								case "16":
									keywordg = "StanceTime";
									break;
								case "17":
									keywordg = "CopLength";
									break;
								case "18":
									keywordg = "val18";
									break;
								case "19":
									keywordg = "val19";
									break;
							}
							recordDict[keywordg + kvp.Key.Substring(kvp.Key.Length - 1)] = kvp.Value;
						}
						else
						{
							recordDict[kvp.Key] = kvp.Value;
						}
					}
					else
					{
						recordDict[kvp.Key] = kvp.Value;
					}
				}
				oMatData = null;
				oMatData = recordDict;
				recordList.Add(recordDict);
			}
			#endregion

			string json = JsonConvert.SerializeObject(recordList, Formatting.Indented);
			string MatFilePath = Path.Combine(folderPath, "Mat Data.txt");

			File.WriteAllText(MatFilePath, json);
		}

		public void MoveDirectory(string folderPath)
		{
			// 원본 폴더 경로
			string sourceFolder = @"C:\camera\" + folderName;


			// 대상 폴더 경로
			//string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string targetFolder = Path.Combine(folderPath, "camera");

			try
			{
				// 대상 폴더가 이미 존재하는지 확인
				if (Directory.Exists(targetFolder))
				{
					MessageBox.Show("A folder named 'camera' already exists on the Desktop.");
					return;
				}

				// 폴더 이동
				//CopyDirectory(sourceFolder, targetFolder);
				//MessageBox.Show("Folder moved successfully.");
			}
			catch (Exception ex)
			{
				// 오류 발생 시 메시지 표시
				txtTest.Text += ("An error occurred: " + ex.Message + "\n");
			}
		}

		public void CopyDirectory(string sourceFolder, string targetFolder)
		{
			try
			{

				// 대상 폴더가 존재하지 않으면 생성
				if (!Directory.Exists(targetFolder))
				{
					Directory.CreateDirectory(targetFolder);
				}

				// 원본 폴더의 모든 파일을 대상 폴더로 복사
				foreach (string file in Directory.GetFiles(sourceFolder))
				{
					string fileName = Path.GetFileName(file);
					string destFile = Path.Combine(targetFolder, fileName);
					File.Copy(file, destFile, true);
				}

				// 원본 폴더의 모든 하위 디렉토리를 대상 폴더로 복사
				foreach (string subDir in Directory.GetDirectories(sourceFolder))
				{
					string subDirName = Path.GetFileName(subDir);
					string destSubDir = Path.Combine(targetFolder, subDirName);
					//CopyDirectory(subDir, destSubDir); // 재귀 호출
				}

				txtTest.Text += ("Folder copied successfully.\n");
			}
			catch (Exception ex)
			{
				// 오류 발생 시 메시지 표시
				txtTest.Text += ("An error occurred: " + ex.Message + "\n");
			}
		}
		#endregion

		#region :: Custom Events ::
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			int selectionStart = txtName.SelectionStart;
			string newText = RemoveKoreanCharacters(txtName.Text);
			if (txtName.Text != newText)
			{
				txtName.Text = newText;
				txtName.SelectionStart = Math.Max(0, selectionStart - 1);
			}
		}
		#endregion

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			wsst?.Stop();
		}

		#region :: OpenPose exec :::
		private void btnOpenposeCommand_Click(object sender, EventArgs e)
		{
			txtTest.Text += ($"Openpose Execution Start");
			// Stopwatch 인스턴스 생성 및 시작
			

			RunCommandClass runCmd = new RunCommandClass();

			// 실행할 파일 (예: OpenPoseDemo.exe)과 인자 설정
			string commandDir = txtOpenPoseDir.Text;
			//string commandDir = @"C:\Users\gkdis\Desktop\openpose-1.7.0-binaries-win64-cpu-python3.7-flir-3d\openpose\bin\";
			//string commandDir = @"C:\Users\admin\Desktop\openpose\bin\";

			string videoDir = txtVideoDir.Text;
			//string videoDir = "C:\\camera\\20240802_135808_jangwonseok1\\camera\\05\\";
			//string videoDir = "C:\\camera\\20240725112957\\camera\\05\\";

			string videoFile = txtVideoNm.Text + ".mp4";
			//string videoFile = "20240802_135808_jangwonseok1.mp4";
			//string videoFile = "20240725112957.mp4";

			string saveDir = txtSaveDir.Text;
			//string saveDir = "C:\\json\\opentest";

			string videoStep = txtFrameSep.Text;

			string resolution = txtWinHeight.Text + "x" + txtWinWidth.Text;

			string modelDir = txtModelDir.Text;
			//string modelDir = "C:\\Users\\gkdis\\Desktop\\openpose-1.7.0-binaries-win64-cpu-python3.7-flir-3d\\openpose\\models";
			//string modelDir = "C:\\Users\\admin\\Desktop\\openpose\\models";

			string command = commandDir + "\\OpenPoseDemo.exe";
			string arguments = $"--video {videoDir}\\{videoFile} --write_json {saveDir} --frame_step {videoStep} --net_resolution {resolution} --model_folder {modelDir} --display 0 --render_pose 0";

			runCmd.RunCommand(command, arguments);

			// Stopwatch 중지 및 경과 시간 출력
			
		}
		#endregion

		private static BluetoothLEDevice device = null;

		private async void btnSensorRead_Click(object sender, EventArgs e)
		{


			//BLEManager.Instance.DisconnectDevicesAsync();
			// 이전
			// string deviceAddress = "D41DD0B5C556"; // BLE 장치의 MAC 주소 "FD:E7:33:88:CE:AE"
			// await ConnectAndReceive(deviceAddressR, deviceAddressL);

			// string deviceAddressR = this.txtShoesSensorR.Text;
			// string deviceAddressL = this.txtShoesSensorL.Text;

			
			// 이벤트 핸들러 등록 (먼저 등록)
			// 기존 이벤트 핸들러 제거
			// BLEManager.Instance.ThresholdReachedL -= HandleThresholdReachedL;
			// BLEManager.Instance.ThresholdReachedR -= HandleThresholdReachedR;

			// _flagL = false;
			// _flagR = false;
			
			// transformedAccelL.Clear();
			// transformedAccelR.Clear();
			
			// // 새 이벤트 핸들러 등록
			// BLEManager.Instance.ThresholdReachedL += HandleThresholdReachedL;
			// BLEManager.Instance.ThresholdReachedR += HandleThresholdReachedR;


			//watcher.Start();
			//var result = await device.GetGattServicesForUuidAsync(serviceUUID, BluetoothCacheMode.Uncached);
			//await Console.Out.WriteLineAsync("result = " + result.Status);
			//return;

			// int datalen = Convert.ToInt32(this.txtShoesMT.Text) * 100;
			// bool isConnected = await BLEManager.Instance.ConnectAndReceiveAsync(deviceAddressR, deviceAddressL, datalen, serviceUUID, _notifyUuid);

			// btnSensorDataRead.Enabled = isConnected;
		}

		// 왼쪽 장치의 이벤트 핸들러
		private void HandleThresholdReachedL(object sender, EventArgs args)
		{
			// List<int[]> lstr = BLEManager.Instance._parsedDataL;
			// transformedAccelL.Clear();
			// foreach (int[] numbers in lstr)
			// {
			// 	if (numbers.Length == 6)
			// 	{
			// 		// 원시 가속도 데이터를 추출 (여기서는 마지막 3개의 값이 가속도 데이터라고 가정)
			// 		int[] rawAccel = new int[] { numbers[3], numbers[4], numbers[5] };

			// 		// 가속도 데이터를 실제 값으로 변환하고, 필요한 축 반전을 수행
			// 		double[] accelInG = Array.ConvertAll(rawAccel, val => ConvertToG(val));

			// 		// X, -Y, Z 로 축 반전 수행
			// 		double[] dbl = new double[] { -accelInG[0], -accelInG[1], accelInG[2] };
			// 		transformedAccelL.Add(dbl);
			// 	}
			// }

			// int datalen = Convert.ToInt32(this.txtShoesMT.Text) * 100;
			// Console.WriteLine("데이터가 " + datalen + "개에 도달했습니다. 이벤트 발생! ===================== Left");
			// _flagL = true;
			// if (_flagL && _flagR)
			// {
			// 	WriteCsvData();
			// }
			// 추가 작업 수행
		}

		// 오른쪽 장치의 이벤트 핸들러
		private void HandleThresholdReachedR(object sender, EventArgs args)
		{
			// List<int[]> lstr = BLEManager.Instance._parsedDataR;
			// transformedAccelR.Clear();
			// foreach (int[] numbers in lstr)
			// {
			// 	if (numbers.Length == 6)
			// 	{
			// 		// 원시 가속도 데이터를 추출 (여기서는 마지막 3개의 값이 가속도 데이터라고 가정)
			// 		int[] rawAccel = new int[] { numbers[3], numbers[4], numbers[5] };

			// 		// 가속도 데이터를 실제 값으로 변환하고, 필요한 축 반전을 수행
			// 		double[] accelInG = Array.ConvertAll(rawAccel, val => ConvertToG(val));

			// 		double[] dbl = new double[] { -accelInG[0], -accelInG[1], accelInG[2] };
			// 		transformedAccelR.Add(dbl);
			// 	}
			// }
			// int datalen = Convert.ToInt32(this.txtShoesMT.Text) * 100;
			// Console.WriteLine("데이터가 " + datalen + "개에 도달했습니다. 이벤트 발생! ===================== Right");
			// _flagR = true;
			// if (_flagL && _flagR)
			// {
			// 	WriteCsvData();
			// }
			// 추가 작업 수행
		}


		//private const double FullScaleRange = 2.0; // 기본 값: ±2g
		//private const double Gravity = 9.8; // 중력가속도 [m/s^2]
		//private const int ScaleFactor = 32768;

		/// <summary>
		/// 가속도 원시 데이터를 실제 g 단위의 가속도로 변환하는 함수
		/// </summary>
		/// <param name="rawData">원시 가속도 데이터 (int)</param>
		/// <param name="fullScaleRange">가속도계의 풀 스케일 범위 (기본값: ±2g)</param>
		/// <returns>[m/s^2] 단위의 실제 가속도 값 (double)</returns>
		public double ConvertToG(int rawData)
		{
			return (rawData * 2.0 * 9.8) / 32768;
		}

		private async void MeasureFunction()
		{
			// var rightDevice = BLEManager.Instance.GetRightDevice();
			// if (rightDevice != null)
			// {
			// 	Console.WriteLine($"Right device: {rightDevice.Name}");
			// }
			// else
			// {
			// 	Console.WriteLine("Right device is not connected.");
			// }

			// var leftDevice = BLEManager.Instance.GetLeftDevice();
			// if (leftDevice != null)
			// {
			// 	Console.WriteLine($"Left device: {leftDevice.Name}");
			// }
			// else
			// {
			// 	Console.WriteLine("Left device is not connected.");
			// }

			// int datalen = Convert.ToInt32(this.txtShoesMT.Text) * 100;
			
			// await functiontest(rightDevice, datalen);
			// await functiontest(leftDevice, datalen);
		}

		private async Task functiontest(InTheHand.Bluetooth.BluetoothDevice device, int datalen)
		{
			// var services = await device.Gatt.GetPrimaryServicesAsync(serviceUUID);
			// foreach (var service in services)
			// {
			// 	var characteristics = await service.GetCharacteristicsAsync();
			// 	Console.WriteLine(characteristics);
			// 	foreach (var characteristic in characteristics)
			// 	{
			// 		if (characteristic.Uuid == _writeUuid)
			// 		{
			// 			Console.WriteLine("received.");
			// 			string dataToSend = $"@DATA,{datalen}#\r\n";
			// 			byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
			// 			await characteristic.WriteValueWithResponseAsync(dataBytes);
			// 		}
			// 	}
			// }

		}

		// CSV 파일에서 데이터를 읽어오는 함수
		public List<double[]> ReadCsvData()
		{
			var data = new List<double[]>();
			int cntBetter = transformedAccelL.Count <= transformedAccelR.Count ? transformedAccelL.Count : transformedAccelR.Count;

			for (int i = 0; i < cntBetter; i++)
			{
				var row = new double[6];
				row[0] = transformedAccelL[i][0];
				row[1] = transformedAccelL[i][1];
				row[2] = transformedAccelL[i][2];
				row[3] = transformedAccelR[i][0];
				row[4] = transformedAccelR[i][1];
				row[5] = transformedAccelR[i][2];
				data.Add(row);
			}
		
			return data;
		}

		public void WriteCsvData()
		{
			// "Gcon Measure Data" 폴더 경로 지정
			string baseFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Gcon Measure Data");
			Directory.CreateDirectory(baseFolderPath);

			string txtNameInput = txtName.Text.ToString();
			string cleanedName = string.Concat(txtNameInput.Split(Path.GetInvalidFileNameChars()));
			this.folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + cleanedName;
			this.folderPath = Path.Combine(baseFolderPath, folderName);
			Directory.CreateDirectory(folderPath);

			string filePath = Path.Combine(folderPath, "Sensor Data.csv");

			var data = ReadCsvData();

			// CSV 파일을 작성할 StringBuilder 생성
			var csv = new StringBuilder();

			// CSV 헤더 추가
			csv.AppendLine("AccelL_X,AccelL_Y,AccelL_Z,AccelR_X,AccelR_Y,AccelR_Z");

			// 데이터 행 추가
			foreach (var row in data)
			{
				// 각 행의 값을 쉼표로 구분하여 CSV 형식으로 변환
				var line = string.Join(",", row);
				csv.AppendLine(line);
			}

			// CSV 파일 저장
			File.WriteAllText(filePath, csv.ToString(), Encoding.UTF8);

			// BLEManager.Instance._parsedDataL.Clear();
			// BLEManager.Instance._parsedDataR.Clear();
		}

		private void btnSensorDataRead_Click(object sender, EventArgs e)
		{
			MeasureFunction();

			//this.btnSensorDataRead.Enabled = false;
			//this._connFlagL = false;
			//this._connFlagR = false;
		}



	}
}
