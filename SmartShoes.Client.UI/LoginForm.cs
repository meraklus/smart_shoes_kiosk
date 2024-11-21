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

			List<MatData> lstmd = MatDataManager.Instance.GetAllMatData();
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
                        UserInfo.Instance.SetUserInfo("테스터", "68", inputKey);

                        // MeasureForm으로 이동
                        MovePage(typeof(MeasureForm));
                    }
                }
            }
        }
    }
}
