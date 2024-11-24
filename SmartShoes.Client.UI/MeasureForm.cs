using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartShoes.Common.Forms;

namespace SmartShoes.Client.UI
{
	public partial class MeasureForm : UserControl, IPageChangeNotifier
	{

		public event EventHandler<PageChangeEventArgs> PageChangeRequested;

		public MeasureForm()
		{
			InitializeComponent();

		}
		
		private async void btnNomal_Click(object sender, EventArgs e)
		{
			LoadingPopup loadingPopup = new LoadingPopup();

			loadingPopup.Show();
			Application.DoEvents();

			// 백그라운드 작업 실행
			Type nextPageType = await Task.Run(() =>
			{
				// 여기서 오래 걸리는 작업이 있으면 넣어줍니다.
				return typeof(MeasureNomalFirst);
				
			});

			// UI 스레드에서 페이지 이동 처리
			MovePage(nextPageType);

			loadingPopup.Close();
		}


		private void btnTiny_Click(object sender, EventArgs e)
		{
			LoadingPopup loadingPopup = new LoadingPopup();
			loadingPopup.Show();
			Application.DoEvents();

			this.Invoke(new Action(() => MovePage(typeof(MeasureReadyForm))));
			//MovePage(new MeasureTiny());
			loadingPopup.Close();
		}

		protected void MovePage(Type ctr)
		{
			PageChangeRequested?.Invoke(this, new PageChangeEventArgs(ctr));
		}

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            // 얼럿 메시지 박스 표시
        
                UserInfo.Instance.ClearUserInfo();

                MovePage(typeof(LoginForm));
            
        }
	}
}
