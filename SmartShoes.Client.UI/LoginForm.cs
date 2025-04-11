using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Renci.SshNet.Security;
using SmartShoes.Common.Forms;

namespace SmartShoes.Client.UI
{
	public partial class LoginForm : UserControl, IPageChangeNotifier
	{
		public event EventHandler<PageChangeEventArgs> PageChangeRequested;

		public LoginForm()
		{
			InitializeComponent();
			UserInfo.Instance.ClearUserInfo();
			MatDataManager.Instance.ResetMatData();

			Console.WriteLine("LoginForm 생성");
			
			// BLEManager 초기화
			try
			{
				// 인스턴스가 없을 때만 초기화
				if (!BLEManager.HasInstance())
				{
					string leftMac = Properties.Settings.Default.BLE_LEFT_MAC_ADDRESS;
					string rightMac = Properties.Settings.Default.BLE_RIGHT_MAC_ADDRESS;
					int time = Properties.Settings.Default.SENSOR_SET_TIME;
					
					// 설정 값이 있을 때만 초기화
					if (!string.IsNullOrEmpty(leftMac) && !string.IsNullOrEmpty(rightMac) && time > 0)
					{
						Console.WriteLine($"BLEManager 초기화: 왼쪽={leftMac}, 오른쪽={rightMac}, 시간={time}초");
						BLEManager.Instance.InitializeConnection(leftMac, rightMac, time);
					}
					else
					{
						Console.WriteLine("BLEManager 초기화: 설정 값이 없어 초기화를 건너뜁니다.");
					}
				}
				else
				{
					Console.WriteLine("BLEManager 이미 초기화되어 있습니다.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"BLEManager 초기화 오류: {ex.Message}");
			}

			UserInfo uif = UserInfo.Instance;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			// QR 스캔 페이지로 이동
			MovePage(typeof(LoginQRScan));
			//MovePage(typeof(MeasureForm));
			//MovePage(typeof(MeasureReadyForm));
			//MovePage(typeof(MeasureResultForm2));
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			// 설정 페이지로 이동
			MovePage(typeof(SettingsForm));
		}

		protected void MovePage(Type pageType)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Action(() => PageChangeRequested?.Invoke(this, new PageChangeEventArgs(pageType))));
			}
			else
			{

				if (PageChangeRequested != null)
				{
					// 이벤트 트리거
					PageChangeRequested(this, new PageChangeEventArgs(pageType));
				}
				//PageChangeRequested?.Invoke(this, new PageChangeEventArgs(pageType));
			}
		}

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            using (KeyInputForm keyInputForm = new KeyInputForm())
            {
                var result = keyInputForm.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrEmpty(keyInputForm.InputKey))
                {
                    if (int.TryParse(keyInputForm.InputKey, out int inputKey))
                    {
                        // 입력된 키 값을 SetUserInfo에 전달
                        UserInfo.Instance.SetUserInfo("테스터", "68", inputKey ,240, "여성", "1991-11-11" );

                        // MeasureForm으로 이동
                        MovePage(typeof(MeasureForm));
                    }
                }
            }
        }
    }
}
