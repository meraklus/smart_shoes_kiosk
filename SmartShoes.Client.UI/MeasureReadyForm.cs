using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartShoes.Common.Forms;

namespace SmartShoes.Client.UI
{
	public partial class MeasureReadyForm : UserControl, IPageChangeNotifier
	{

		public event EventHandler<PageChangeEventArgs> PageChangeRequested;
		private ShoesConnectPopup scp;

		public MeasureReadyForm()
		{
			InitializeComponent();
			btnNomal.Enabled = false;
			
		}

		private void btnNomal_Click(object sender, EventArgs e)
		{
			try
			{
                LoadingPopup loadpop = new LoadingPopup();
                loadpop.Show();
                Application.DoEvents();
                this.Invoke(new Action(() => MovePage(typeof(MeasureNomalSecond))));
                //MovePage(new MeasureNomalSecond());
                loadpop.Close();
            }
			catch (Exception ex){
                Console.WriteLine("ex에러");
                Console.WriteLine(ex);
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

		private async void pictureBox1_Click(object sender, EventArgs e)
		{
			Guid serviceUUID = new Guid(Properties.Settings.Default.SERVICE_UUID);
			Guid notifyUUID = new Guid(Properties.Settings.Default.NOTIFY_UUID);
			int datalen = Properties.Settings.Default.SENSOR_SET_TIME * 100;

			await BLEManager.Instance.DisconnectDevicesAsync(serviceUUID);

			btnNomal.Enabled = false;
			this.btnNomal.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_measu_continue_off_1;

			string deviceAddressL = "";
			string deviceAddressR = "";
			ShoesChoicePopup shoesChoicePopup = new ShoesChoicePopup();
			shoesChoicePopup.deviceAddressL = deviceAddressL;
			shoesChoicePopup.deviceAddressR = deviceAddressR;
			shoesChoicePopup.StartPosition = FormStartPosition.Manual; // 위치를 수동으로 설정
			int screenCenterX = (Screen.PrimaryScreen.WorkingArea.Width - shoesChoicePopup.Width) / 2;
			int customY = 100;
			shoesChoicePopup.Location = new System.Drawing.Point(screenCenterX, customY);
			shoesChoicePopup.ShowDialog();

			deviceAddressL = shoesChoicePopup.deviceAddressL;
			deviceAddressR = shoesChoicePopup.deviceAddressR;

			if (deviceAddressL.Equals("") || deviceAddressR.Equals(""))
			{
				return;
			}

			scp = new ShoesConnectPopup();
			scp.StartPosition = FormStartPosition.Manual; // 위치를 수동으로 설정
			int screenCenterX2 = (Screen.PrimaryScreen.WorkingArea.Width - scp.Width) / 2;
			int customY2 = 300;
			scp.Location = new System.Drawing.Point(screenCenterX2, customY2);
			scp.Show();

            //bool isConnected = await BLEManager.Instance.ConnectAndReceiveAsync3(deviceAddressR, deviceAddressL, datalen, serviceUUID, notifyUUID);

            await Task.Delay(5000); 

            bool isConnected = true;

            btnNomal.Enabled = isConnected;
			if( isConnected ) 
			{
				
				scp.Close();
				this.btnNomal.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_measu_continue;
			}
			else
			{
				scp.Close();
			}
			Console.WriteLine(isConnected);
		}

        private async void pictureBox4_Click(object sender, EventArgs e)
        {
            //Guid serviceUUID = new Guid(Properties.Settings.Default.SERVICE_UUID);
            //await BLEManager.Instance.DisconnectDevicesAsync(serviceUUID);

            MovePage(typeof(LoginForm));

        }
    }
}
