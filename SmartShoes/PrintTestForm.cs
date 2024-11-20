using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;
using static SmartShoes.TotalProcessForm;

namespace SmartShoes
{
	public partial class PrintTestForm : Form
	{
		private UserData oUserData = null;
		private Dictionary<string, object> oMatData = null;

		public PrintTestForm(UserData oUserData, Dictionary<string, object> oMatData)
		{
			InitializeComponent();

			this.oUserData = oUserData;
			this.oMatData = oMatData;

			#region ::: 매트 데이터 위치 지정 :::
			this.label1.Parent = pictureBox2;
			this.label1.Location = new Point(50, 105);
			this.label2.Parent = pictureBox2;
			this.label2.Location = new Point(70, 173);
			this.label3.Parent = pictureBox2;
			this.label3.Location = new Point(90, 213);
			this.label4.Parent = pictureBox2;
			this.label4.Location = new Point(100, 140);
			this.label5.Parent = pictureBox2;
			this.label5.Location = new Point(140, 173);

			// 신체 균형 상태
			this.label7.Parent = pictureBox3;
			this.label7.Location = new Point(27, 280);
			this.label6.Parent = pictureBox3;
			this.label6.Location = new Point(146, 280);

			//// 지하이웰 매트 데이터
			//// left
			this.label8.Parent = pictureBox4;
			this.label8.Location = new Point(180, 37);
			this.label9.Parent = pictureBox4;
			this.label9.Location = new Point(180, 93);
			this.label10.Parent = pictureBox4;
			this.label10.Location = new Point(180, 149);
			this.label11.Parent = pictureBox4;
			this.label11.Location = new Point(180, 177);
			this.label12.Parent = pictureBox4;
			this.label12.Location = new Point(180, 205);
			this.label13.Parent = pictureBox4;
			this.label13.Location = new Point(180, 233);
			this.label14.Parent = pictureBox4;
			this.label14.Location = new Point(180, 261);

			//// right
			this.label15.Parent = pictureBox4;
			this.label15.Location = new Point(280, 261);
			this.label16.Parent = pictureBox4;
			this.label16.Location = new Point(280, 233);
			this.label17.Parent = pictureBox4;
			this.label17.Location = new Point(280, 205);
			this.label18.Parent = pictureBox4;
			this.label18.Location = new Point(280, 177);
			this.label19.Parent = pictureBox4;
			this.label19.Location = new Point(280, 149);
			this.label20.Parent = pictureBox4;
			this.label20.Location = new Point(280, 93);
			this.label21.Parent = pictureBox4;
			this.label21.Location = new Point(280, 37);
			// third
			this.label22.Parent = pictureBox4;
			this.label22.Location = new Point(380, 65);
			this.label23.Parent = pictureBox4;
			this.label23.Location = new Point(380, 121);
			#endregion
		}

		private void PrintTestForm_Load(object sender, EventArgs e)
		{

			this.label24.Text = oUserData.Name;
			this.label25.Text = oUserData.Birth ;
			this.label26.Text = oUserData.Gender;
			this.label27.Text = oUserData.MeasureTime;
			this.label28.Text = oUserData.Height;
			this.label29.Text = oUserData.Weight;
			this.label30.Text = oUserData.ShoeSize;

			this.label1.Text = Convert.ToDouble(oMatData["StrideLength4"]).ToString("F0") + "cm";
			this.label2.Text = Convert.ToDouble(oMatData["StepLength1"]).ToString("F0") + "cm";
			this.label3.Text = Convert.ToDouble(oMatData["StepAngle1"]).ToString("F0") + "˚";
			this.label4.Text = Convert.ToDouble(oMatData["StepAngle2"]).ToString("F0") + "˚";
			this.label5.Text = Convert.ToDouble(oMatData["StepLength2"]).ToString("F0") + "cm";

			this.label7.Text = Convert.ToDouble(oMatData["StepForce1"]).ToString("F0") + "%";
			this.label6.Text = Convert.ToDouble(oMatData["StepForce2"]).ToString("F0") + "%";

			this.label8.Text = Convert.ToDouble(oMatData["StepLength1"]).ToString("F1");
			this.label9.Text = Convert.ToDouble(oMatData["StepAngle1"]).ToString("F1");
			this.label10.Text = Convert.ToDouble(oMatData["StepForce1"]).ToString("F1");
			this.label11.Text = Convert.ToDouble(oMatData["StancePhase1"]).ToString("F1");
			this.label12.Text = Convert.ToDouble(oMatData["SwingPhase1"]).ToString("F1");
			this.label13.Text = Convert.ToDouble(oMatData["StrideTime1"]).ToString("F1");
			this.label14.Text = Convert.ToDouble(oMatData["CopLength1"]).ToString("F1");

			this.label21.Text = Convert.ToDouble(oMatData["StepLength2"]).ToString("F1");
			this.label20.Text = Convert.ToDouble(oMatData["StepAngle2"]).ToString("F1");
			this.label19.Text = Convert.ToDouble(oMatData["StepForce2"]).ToString("F1");
			this.label18.Text = Convert.ToDouble(oMatData["StancePhase2"]).ToString("F1");
			this.label17.Text = Convert.ToDouble(oMatData["SwingPhase2"]).ToString("F1");
			this.label16.Text = Convert.ToDouble(oMatData["StrideTime2"]).ToString("F1");
			this.label15.Text = Convert.ToDouble(oMatData["CopLength2"]).ToString("F1");

			this.label22.Text = Convert.ToDouble(oMatData["StrideLength4"]).ToString("F1");
			this.label23.Text = Convert.ToDouble(oMatData["BaseOfGait4"]).ToString("F1");

		}

		private void btnCameraStart_Click(object sender, EventArgs e)
		{
			//return;
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

			// PrintDialog 생성 및 설정
			PrintDialog printDialog = new PrintDialog
			{
				Document = printDocument,
				AllowSomePages = true,
				ShowHelp = true
			};

			// 프린트 다이얼로그 표시
			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				printDocument.Print();
			}
		}

		private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			// 패널을 캡처합니다.
			Bitmap bmp = CapturePanel(this.panel1); // yourTargetPanel은 출력하려는 패널입니다.

			// 프린터의 페이지 크기에 맞게 비트맵의 크기를 조정합니다.
			float scale = Math.Min((float)e.PageBounds.Width / bmp.Width, (float)e.PageBounds.Height / bmp.Height);

			// 크기 조정된 이미지 출력 위치를 계산합니다.
			int scaledWidth = (int)(bmp.Width * scale);
			int scaledHeight = (int)(bmp.Height * scale);
			int posX = (e.PageBounds.Width - scaledWidth) / 2;
			int posY = (e.PageBounds.Height - scaledHeight) / 2;

			// 이미지를 프린터 페이지에 출력합니다.
			e.Graphics.DrawImage(bmp, posX, posY, scaledWidth, scaledHeight);
		}

		private Bitmap CapturePanel(Panel targetPanel)
		{
			// 패널의 크기만큼 비트맵 생성
			Bitmap bmp = new Bitmap(targetPanel.ClientSize.Width, targetPanel.ClientSize.Height);

			// 비트맵에 패널의 내용을 그립니다.
			targetPanel.DrawToBitmap(bmp, new Rectangle(0, 0, targetPanel.Width, targetPanel.Height));

			return bmp;
		}

		
	}
}
