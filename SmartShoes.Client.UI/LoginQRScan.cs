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
using System.Text;

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
			Console.WriteLine("1111");
            InitializeComponent();
		}

		private void LoginQRScan_Load(object sender, EventArgs e)
		{
            Console.WriteLine("3333");
            StartCamera();
			roundedPictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Console.WriteLine("4444");
        }

		private void StartCamera()
		{
            Console.WriteLine("5555");
            if (videoSource != null && videoSource.IsRunning)
			{
                Console.WriteLine("5-1-5-1-5-1-5-1");
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
                Console.WriteLine("6666");
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
                //Console.WriteLine("7777");
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
				bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone); // 이미지 90도 회전

				lock (lockObject)
				{
                    //Console.WriteLine("8888");
                    roundedPictureBox1.Image?.Dispose();
                    roundedPictureBox1.Image = new Bitmap(bitmap, roundedPictureBox1.Size); // 프레임을 PictureBox 크기에 맞게 조정
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
                    Console.WriteLine("9999");

                    Invoke(new Action(() =>
					{
						if (qrReadBool) { return; }
						qrReadBool = true;
                        Console.WriteLine("101010");
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
                Console.WriteLine("11@11@11@11");
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
            Console.WriteLine("12@12@12@12@");
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
				// URL 디코딩을 통해 한글 처리 (Uri.UnescapeDataString 사용)
				string decodedQrCode = Uri.UnescapeDataString(qrcode);
				Console.WriteLine($"디코딩된 QR 코드: {decodedQrCode}");
				
				// 디코딩된 QR 코드로 API 호출
				string getResponse = await apiCallHelper.GetAsync(getUrl);
				JObject json = JObject.Parse(getResponse);

				// Key와 Value 출력
				foreach (var pair in json)
				{
					string key = pair.Key;
					string value = pair.Value.ToString();

					Console.WriteLine($"Key: {key}, Value: {value}");
					
					// QrCk 키인 경우에만 apistr에 값을 할당
					if (key == "qrCk")
					{
						apistr = value;
					}
				}


				if (apistr.Equals("True"))
				{
					int separatorIndex = decodedQrCode.IndexOf("2c8");

					if (separatorIndex != -1)
					{
						string key = decodedQrCode.Substring(0, separatorIndex);
						string value = decodedQrCode.Substring(separatorIndex + "2c8".Length);

						// key에서 앞의 14자리를 제거
						if (key.Length >= 14)
						{
							key = key.Substring(14); // 14자리 숫자 이후의 문자열을 추출
						}
						else
						{
							key = ""; // 만약 key가 14자리보다 짧다면 빈 문자열로 처리
						}

						Console.WriteLine("Key (디코딩됨): " + key);
						Console.WriteLine("Value: " + value);
						
						// QR 코드 응답 데이터 파싱
						// QrResponseDto 형식: { QrCk: boolean, name: string, gender: string, birthYear: number, shoeSize: number, height: number }
						try
						{
							
							// 사용자 정보 추출
							string name = json["name"]?.ToString() ?? "";
							string gender = json["gender"]?.ToString() ?? "";
							string birthday = json["birthday"]?.ToString();
							int shoeSize = json["shoeSize"]?.Value<int>() ?? 0;
                            int height = json["height"]?.Value<int>() ?? 0;
							
							// 성별 변환 (문자열 -> 숫자)
							string sexText = "남성"; // 기본값
							if (gender.ToLower() == "male" || gender.ToLower() == "남성" || gender.ToLower() == "M")
								sexText = "남성";
							else if (gender.ToLower() == "female" || gender.ToLower() == "여성" || gender.ToLower() == "F")
								sexText = "여성";
							
							Console.WriteLine($"파싱된 사용자 정보: 이름={name}, 성별={sexText}, 생년월일={birthday}, 신발사이즈={shoeSize}, 키={height}");
							
							// UserInfo에 데이터 저장
							UserInfo.Instance.SetUserInfo(name, value, (int)height, (int)shoeSize, sexText, birthday);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"QR 응답 파싱 오류: {ex.Message}");
							// 파싱 실패 시 기본값으로 설정
							//UserInfo.Instance.SetUserInfo(key, value, 0);
						}
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
            Console.WriteLine("!!!!!@@@@@");
            if (videoSource != null && videoSource.IsRunning)
			{
                Console.WriteLine("!!!!!@@@@@22222");
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
