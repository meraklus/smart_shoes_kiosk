using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartShoes.Client.UI;
using SmartShoes.Common.Forms;

namespace SmartShoes
{
	public partial class LauncherForm : Form
	{
        private static RichTextBox logTextBox;


        public LauncherForm()
		{
			InitializeComponent();
			ShowInitialPage();


            //this.Load += LauncherForm_Load;

			// RichTextBox 초기화
			//logTextBox = new RichTextBox
   //         {
   //             Dock = DockStyle.Bottom,
   //             Height = 200,
   //             ReadOnly = true
   //         };

			//this.Controls.Add(logTextBox);
        }
        public static void LogMessage(string message)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(new Action(() => LogMessage(message)));
            }
            else
            {
                logTextBox.AppendText($"{DateTime.Now}: {message}\n");
                logTextBox.ScrollToCaret();
            }
        }


              private void LauncherForm_Load(object sender, EventArgs e)
		{ // 전체화면으로 표시할 모니터 선택 (예: 두 번째 모니터)
			Screen[] screens = Screen.AllScreens;
			if (screens.Length > 1)
			{
				Screen secondaryScreen = screens[1];
				this.Location = secondaryScreen.Bounds.Location;
				this.WindowState = FormWindowState.Normal;
				this.FormBorderStyle = FormBorderStyle.None;
				this.Bounds = secondaryScreen.Bounds;
			} // 전체화면 설정
			this.WindowState = FormWindowState.Maximized;
		}


		private void ShowInitialPage()
		{
			LoginForm loginForm = new SmartShoes.Client.UI.LoginForm();
			//MeasureResultForm loginForm = new SmartShoes.Client.UI.MeasureResultForm();

			loginForm.Dock = DockStyle.Fill;
			this.panel1.Controls.Add(loginForm);

			loginForm.PageChangeRequested += UserControl_PageChangeRequested;
		}

		// 공통 이벤트 처리 메서드
		private void UserControl_PageChangeRequested(object sender, PageChangeEventArgs e)
		{
			
			if (this.panel1.InvokeRequired)
			{
				this.panel1.Invoke(new Action(() => UserControl_PageChangeRequested(sender, e)));
			}
			else
			{
				ShowPage(e.NextPage);
			}
			
		}

		// 공통 페이지 전환 메서드
		//private void ShowPage(Control page)
		//{
		//	if (this.panel1.InvokeRequired)
		//	{
		//		this.panel1.Invoke(new Action(() => ShowPage(page)));
		//	}
		//	else
		//	{
		//		// 이전 페이지에서 이벤트 핸들러 제거
		//		if (this.panel1.Controls.Count > 0 && this.panel1.Controls[0] is IPageChangeNotifier previousNotifier)
		//		{
		//			previousNotifier.PageChangeRequested -= UserControl_PageChangeRequested;
		//		}

		//		page.Dock = DockStyle.Fill;
		//		this.panel1.Controls.Clear();
		//		this.panel1.Controls.Add(page);

		//		// 새로운 페이지에서 이벤트 핸들러 추가
		//		if (page is IPageChangeNotifier notifier)
		//		{
		//			notifier.PageChangeRequested += UserControl_PageChangeRequested;
		//		}
		//	}
		//}

		private void ShowPage(Control page)
		{
			if (this.panel1.InvokeRequired)
			{
				this.panel1.Invoke(new Action(() => ShowPage(page)));
			}
			else
			{
				// 이전 페이지에서 이벤트 핸들러 제거
				if (this.panel1.Controls.Count > 0 && this.panel1.Controls[0] is IPageChangeNotifier previousNotifier)
				{
					previousNotifier.PageChangeRequested -= UserControl_PageChangeRequested;
				}

				page.Dock = DockStyle.None;
				this.panel1.Controls.Clear();
				this.panel1.Controls.Add(page);

				// 페이지가 IPageChangeNotifier를 구현한 경우 이벤트 핸들러 추가
				if (page is IPageChangeNotifier notifier)
				{
					// 중복 등록 방지
					notifier.PageChangeRequested -= UserControl_PageChangeRequested;
					notifier.PageChangeRequested += UserControl_PageChangeRequested;
				}

				// 페이지 상태 초기화
				if (page is IPageInitializable initializablePage)
				{
					initializablePage.InitializePage(); // 페이지 초기화 메서드 호출
				}
			}
		}

		private void LauncherForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Application.ExitThread();
			Application.Exit();
		}
	}
}
