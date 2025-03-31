using System;
using System.Windows.Forms;
using SmartShoes.Common.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ZXing.QrCode.Internal;
using Windows.System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace SmartShoes.Client.UI
{
    public partial class SettingsForm : UserControl, IPageChangeNotifier
    {
        public event EventHandler<PageChangeEventArgs> PageChangeRequested;
        private DelphiHelper dph = new DelphiHelper(@"ghwlongdllG_64.dll");
        private bool showMatSet = false;

        private List<ShoesInform> listShoes = new List<ShoesInform>();


        public SettingsForm()
        {
            InitializeComponent();
            this.panel1.Visible = true;
            this.txtContainerId.Text = Properties.Settings.Default.CONTAINER_ID;

            // 블루투스 연결 상태 변경 이벤트 구독
            BLEManager.Instance.ConnectionStatusChanged += OnBluetoothConnectionChanged;

            // 블루투스 연결 상태 초기화
            UpdateBluetoothStatus(true, BLEManager.Instance.IsLeftDeviceConnected);
            UpdateBluetoothStatus(false, BLEManager.Instance.IsRightDeviceConnected);

            // 카메라 상태 초기화
            InitializeCameraStatus();

            // ws 자동연결결
            connectWebSocket();

            // 폼 로드 이벤트 추가
            this.Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // 폼 로드 시 연결된 클라이언트 상태 업데이트
            UpdateConnectedClientsStatus();
            
        }
        //private void UpdateConnectedClientsStatus()
        //{

        //}

        private void UpdateConnectedClientsStatus()
        {
            // 서버가 실행 중인지 확인
            if (WebSocketServerThread.Instance.IsRunning)
            {
                // 연결된 클라이언트 목록 가져오기
                List<string> connectedClients = WebSocketServerThread.Instance.GetConnectedClients();
                
                // 모든 카메라 상태를 "Disconnect"로 초기화
                InitializeCameraStatus();
                
                // 연결된 클라이언트 상태 업데이트
                foreach (string clientId in connectedClients)
                {
                    UpdateCameraStatus(clientId, "Connect");
                }
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CONTAINER_ID = this.txtContainerId.Text;
            Properties.Settings.Default.Save();

            if (showMatSet)
            {
                showMatSet = false;
                var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
                closeForm();
            }

            this.Invoke(new Action(() => MovePage(typeof(LoginForm))));
            //this.Invoke(new Action(() => MovePage(new LoginForm())));
            //MovePage(new LoginForm());

        }

        private void connectWebSocket(){
            try
            {
                WebSocketServerThread.Instance.Initialize("0.0.0.0", 8080);
                // OnClientConnected 콜백 등록
                WebSocketServerThread.Instance.OnClientConnected = OnClientConnected;
                // OnClientDisconnected 콜백 등록
                WebSocketServerThread.Instance.OnClientDisconnected = OnClientDisconnected;
                WebSocketServerThread.Instance.Start();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"WebSocket 서버 시작 실패: {ex.Message}");
            }
        }

        protected void MovePage(Type pageType)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => PageChangeRequested?.Invoke(this, new PageChangeEventArgs(pageType))));
            }
            else
            {
                PageChangeRequested?.Invoke(this, new PageChangeEventArgs(pageType));
            }
        }

        private void btnCameraConnect_Click(object sender, EventArgs e)
        {
            CameraManager.Instance.Connect("ws://localhost:8080/smartShoes/ws/chat");

            CameraManager.Instance.RegisterCameraStatusUpdate(UpdateCameraStatus);

        }


        private void OnClientConnected(string clientId)
        {
            // 다른 페이지의 Label 텍스트를 변경
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {

                    UpdateCameraStatus(clientId, "Connect");
                    //myLabel.Text = $"클라이언트 {clientId}가 연결되었습니다.";
                }));
            }
            else
            {
                UpdateCameraStatus(clientId, "Connect");
                //myLabel.Text = $"클라이언트 {clientId}가 연결되었습니다.";
            }
        }

        private void OnClientDisconnected(string clientId)
        {
            // 클라이언트 연결 해제 시 상태 업데이트
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    UpdateCameraStatus(clientId, "Disconnect");
                }));
            }
            else
            {
                UpdateCameraStatus(clientId, "Disconnect");
            }
        }

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
            if (cameraId.StartsWith("camera"))
            {
                if (int.TryParse(cameraId.Substring(6), out int cameraNumber) && cameraNumber >= 1 && cameraNumber <= 9)
                {
                    return cameraNumber - 1;
                }
            }
            return -1;
        }

        private void btnMatSetting_Click(object sender, EventArgs e)
        {
            //test();
            if (dph != null)
            {
                showMatSet = true;
                var showForm = dph.GetFunction<DelphiHelper.TShowForm>("ShowForm");
                showForm(panel1.Handle, 0, 0, 0, 0, true);
            }
        }

        private void btnSkeleton_Click(object sender, EventArgs e)
        {
            if (showMatSet)
            {
                showMatSet = false;
                var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
                closeForm();
            }

            // WebBrowser webBrowser = new WebBrowser();
            // webBrowser.Dock = DockStyle.Fill;
            // panel1.Controls.Add(webBrowser);

            // URL 로드
            // webBrowser.Navigate(new Uri("http://192.168.0.103:8554"));

            // WebView2 컨트롤을 생성하고 Panel에 추가
            WebView2 webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            panel1.Controls.Add(webView);

            // 초기화 및 웹 페이지 URL 설정
            // webView.Source = new Uri("http://192.168.0.29:8554");
            webView.Source = new Uri(Properties.Settings.Default.WEBVIEW_URL);

        }

        private  void pictureBox1_Click(object sender, EventArgs e)
        {
             if (showMatSet)
            {
                showMatSet = false;
                var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
                closeForm();
            }

            closefunc();

            // 테스트 명령어 전송
            // BLEManager.Instance.SendData("@START#1$\r\n");

            // 파일 전송 api 테스트

            //test();
        }


        private async void test()
        {
            try
            {
                // 1. 간단한 파일 생성(json내용이 들어갈 예정)
                string jsonContent = "{\"name\":\"test\",\"age\":10}";
                string filePath = Path.Combine(Path.GetTempPath(), "test.json");
                File.WriteAllText(filePath, jsonContent);

                // 2. 파일 전송 api 호출
                // http://221.161.177.191:8080/swagger-ui/index.html#/Report/createCameraResult
                string apiUrl = "http://221.161.177.191:8080/api/report/camera-result";

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // 타임아웃 설정

                    // MultipartFormDataContent 생성
                    using (var multipartContent = new MultipartFormDataContent())
                    {
                        // 파일 추가 - Content-Type 헤더를 제거하여 서버가 자동으로 처리하도록 함
                        var fileBytes = File.ReadAllBytes(filePath);
                        var fileContent = new ByteArrayContent(fileBytes);
                        // Content-Type 헤더 제거 (서버가 자동으로 처리하도록)
                        // fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        
                        // 파일 이름에 확장자가 포함되어 있는지 확인
                        string fileName = Path.GetFileName(filePath);
                        multipartContent.Add(fileContent, "cameraFile", fileName);

                        // 파라미터 추가
                        multipartContent.Add(new StringContent("68"), "userSid");
                        multipartContent.Add(new StringContent("33"), "containerSid");
                        multipartContent.Add(new StringContent("419"), "reportSid");

                        // 요청 전송 전 로그 출력
                        Console.WriteLine("전송할 파라미터:");
                        Console.WriteLine($"userSid: 68");
                        Console.WriteLine($"containerSid: 33");
                        Console.WriteLine($"reportSid: 419");
                        Console.WriteLine($"cameraFile: {fileName} (크기: {fileBytes.Length} 바이트)");

                        // 요청 전송
                        var response = await client.PostAsync(apiUrl, multipartContent);

                        // 응답 확인
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"파일 업로드 성공\n응답: {responseContent}", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"파일 업로드 실패: {response.StatusCode} - {response.ReasonPhrase}\n오류 내용: {errorContent}",
                                "실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // 3. 파일 전송 완료 후 파일 삭제
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"HTTP 요청 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TaskCanceledException ex)
            {
                MessageBox.Show($"요청 시간 초과: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예상치 못한 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closefunc()
        {
            if (dph != null)
            {
                dph.Dispose();
                dph = null;
            }
            Application.Exit();
        }

        private async void btnCallShoes_Click(object sender, EventArgs e)
        {
            ApiCallHelper apiCallHelper = new ApiCallHelper();
            string shoesUrl = Properties.Settings.Default.SHOES_CALL_URL_RELEASE + txtContainerId.Text;

            try
            {
                string getResponse = await apiCallHelper.GetAsync(shoesUrl);

                // JSON 파싱
                JObject containerInfo = JObject.Parse(getResponse);
                string leftMac = containerInfo["leftSensorMac"].ToString();
                string rightMac = containerInfo["rightSensorMac"].ToString();
                Console.WriteLine($"leftMac: {leftMac}, rightMac: {rightMac}");
                // MAC 주소 저장
                SaveBluetoothSettings(leftMac, rightMac);

                int time = Properties.Settings.Default.SENSOR_SET_TIME;
                // BLE 연결 시도
                await BLEManager.Instance.InitializeConnection(leftMac, rightMac, time);

                Properties.Settings.Default.CONTAINER_ID = this.txtContainerId.Text;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"신발 연결 실패: {ex.Message}");
            }
        }

        private void SaveBluetoothSettings(string leftMac, string rightMac)
        {
            Properties.Settings.Default.BLE_LEFT_MAC_ADDRESS = leftMac;
            Properties.Settings.Default.BLE_RIGHT_MAC_ADDRESS = rightMac;
            Properties.Settings.Default.Save();
        }

        // listView1에서 선택된 신발의 MAC 주소를 저장
        // private void btnSaveSelectedShoes_Click(object sender, EventArgs e)
        // {
        // 	if (listView1.SelectedItems.Count > 0)
        // 	{
        // 		var selectedItem = listView1.SelectedItems[0];
        // 		string leftMac = selectedItem.SubItems[0].Text;  // 첫 번째 컬럼 (왼쪽 MAC)
        // 		string rightMac = selectedItem.SubItems[1].Text; // 두 번째 컬럼 (오른쪽 MAC)

        // 		SaveBluetoothSettings(leftMac, rightMac);
        // 		MessageBox.Show("블루투스 설정이 저장되었습니다.");
        // 	}
        // }

        private void UpdateBluetoothStatus(bool isLeft, bool isConnected, string errorMessage = null)
        {
            var label = isLeft ? label11 : label12;
            var side = isLeft ? "왼쪽" : "오른쪽";

            label.Text = $"{side} 센서 연결 상태: {(isConnected ? "연결됨" : "미연결")}";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine($"블루투스 연결 오류: {errorMessage}");
            }
        }

        private void OnBluetoothConnectionChanged(object sender, BluetoothConnectionEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateBluetoothStatus(e.IsLeft, e.IsConnected, e.ErrorMessage)));
                return;
            }
            UpdateBluetoothStatus(e.IsLeft, e.IsConnected, e.ErrorMessage);
        }

        // Form이 닫힐 때 이벤트 구독 해제
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Designer.cs의 리소스 정리
                if (dph != null)
                {
                    dph.Dispose();
                    dph = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }

                // 블루투스 이벤트 구독 해제
                BLEManager.Instance.ConnectionStatusChanged -= OnBluetoothConnectionChanged;
            }
            base.Dispose(disposing);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(this.textBox1.Text, out int time))
            {
                MessageBox.Show("숫자로 입력하세요.");
                this.textBox1.Text = "";
            }
            else
            {
                BLEManager.Instance.SetTimer(time);
            }
        }

        private void InitializeCameraStatus()
        {
            // 모든 카메라 상태를 "Disconnect"로 초기화
            Label[] cameraStatusLabels = new Label[9] { cameraStatus1, cameraStatus2, cameraStatus3, cameraStatus4, cameraStatus5, cameraStatus6, cameraStatus7, cameraStatus8, cameraStatus9 };
            
            foreach (var label in cameraStatusLabels)
            {
                label.Text = "Disconnect";
            }
        }
    }
}