using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using SmartShoes.Common.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet.Messages;


namespace SmartShoes.Client.UI
{
	public partial class LoginQRScan : UserControl, IPageChangeNotifier
	{
		public event EventHandler<PageChangeEventArgs> PageChangeRequested;

		private FilterInfoCollection videoDevices;
		private VideoCaptureDevice videoSource;
		private readonly object lockObject = new object();
		private bool qrReadBool = false;

		public LoginQRScan()
		{
			InitializeComponent();
		}

		private void LoginQRScan_Load(object sender, EventArgs e)
		{

            StartCamera();
			roundedPictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		private void StartCamera()
		{
			if (videoSource != null && videoSource.IsRunning)
			{
				StopCamera();
			}


			videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			if (videoDevices.Count == 0)
			{
				MessageBox.Show("카메라를 찾을 수 없습니다");
				return;
			}
			else
			{
				//OnLoginSuccessful();
				videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
				videoSource.NewFrame += video_NewFrame;
				videoSource.Start();

			}
		}

		private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			try
			{
				Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); // 이미지 90도 회전

				lock (lockObject)
				{
					roundedPictureBox1.Image?.Dispose();
					roundedPictureBox1.Image = (Bitmap)bitmap.Clone();
				}

				Task.Run(() => DecodeQRCodeAsync(bitmap));
			}
			catch (Exception ex)
			{

			}
			
			
		}

		private async Task DecodeQRCodeAsync(Bitmap bitmap)
		{
			try
			{
				string decodedText = await Task.Run(() => ReadQRCode(bitmap));
				if (!string.IsNullOrEmpty(decodedText))
				{
					Invoke(new Action(() =>
					{
						if (qrReadBool) { return; }
						qrReadBool = true;

						StopCamera(); // QR 코드 디코딩 후 카메라 멈추기
						OnLoginSuccessful(decodedText);
					}));
				}
			}
			catch (Exception ex)
			{
				StartCamera();
			}

			
		}

		protected async void OnLoginSuccessful(string qrString)
		{
            try
			{
				string response = await CallApiText(qrString);

				//this.Invoke(new Action(() => MovePage(typeof(MeasureForm))));
				//return;

				if (response != null && response.Equals("True"))
				{

					UserInfo usi = UserInfo.Instance;
					string userMessage = "\"" + usi.UserName + "\"님이신가요?";

					ConfirmPopup cfp = new ConfirmPopup(userMessage);

					cfp.ShowDialog();

					qrReadBool = false;

					if (cfp.Confirmed)
					{
						this.Invoke(new Action(() => MovePage(typeof(MeasureForm))));
					}
					else
					{
						StartCamera();
					}
				}
				else
				{
					LoginQRScanPopup lqsp = new LoginQRScanPopup();

					lqsp.ShowDialog();
					qrReadBool = false;

					StartCamera();
				}
			}
			catch (Exception e)
			{
				StartCamera();
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

		private async Task<string> CallApiText(string qrcode)
		{
			string apistr = "";
			ApiCallHelper apiCallHelper = new ApiCallHelper();

		#if DEBUG
			//string getUrl = Properties.Settings.Default.QR_CALL_URL_DEBUG + qrcode;
			string getUrl = Properties.Settings.Default.QR_CALL_URL_RELEASE + qrcode;
#else
			string getUrl = Properties.Settings.Default.QR_CALL_URL_RELEASE + qrcode;
#endif
			try
			{
				string getResponse = await apiCallHelper.GetAsync(getUrl);
				JObject json = JObject.Parse(getResponse);

				// Key와 Value 출력
				foreach (var pair in json)
				{
					string key = pair.Key;
					string value = pair.Value.ToString();

					Console.WriteLine($"Key: {key}, Value: {value}");
					apistr = value;
				}


				if (apistr.Equals("True"))
				{

					int separatorIndex = qrcode.IndexOf("2c8");

					if (separatorIndex != -1)
					{
						string key = qrcode.Substring(0, separatorIndex);
						string value = qrcode.Substring(separatorIndex + "2c8".Length);

						// key에서 앞의 14자리를 제거
						if (key.Length >= 14)
						{
							key = key.Substring(14); // 14자리 숫자 이후의 문자열을 추출
						}
						else
						{
							key = ""; // 만약 key가 14자리보다 짧다면 빈 문자열로 처리
						}

						Console.WriteLine("Key: " + key);
						Console.WriteLine("Value: " + value);

						UserInfo.Instance.SetUserInfo(key, value, 0);
					}
					else
					{
						Console.WriteLine("not found in the string.");
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return apistr;
		}

		private string ReadQRCode(Bitmap bitmap)
		{
			try
			{
				var barcodeReader = new BarcodeReader();
				var result = barcodeReader.Decode(bitmap);
				return result?.Text;
			}
			catch (Exception ex)
			{
				Invoke(new Action(() =>
				{
					MessageBox.Show($"Error decoding QR code: {ex.Message}");
				}));
				return null;
			}
		}

		private void StopCamera()
		{
			if (videoSource != null && videoSource.IsRunning)
			{
				videoSource.SignalToStop();
				videoSource.WaitForStop();
				videoSource.NewFrame -= video_NewFrame;
				videoSource = null;
			}
		}

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            qrReadBool = false;
			StopCamera();

            MovePage(typeof(LoginForm));
        }
    }
}
