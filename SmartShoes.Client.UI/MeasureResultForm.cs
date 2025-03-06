using System;
using System.Drawing;
using System.Windows.Forms;
using SmartShoes.Common.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartShoes.Client.UI
{
	public partial class MeasureResultForm : UserControl, IPageChangeNotifier
	{
		

		public event EventHandler<PageChangeEventArgs> PageChangeRequested;
		private MeasurementData measurementData = null;

		
		public MeasureResultForm()
		{
			InitializeComponent();

			// #region ::: Position label in PictureBox :::
			// this.labelTopL1.Parent = pictureBox7;
			// this.labelTopL1.Location = new Point(70, 8);
			// this.txtTopL1.Parent = pictureBox7;
			// this.txtTopL1.Location = new Point(170, 8);

			// this.labelTopL2.Parent = pictureBox7;
			// this.labelTopL2.Location = new Point(290, 8);
			// this.txtTopL2.Parent = pictureBox7;
			// this.txtTopL2.Location = new Point(390, 8);

			// this.labelTopR1.Parent = pictureBox8;
			// this.labelTopR1.Location = new Point(70, 8);
			// this.txtTopR1.Parent = pictureBox8;
			// this.txtTopR1.Location = new Point(170, 8);

			// this.labelTopR2.Parent = pictureBox8;
			// this.labelTopR2.Location = new Point(290, 8);
			// this.txtTopR2.Parent = pictureBox8;
			// this.txtTopR2.Location = new Point(390, 8);

			// this.pictureBoxGrade.Parent = pictureBox3;
			// //this.pictureBoxGrade.Location = new Point(290, 8);
			// this.pictureBoxGrade.Location = new Point(0, 8);
			// this.txtGrade.Parent = pictureBoxGrade;
			// this.txtGrade.Location = new Point(5, 22);


			// // 보행 보폭 보행각도
			// this.txtMiddle1.Parent = pictureBox1;
			// this.txtMiddle1.Location = new Point(50, 70);
			// this.txtMiddle2.Parent = pictureBox1;
			// this.txtMiddle2.Location = new Point(100, 260);
			// this.txtMiddle3.Parent = pictureBox1;
			// this.txtMiddle3.Location = new Point(125, 331);
			// this.txtMiddle4.Parent = pictureBox1;
			// this.txtMiddle4.Location = new Point(150, 188);
			// this.txtMiddle5.Parent = pictureBox1;
			// this.txtMiddle5.Location = new Point(160, 70);

			// // 신체 균형 상태
			// this.labelMiddleR1.Parent = pictureBox4;
			// this.labelMiddleR1.Location = new Point(20, 280);
			// this.txtMiddleR1.Parent = pictureBox4;
			// this.txtMiddleR1.Location = new Point(0, 310);
			
			// this.labelMiddleR2.Parent = pictureBox4;
			// this.labelMiddleR2.Location = new Point(230, 280);
			// this.txtMiddleR2.Parent = pictureBox4;
			// this.txtMiddleR2.Location = new Point(223, 310);

			// // 지하이웰 매트 데이터
			// // left
			// this.txtBottom1.Parent = pictureBox2;
			// this.txtBottom1.Location = new Point(407, 85);
			// this.txtBottom2.Parent = pictureBox2;
			// this.txtBottom2.Location = new Point(407, 133);
			// this.txtBottom3.Parent = pictureBox2;
			// this.txtBottom3.Location = new Point(407, 178);
			// this.txtBottom4.Parent = pictureBox2;
			// this.txtBottom4.Location = new Point(407, 223);
			// this.txtBottom5.Parent = pictureBox2;
			// this.txtBottom5.Location = new Point(407, 270);
			// this.txtBottom6.Parent = pictureBox2;
			// this.txtBottom6.Location = new Point(407, 318);
			// // center
			// this.txtBottom7.Parent = pictureBox2;
			// this.txtBottom7.Location = new Point(522, 85);
			// this.txtBottom8.Parent = pictureBox2;
			// this.txtBottom8.Location = new Point(522, 133);
			// this.txtBottom9.Parent = pictureBox2;
			// this.txtBottom9.Location = new Point(522, 178);
			// this.txtBottom10.Parent = pictureBox2;
			// this.txtBottom10.Location = new Point(522, 223);
			// this.txtBottom11.Parent = pictureBox2;
			// this.txtBottom11.Location = new Point(522, 270);
			// this.txtBottom12.Parent = pictureBox2;
			// this.txtBottom12.Location = new Point(522, 318);
			// // right
			// this.txtBottom13.Parent = pictureBox2;
			// this.txtBottom13.Location = new Point(635, 85);
			// this.txtBottom14.Parent = pictureBox2;
			// this.txtBottom14.Location = new Point(635, 133);
			// this.txtBottom15.Parent = pictureBox2;
			// this.txtBottom15.Location = new Point(635, 178);
			// this.txtBottom16.Parent = pictureBox2;
			// this.txtBottom16.Location = new Point(635, 223);
			// this.txtBottom17.Parent = pictureBox2;
			// this.txtBottom17.Location = new Point(635, 270);
			// this.txtBottom18.Parent = pictureBox2;
			// this.txtBottom18.Location = new Point(635, 318);
			
			// this.label22.Parent = pictureBox2;
			// this.label22.Location = new Point(385, 425);
			// this.label23.Parent = pictureBox2;
			// this.label23.Location = new Point(552, 425);
			// #endregion

			// List<MatData> lstmd = MatDataManager.Instance.GetMatData();
			// string userName = UserInfo.Instance.UserName;
			// string userId = UserInfo.Instance.UserId;
			// this.txtUserName.Text = userName + "님의 결과";
			// this.txtDate.Text = "측정일 : " + DateTime.Now.ToShortDateString();
			// this.txtGrade.Text = 1 + "등급";

			// if (lstmd != null)
			// {
			// 	txtTopL1.Text = lstmd.StancePhase1.ToString("F0");
			// 	txtTopL2.Text = lstmd.SwingPhase1.ToString("F0");
			// 	txtTopR1.Text = lstmd.StancePhase2.ToString("F0");
			// 	txtTopR2.Text = lstmd.SwingPhase2.ToString("F0");

			// 	txtMiddle1.Text = lstmd.StrideLength4.ToString("F0") + "cm";
			// 	txtMiddle2.Text = lstmd.StepLength1.ToString("F0") + "cm";
			// 	txtMiddle3.Text = lstmd.StepAngle1.ToString("F0") + "˚";
			// 	txtMiddle4.Text = lstmd.StepAngle2.ToString("F0") + "˚";
			// 	txtMiddle5.Text = lstmd.StepLength2.ToString("F0") + "cm";

			// 	txtMiddleR1.Text = lstmd.StepForce2.ToString("F0") + "%";
			// 	txtMiddleR2.Text = lstmd.StepForce1.ToString("F0") + "%";

			// 	// 표데이터 
			// 	txtBottom1.Text = lstmd.StepLength1.ToString("F2");
			// 	txtBottom2.Text = lstmd.SingleStepTime1.ToString("F2");
			// 	txtBottom3.Text = lstmd.StrideTime1.ToString("F2");
			// 	txtBottom4.Text = lstmd.StepAngle1.ToString("F2");
			// 	txtBottom5.Text = lstmd.StepForce1.ToString("F2");
			// 	txtBottom6.Text = lstmd.BaseOfGait4.ToString("F2");

			// 	txtBottom7.Text = lstmd.StepLength2.ToString("F2");
			// 	txtBottom8.Text = lstmd.SingleStepTime2.ToString("F2");
			// 	txtBottom9.Text = lstmd.StrideTime2.ToString("F2");
			// 	txtBottom10.Text = lstmd.StepAngle2.ToString("F2");
			// 	txtBottom11.Text = lstmd.StepForce2.ToString("F2");
			// 	txtBottom12.Text = lstmd.BaseOfGait4.ToString("F2");
				
			// 	label22.Text = lstmd.CopLength1.ToString("F2");
			// 	label23.Text = lstmd.CopLength2.ToString("F2");

			// 	int userid = UserInfo.Instance.UserId == null ? 0 : Convert.ToInt32(UserInfo.Instance.UserId);

			// 	measurementData = new MeasurementData
			// 	{
			// 		userSid = userid,
			// 		steplength1 = lstmd.StepLength1,
			// 		steplength2 = lstmd.StepLength2,
			// 		steplength3 = lstmd.StepLength3,
			// 		steplength4 = lstmd.StepLength4,
			// 		stridelength1 = lstmd.StrideLength1,
			// 		stridelength2 = lstmd.StrideLength2,
			// 		stridelength3 = lstmd.StrideLength3,
			// 		stridelength4 = lstmd.StrideLength4,
			// 		singlesteptime1 = lstmd.SingleStepTime1,
			// 		singlesteptime2 = lstmd.SingleStepTime2,
			// 		singlesteptime3 = lstmd.SingleStepTime3,
			// 		singlesteptime4 = lstmd.SingleStepTime4,
			// 		stepangle1 = lstmd.StepAngle1,
			// 		stepangle2 = lstmd.StepAngle2,
			// 		stepangle3 = lstmd.StepAngle3,
			// 		stepangle4 = lstmd.StepAngle4,
			// 		stepcount1 = lstmd.StepCount1,
			// 		stepcount2 = lstmd.StepCount2,
			// 		stepcount3 = lstmd.StepCount3,
			// 		stepcount4 = lstmd.StepCount4,
			// 		baseofgait1 = lstmd.BaseOfGait1,
			// 		baseofgait2 = lstmd.BaseOfGait2,
			// 		baseofgait3 = lstmd.BaseOfGait3,
			// 		baseofgait4 = lstmd.BaseOfGait4,
			// 		stepforce1 = lstmd.StepForce1,
			// 		stepforce2 = lstmd.StepForce2,
			// 		stepforce3 = lstmd.StepForce3,
			// 		stepforce4 = lstmd.StepForce4,
			// 		stancephase1 = lstmd.StancePhase1,
			// 		stancephase2 = lstmd.StancePhase2,
			// 		stancephase3 = lstmd.StancePhase3,
			// 		stancephase4 = lstmd.StancePhase4,
			// 		swingphase1 = lstmd.SwingPhase1,
			// 		swingphase2 = lstmd.SwingPhase2,
			// 		swingphase3 = lstmd.SwingPhase3,
			// 		swingphase4 = lstmd.SwingPhase4,
			// 		singlesupport1 = lstmd.SingleSupport1,
			// 		singlesupport2 = lstmd.SingleSupport2,
			// 		singlesupport3 = lstmd.SingleSupport3,
			// 		singlesupport4 = lstmd.SingleSupport4,
			// 		totaldoublesupport1 = lstmd.TotalDoubleSupport1,
			// 		totaldoublesupport2 = lstmd.TotalDoubleSupport2,
			// 		totaldoublesupport3 = lstmd.TotalDoubleSupport3,
			// 		totaldoublesupport4 = lstmd.TotalDoubleSupport4,
			// 		loadresponce1 = lstmd.LoadResponce1,
			// 		loadresponce2 = lstmd.LoadResponce2,
			// 		loadresponce3 = lstmd.LoadResponce3,
			// 		loadresponce4 = lstmd.LoadResponce4,
			// 		preswing1 = lstmd.PreSwing1,
			// 		preswing2 = lstmd.PreSwing2,
			// 		preswing3 = lstmd.PreSwing3,
			// 		preswing4 = lstmd.PreSwing4,
			// 		stepposition1 = lstmd.StepPosition1,
			// 		stepposition2 = lstmd.StepPosition2,
			// 		stepposition3 = lstmd.StepPosition3,
			// 		stepposition4 = lstmd.StepPosition4,
			// 		stridetime1 = lstmd.StrideTime1,
			// 		stridetime2 = lstmd.StrideTime2,
			// 		stridetime3 = lstmd.StrideTime3,
			// 		stridetime4 = lstmd.StrideTime4,
			// 		stancetime1 = lstmd.StanceTime1,
			// 		stancetime2 = lstmd.StanceTime2,
			// 		stancetime3 = lstmd.StanceTime3,
			// 		stancetime4 = lstmd.StanceTime4,
			// 		coplength1 = lstmd.CopLength1,
			// 		coplength2 = lstmd.CopLength2,
			// 		coplength3 = lstmd.CopLength3,
			// 		coplength4 = lstmd.CopLength4
			// 	};
			// 	double userHeight = Convert.ToDouble(UserInfo.Instance.Height);
			// 	Dictionary<string, string> result1 = GaitAnalysis.Algorithm1(lstmd, userHeight);
			// 	Dictionary<string, string> result2 = GaitAnalysis.Algorithm2(lstmd);
			// 	Dictionary<string, double> result3 = GaitAnalysis.Standard1(lstmd);

			// 	foreach(KeyValuePair<string, string> pair in result1)
			// 	{
			// 		string key = pair.Key;
			// 		string value = pair.Value;
			// 		Console.WriteLine("result1 : " + key + "=" + value);
			// 	}

			// 	foreach (KeyValuePair<string, string> pair in result2)
			// 	{
			// 		string key = pair.Key;
			// 		string value = pair.Value;
			// 		Console.WriteLine("result2 : " + key + "=" + value);
			// 	}

			// 	foreach (KeyValuePair<string, double> pair in result3)
			// 	{
			// 		string key = pair.Key;
			// 		double value = pair.Value;
			// 		Console.WriteLine("result3 : " + key + "=" + value);
			// 	}

			// };
		}

		private async Task<string> CallApiText()
		{
			string apistr = "";
			ApiCallHelper apiCallHelper = new ApiCallHelper();

			// GET 요청 예제 
			string getUrl = "http://221.161.177.193:8080/api/basedata";
			//string getUrl = "http://192.168.0.41:8080/api/basedata";
			try
			{
				string getResponse = await apiCallHelper.PostAsync(getUrl, this.measurementData);
				JObject json = JObject.Parse(getResponse);

				// Key와 Value 출력
				foreach (var pair in json)
				{
					string key = pair.Key;
					string value = pair.Value.ToString();

					Console.WriteLine($"Key: {key}, Value: {value}");
					apistr = value;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return apistr;
		}


		private void btnNomal_Click(object sender, EventArgs e)
		{
			this.Invoke(new Action(() => MovePage(typeof(MeasureNomalFirst))));
			//MovePage(new MeasureNomalFirst());
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


		// PrintDocument의 PrintPage 이벤트 핸들러
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

		private void btnTiny_Click(object sender, EventArgs e)
		{

			// var ls = BLEManager.Instance._parsedDataL;
			// var rs = BLEManager.Instance._parsedDataR;


			// //return;
			// PrintDocument printDocument = new PrintDocument();
			// printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

			// // PrintDialog 생성 및 설정
			// PrintDialog printDialog = new PrintDialog
			// {
			// 	Document = printDocument,
			// 	AllowSomePages = true,
			// 	ShowHelp = true
			// };

			// // 프린트 다이얼로그 표시
			// if (printDialog.ShowDialog() == DialogResult.OK)
			// {
			// 	printDocument.Print();
			// }

			// ConfirmPopup cfp = new ConfirmPopup("측정을 종료하시겠습니까?");
			// cfp.ShowDialog();


			// if (cfp.Confirmed)
			// {
			// 	this.Invoke(new Action(() => MovePage(typeof(LoginForm))));
			// }
			

		}

		private async void MeasureResultForm_Load(object sender, EventArgs e)
		{
			await CallApiText();
		}

		#region :: 측정데이터 클래스 ::
		private class MeasurementData
		{
			public int userSid { get; set; }
			public double steplength1 { get; set; }
			public double steplength2 { get; set; }
			public double steplength3 { get; set; }
			public double steplength4 { get; set; }
			public double stridelength1 { get; set; }
			public double stridelength2 { get; set; }
			public double stridelength3 { get; set; }
			public double stridelength4 { get; set; }
			public double singlesteptime1 { get; set; }
			public double singlesteptime2 { get; set; }
			public double singlesteptime3 { get; set; }
			public double singlesteptime4 { get; set; }
			public double stepangle1 { get; set; }
			public double stepangle2 { get; set; }
			public double stepangle3 { get; set; }
			public double stepangle4 { get; set; }
			public double stepcount1 { get; set; }
			public double stepcount2 { get; set; }
			public double stepcount3 { get; set; }
			public double stepcount4 { get; set; }
			public double baseofgait1 { get; set; }
			public double baseofgait2 { get; set; }
			public double baseofgait3 { get; set; }
			public double baseofgait4 { get; set; }
			public double stepforce1 { get; set; }
			public double stepforce2 { get; set; }
			public double stepforce3 { get; set; }
			public double stepforce4 { get; set; }
			public double stancephase1 { get; set; }
			public double stancephase2 { get; set; }
			public double stancephase3 { get; set; }
			public double stancephase4 { get; set; }
			public double swingphase1 { get; set; }
			public double swingphase2 { get; set; }
			public double swingphase3 { get; set; }
			public double swingphase4 { get; set; }
			public double singlesupport1 { get; set; }
			public double singlesupport2 { get; set; }
			public double singlesupport3 { get; set; }
			public double singlesupport4 { get; set; }
			public double totaldoublesupport1 { get; set; }
			public double totaldoublesupport2 { get; set; }
			public double totaldoublesupport3 { get; set; }
			public double totaldoublesupport4 { get; set; }
			public double loadresponce1 { get; set; }
			public double loadresponce2 { get; set; }
			public double loadresponce3 { get; set; }
			public double loadresponce4 { get; set; }
			public double preswing1 { get; set; }
			public double preswing2 { get; set; }
			public double preswing3 { get; set; }
			public double preswing4 { get; set; }
			public double stepposition1 { get; set; }
			public double stepposition2 { get; set; }
			public double stepposition3 { get; set; }
			public double stepposition4 { get; set; }
			public double stridetime1 { get; set; }
			public double stridetime2 { get; set; }
			public double stridetime3 { get; set; }
			public double stridetime4 { get; set; }
			public double stancetime1 { get; set; }
			public double stancetime2 { get; set; }
			public double stancetime3 { get; set; }
			public double stancetime4 { get; set; }
			public double coplength1 { get; set; }
			public double coplength2 { get; set; }
			public double coplength3 { get; set; }
			public double coplength4 { get; set; }
		}
		#endregion

	}
}
