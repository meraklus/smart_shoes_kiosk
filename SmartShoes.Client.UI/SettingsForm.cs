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
			// this.numSensorTime.Value = Properties.Settings.Default.SENSOR_SET_TIME;

			// this.listView1.View = View.Details;
			// this.listView1.FullRowSelect = true;

			// // 컬럼 설정
			// this.listView1.Columns.Add("leftDevice", 0, HorizontalAlignment.Center);
			// this.listView1.Columns.Add("rightDevice", 0, HorizontalAlignment.Center);
			// this.listView1.Columns.Add("신발명", 220, HorizontalAlignment.Center);
			// this.listView1.Columns.Add("신발SIZE", 150, HorizontalAlignment.Center);

			if (!Properties.Settings.Default.SHOES_JSON.Equals(""))
			{
				CallShoesMacAddr();
			}
		}

		private async void CallShoesMacAddr()
		{
			ApiCallHelper apiCallHelper = new ApiCallHelper();
			// GET 요청 예제 
#if DEBUG
			//string shoesUrl = Properties.Settings.Default.SHOES_CALL_URL_DEBUG + txtContainerId.Text;
			string shoesUrl = Properties.Settings.Default.SHOES_CALL_URL_RELEASE + txtContainerId.Text;
#else
			string shoesUrl = Properties.Settings.Default.SHOES_CALL_URL_RELEASE + txtContainerId.Text;
#endif
			try
			{

				string getResponse = await apiCallHelper.GetAsync(shoesUrl);

				Properties.Settings.Default.CONTAINER_ID = this.txtContainerId.Text;
				// Properties.Settings.Default.SHOES_JSON = getResponse;
				// Properties.Settings.Default.SENSOR_SET_TIME = Convert.ToInt16(this.numSensorTime.Value);
				// Properties.Settings.Default.Save();
				// Console.WriteLine(Properties.Settings.Default.SHOES_JSON);


				BLEManager.Instance.InitializeConnection(Properties.Settings.Default.BLE_LEFT_MAC_ADDRESS, Properties.Settings.Default.BLE_RIGHT_MAC_ADDRESS);
				// JArray shoesArray = JArray.Parse(Properties.Settings.Default.SHOES_JSON);
				// 각 요소를 JObject로 접근

				// listView1.Items.Clear();
				// listShoes.Clear();
				// foreach (JObject shoe in shoesArray)
				// {
				// 	ShoesInform shoeInform = new ShoesInform();
				// 	shoeInform.ID = shoe.GetValue("shoesSid").ToString();
				// 	shoeInform.Name = shoe.GetValue("shoesName").ToString();
				// 	shoeInform.LeftDeviceAddr = shoe.GetValue("leftMacAddress").ToString();
				// 	shoeInform.RightDeviceAddr = shoe.GetValue("rightMacAddress").ToString();
				// 	shoeInform.Size = shoe.GetValue("shoesSize").ToString();
				// 	listShoes.Add(shoeInform);


				// 	ListViewItem item = new ListViewItem(shoe.GetValue("leftMacAddress").ToString());
				// 	item.SubItems.Add(shoe.GetValue("rightMacAddress").ToString());
				// 	item.SubItems.Add(shoe.GetValue("shoesName").ToString());
				// 	item.SubItems.Add(shoe.GetValue("shoesSize").ToString());
				// 	// listView1.Items.Add(item);
				// }


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
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
			if (showMatSet)
			{
				showMatSet = false;
				var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
				closeForm();
			}

			closefunc();
		}

		private void closefunc()
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		private void btnCallShoes_Click(object sender, EventArgs e)
		{
			CallShoesMacAddr();
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

	}
}