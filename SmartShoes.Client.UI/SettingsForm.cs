using System;
using System.Windows.Forms;
using SmartShoes.Common.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ZXing.QrCode.Internal;
using Windows.System;
using System.Threading.Tasks;

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

            // ws 자동연결결
            connectWebSocket();
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
                WebSocketServerThread.Instance.Start();
            }
            catch (System.Exception)
            {
                Console.WriteLine("WebSocket 서버 시작 실패");
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // if (showMatSet)
            // {
            // 	showMatSet = false;
            // 	var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
            // 	closeForm();
            // }

            // closefunc();



            // 테스트 명령어 전송
            BLEManager.Instance.SendData("@START#1$\r\n");


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
    }
}