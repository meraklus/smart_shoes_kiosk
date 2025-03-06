using System;
using System.Drawing;
using System.Windows.Forms;
using SmartShoes.Common.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.System;

namespace SmartShoes.Client.UI
{
    public partial class MeasureResultForm2 : UserControl, IPageChangeNotifier
    {


        public event EventHandler<PageChangeEventArgs> PageChangeRequested;
        private MeasurementData measurementData = null;
        private MeasureResult measureResult = null;
        private ShoesData shoesData = null;
        private ShoesResult shoesResult = null;


        public MeasureResultForm2()
        {
            Console.WriteLine("폼 실행");
            InitializeComponent();

            BatchText();

            List<MatData> lstmd = MatDataManager.Instance.GetAllMatData();
            string userName = UserInfo.Instance.UserName;
            string userId = UserInfo.Instance.UserId;
            this.txtUserName.Text = userName + "님의 결과";
            this.txtDate.Text = "측정일 : " + DateTime.Now.ToShortDateString();
            //this.txtGrade.Text = 1 + "등급";


        }

        /// <summary>
        /// 라벨 TEXT 배치
        /// </summary>
        private void BatchText()
        {
            #region ::: Set Label Parent :::
            this.txtMatTL01.Parent = MatTopL;
            this.txtMatTL02.Parent = MatTopL;
            this.txtMatTL03.Parent = MatTopL;
            this.txtMatTL04.Parent = MatTopL;

            this.txtMatTM01.Parent = MatTopM;
            this.txtMatTM02.Parent = MatTopM;
            this.txtMatTM03.Parent = MatTopM;
            this.txtMatTM04.Parent = MatTopM;

            this.txtMatTR01.Parent = MatTopR;
            this.txtMatTR02.Parent = MatTopR;
            this.txtMatTR03.Parent = MatTopR;
            this.txtMatTR04.Parent = MatTopR;

            this.txtShoesTL01.Parent = ShoesL;
            this.txtShoesTL02.Parent = ShoesL;
            this.txtShoesTL03.Parent = ShoesL;
            this.txtShoesTL04.Parent = ShoesL;

            this.txtShoesTM01.Parent = ShoesM;
            this.txtShoesTM02.Parent = ShoesM;
            this.txtShoesTM03.Parent = ShoesM;
            this.txtShoesTM04.Parent = ShoesM;

            this.txtShoesTR01.Parent = ShoesR;
            this.txtShoesTR02.Parent = ShoesR;
            this.txtShoesTR03.Parent = ShoesR;
            this.txtShoesTR04.Parent = ShoesR;

            this.txtStep01.Parent = StepPic;
            this.txtStep02.Parent = StepPic;
            this.txtStep03.Parent = StepPic;
            this.txtStep04.Parent = StepPic;
            this.txtStep05.Parent = StepPic;

            this.txtBalance01.Parent = BalancePic;
            this.txtBalance02.Parent = BalancePic;
            this.txtStepTotalLength.Parent = BalancePic;

            this.txtGrid01.Parent = Gridpic;
            this.txtGrid02.Parent = Gridpic;
            this.txtGrid03.Parent = Gridpic;
            this.txtGrid04.Parent = Gridpic;
            this.txtGrid05.Parent = Gridpic;
            this.txtGrid06.Parent = Gridpic;
            this.txtGrid07.Parent = Gridpic;
            this.txtGrid08.Parent = Gridpic;
            this.txtGrid09.Parent = Gridpic;
            this.txtGrid10.Parent = Gridpic;
            this.txtGrid11.Parent = Gridpic;
            this.txtGrid12.Parent = Gridpic;
            this.txtGrid13.Parent = Gridpic;
            this.txtGrid14.Parent = Gridpic;
            this.txtGrid15.Parent = Gridpic;
            this.txtGrid16.Parent = Gridpic;
            this.txtGrid17.Parent = Gridpic;
            this.txtGrid18.Parent = Gridpic;

            this.txtRemark.Parent = Remarkpic;
            #endregion

            #region ::: Position in PictureBox :::
            this.txtMatTL01.Location = new Point(117, 105);
            this.txtMatTL02.Location = new Point(195, 105);
            this.txtMatTL03.Location = new Point(117, 130);
            this.txtMatTL04.Location = new Point(195, 130);

            this.txtMatTM01.Location = new Point(93, 68);
            this.txtMatTM02.Location = new Point(159, 68);
            this.txtMatTM03.Location = new Point(93, 102);
            this.txtMatTM04.Location = new Point(159, 102);

            this.txtMatTR01.Location = new Point(93, 68);
            this.txtMatTR02.Location = new Point(159, 68);
            this.txtMatTR03.Location = new Point(93, 102);
            this.txtMatTR04.Location = new Point(159, 102);

            this.txtShoesTL01.Location = new Point(117, 105);
            this.txtShoesTL02.Location = new Point(195, 105);
            this.txtShoesTL03.Location = new Point(117, 130);
            this.txtShoesTL04.Location = new Point(195, 130);

            this.txtShoesTM01.Location = new Point(93, 68);
            this.txtShoesTM02.Location = new Point(159, 68);
            this.txtShoesTM03.Location = new Point(93, 102);
            this.txtShoesTM04.Location = new Point(159, 102);

            this.txtShoesTR01.Location = new Point(93, 68);
            this.txtShoesTR02.Location = new Point(159, 68);
            this.txtShoesTR03.Location = new Point(93, 102);
            this.txtShoesTR04.Location = new Point(159, 102);

            this.txtStep01.Location = new Point(80, 20);
            this.txtStep02.Location = new Point(290, 35);
            this.txtStep03.Location = new Point(370, 40);
            this.txtStep04.Location = new Point(70, 170);
            this.txtStep05.Location = new Point(170, 140);

            this.txtBalance01.Location = new Point(40, 165);
            this.txtBalance02.Location = new Point(150, 165);
            this.txtStepTotalLength.Location = new Point(160, 215);

            this.txtGrid01.Location = new Point(201, 47);
            this.txtGrid02.Location = new Point(305, 47);
            this.txtGrid03.Location = new Point(405, 47);
            this.txtGrid04.Location = new Point(201, 81);
            this.txtGrid05.Location = new Point(305, 81);
            this.txtGrid06.Location = new Point(405, 81);
            this.txtGrid07.Location = new Point(201, 115);
            this.txtGrid08.Location = new Point(305, 115);
            this.txtGrid09.Location = new Point(405, 115);
            this.txtGrid10.Location = new Point(201, 149);
            this.txtGrid11.Location = new Point(305, 149);
            this.txtGrid12.Location = new Point(405, 149);
            this.txtGrid13.Location = new Point(201, 179);
            this.txtGrid14.Location = new Point(305, 179);
            this.txtGrid15.Location = new Point(405, 179);
            this.txtGrid16.Location = new Point(201, 208);
            this.txtGrid17.Location = new Point(305, 208);
            this.txtGrid18.Location = new Point(405, 208);

            this.txtRemark.Location = new Point(30, 15);
            #endregion
        }

        private void InsertData(MatData MatData)
        {
            this.txtMatTL01.Text = MatData.StancePhase1.ToString("F0");
            this.txtMatTL02.Text = MatData.SwingPhase1.ToString("F0");
            this.txtMatTL03.Text = MatData.SwingPhase2.ToString("F0");
            this.txtMatTL04.Text = MatData.StancePhase2.ToString("F0");

            this.txtMatTM01.Text = MatData.StancePhase1.ToString("F0");
            this.txtMatTM02.Text = "0";
            this.txtMatTM03.Text = MatData.SwingPhase1.ToString("F0");
            this.txtMatTM04.Text = "0";

            this.txtMatTR01.Text = MatData.StancePhase2.ToString("F0");
            this.txtMatTR02.Text = "0";
            this.txtMatTR03.Text = MatData.SwingPhase2.ToString("F0");
            this.txtMatTR04.Text = "0";

            this.txtShoesTL01.Text = "";
            this.txtShoesTL02.Text = "";
            this.txtShoesTL03.Text = "";
            this.txtShoesTL04.Text = "";

            this.txtShoesTM01.Text = "";
            this.txtShoesTM02.Text = "";
            this.txtShoesTM03.Text = "";
            this.txtShoesTM04.Text = "";

            this.txtShoesTR01.Text = "";
            this.txtShoesTR02.Text = "";
            this.txtShoesTR03.Text = "";
            this.txtShoesTR04.Text = "";

            this.txtStep01.Text = MatData.StrideLength4.ToString("F0") + "cm";
            this.txtStep02.Text = MatData.StepAngle2.ToString("F0") + "˚";
            this.txtStep03.Text = MatData.StepLength2.ToString("F1") + "cm";
            this.txtStep04.Text = MatData.StepLength1.ToString("F1") + "cm";
            this.txtStep05.Text = MatData.StepAngle1.ToString("F0") + "˚";

            this.txtBalance01.Text = MatData.StepForce1.ToString("F0") + "%";
            this.txtBalance02.Text = MatData.StepForce2.ToString("F0") + "%";
            this.txtStepTotalLength.Text = MatData.StrideLength4.ToString("F0") + "cm";

            this.txtGrid01.Text = MatData.StepLength1.ToString("F1");
            this.txtGrid02.Text = MatData.StepLength2.ToString("F1");

            this.txtGrid04.Text = MatData.SingleStepTime1.ToString("F2");
            this.txtGrid05.Text = MatData.SingleStepTime2.ToString("F2");

            this.txtGrid07.Text = MatData.StrideTime1.ToString("F2");
            this.txtGrid08.Text = MatData.StrideTime2.ToString("F2");

            this.txtGrid10.Text = MatData.StepAngle1.ToString("F2");
            this.txtGrid11.Text = MatData.StepAngle2.ToString("F2");

            this.txtGrid13.Text = MatData.StepForce1.ToString("F2");
            this.txtGrid14.Text = MatData.StepForce2.ToString("F2");

            this.txtGrid16.Text = MatData.BaseOfGait4.ToString("F2");
            this.txtGrid17.Text = MatData.BaseOfGait4.ToString("F2");

            this.txtRemark.Text = "";
        }

        private async Task<string> CallApiReportCreate()
        {
            string apistr = "";
            ApiCallHelper apiCallHelper = new ApiCallHelper();

            // POST 요청 URL
            string postUrl = "http://smartshoes.kr/api/report";

            // POST 요청 데이터 (JSON 형식)
            var postData = new
            {
                userSid = UserInfo.Instance.UserId
            };

            try
            {
                // POST 요청
                string postResponse = await apiCallHelper.PostAsync(postUrl, postData);

                // JSON 응답 파싱
                JObject json = JObject.Parse(postResponse);

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


        public static List<double[]> ConvertData(List<int[]> leftData, List<int[]> rightData)
        {

            Console.WriteLine("컨버팅 시작", leftData.Count, "@@", rightData.Count);
            int dataLength = Math.Min(leftData.Count, rightData.Count);
            var combinedData = new List<double[]>(dataLength);

            for (int i = 0; i < dataLength; i++)
            {
                double[] left = new double[3];
                double[] right = new double[3];

                // 가속도 변환
                left[0] = (leftData[i][0] * 2 * 9.8 / 32768);
                left[1] = (leftData[i][1] * 2 * 9.8 / 32768);
                left[2] = (leftData[i][2] * 2 * 9.8 / 32768);
                right[0] = (rightData[i][0] * 2 * 9.8 / 32768);
                right[1] = (rightData[i][1] * 2 * 9.8 / 32768);
                right[2] = (rightData[i][2] * 2 * 9.8 / 32768);

                // 결합된 row 생성: 왼발(x, y, z 가속도), 오른발(x, y, z 가속도)
                double[] combinedRow = new double[]
                {
                left[0], left[1], left[2], // 왼발 가속도
                right[0], right[1], right[2] // 오른발 가속도
                };

                combinedData.Add(combinedRow);
            }
            Console.WriteLine("컨버팅 끝");
            return combinedData;
        }

        private async Task CallApiResultShoes()
        {
            ApiCallHelper apiCallHelper = new ApiCallHelper();


            // API URL
            string getUrl = "http://smartshoes.kr/api/report/shoes-result";

            try
            {
                Console.WriteLine("신발 post 준비");
                // API 호출 (POST)
                string getResponse = await apiCallHelper.PostAsync(getUrl, this.shoesData);
                Console.WriteLine("신발 post 받음");
                // JSON 응답 파싱
                JObject json = JObject.Parse(getResponse);
                shoesResult = new ShoesResult();
                // JSON 데이터를 MeasureResult 객체에 매핑
                foreach (var pair in json)
                {
                    string key = pair.Key;
                    string value = pair.Value.ToString();
                    switch (key)
                    {
                        case "shoesStancePhase1":
                            shoesResult.shoesStancePhase1 = value;
                            break;
                        case "shoesSwingPhase1":
                            shoesResult.shoesSwingPhase1 = value;
                            break;
                        case "shoesSwingPhase2":
                            shoesResult.shoesSwingPhase2 = value;
                            break;
                        case "shoesStancePhase2":
                            shoesResult.shoesStancePhase2 = value;
                            break;
                        case "shoesLeftStancePhaseScore":
                            shoesResult.shoesLeftStancePhaseScore = value;
                            break;
                        case "shoesLeftSwingPhaseScore":
                            shoesResult.shoesLeftSwingPhaseScore = value;
                            break;
                        case "shoesRightStancePhaseScore":
                            shoesResult.shoesRightStancePhaseScore = value;
                            break;
                        case "shoesRightSwingPhaseScore":
                            shoesResult.shoesRightSwingPhaseScore = value;
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("신발 post 에러터짐");
                Console.WriteLine(ex.Message);
            }

        }


        private async Task CallApiResultMatt()
        {
            ApiCallHelper apiCallHelper = new ApiCallHelper();

            // API URL
            string getUrl = "http://smartshoes.kr/api/report/mat-result";

            try
            {
                // API 호출 (POST)
                string getResponse = await apiCallHelper.PostAsync(getUrl, this.measurementData);

                // JSON 응답 파싱
                JObject json = JObject.Parse(getResponse);
                measureResult = new MeasureResult();
                // JSON 데이터를 MeasureResult 객체에 매핑
                foreach (var pair in json)
                {
                    string key = pair.Key;
                    string value = pair.Value.ToString();

                    switch (key)
                    {
                        case "matStepLengthScore":
                            measureResult.matStepLengthScore = value;
                            break;
                        case "matStepTimeScore":
                            measureResult.matStepTimeScore = value;
                            break;
                        case "matStrideTimeScore":
                            measureResult.matStrideTimeScore = value;
                            break;
                        case "matStepAngleScore":
                            measureResult.matStepAngleScore = value;
                            break;
                        case "matStepForceScore":
                            measureResult.matStepForceScore = value;
                            break;
                        case "matBaseOfGaitScore":
                            measureResult.matBaseOfGaitScore = value;
                            break;
                        case "matTotalScore":
                            measureResult.matTotalScore = value;
                            break;
                        case "matLeftStancePhaseScore":
                            measureResult.matLeftStancePhaseScore = value;
                            break;
                        case "matRightStancePhaseScore":
                            measureResult.matRightStancePhaseScore = value;
                            break;
                        case "matLeftSwingPhaseScore":
                            measureResult.matLeftSwingPhaseScore = value;
                            break;
                        case "matRightSwingPhaseScore":
                            measureResult.matRightSwingPhaseScore = value;
                            break;
                        case "matStepMinValue":
                            measureResult.matStepMinValue = value;
                            break;
                        case "matStepMaxValue":
                            measureResult.matStepMaxValue = value;
                            break;
                        case "matBalanceLevel":
                            measureResult.matBalanceLevel = value;
                            break;
                        case "matLeftStepStable":
                            measureResult.matLeftStepStable = value;
                            break;
                        case "matRightStepStable":
                            measureResult.matRightStepStable = value;
                            break;
                        case "matGaitSpacingStable":
                            measureResult.matGaitSpacingStable = value;
                            break;
                        case "shoesLeftStancePhaseValue":
                            measureResult.shoesStancePhase1 = value;
                            break;
                        case "shoesLeftSwingPhaseValue":
                            measureResult.shoesSwingPhase1 = value;
                            break;
                        case "shoesRightSwingPhaseValue":
                            measureResult.shoesSwingPhase2 = value;
                            break;
                        case "shoesRightStancePhaseValue":
                            measureResult.shoesStancePhase2 = value;
                            break;
                        case "shoesLeftStancePhaseVariableScore":
                            measureResult.shoesLeftStancePhaseScore = value;
                            break;
                        case "shoesLeftSwingPhaseVariableScore":
                            measureResult.shoesLeftSwingPhaseScore = value;
                            break;
                        case "shoesRightStancePhaseVariableScore":
                            measureResult.shoesRightStancePhaseScore = value;
                            break;
                        case "shoesRightSwingPhaseVariableScore":
                            measureResult.shoesRightSwingPhaseScore = value;
                            break;



                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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


            //       measurementData = null;
            // measureResult = null;
            // shoesData = null;
            // shoesResult = null;


            // if (cfp.Confirmed)
            // {
            // 	this.Invoke(new Action(() => MovePage(typeof(LoginForm))));
            // }
            var leftData = BLEManager.Instance.GetLeftData();
            var rightData = BLEManager.Instance.GetRightData();

            this.Invoke(new Action(() => MovePage(typeof(LoginForm))));


        }

        private void MeasureResultForm_Load(object sender, EventArgs e)
        {
            CallApi();
        }

        private async void CallApi()
        {
            try
            {
                string reportId = await CallApiReportCreate();
                int userid = UserInfo.Instance.UserId == null ? 0 : Convert.ToInt32(UserInfo.Instance.UserId);
                int height = UserInfo.Instance.Height;
                if (!reportId.Equals(""))
                {
                    long reportSid = Convert.ToInt64(reportId);
                    List<MatData> lstmd = MatDataManager.Instance.GetAllMatData();

                    int matCount = lstmd.Count;

                    if (matCount == 1 || matCount == 2)
                    {
                        InsertData(lstmd[matCount - 1]);


                        measurementData = new MeasurementData
                        {
                            userSid = userid,
                            reportSid = reportSid,
                            height = height,
                            wdate = DateTime.Now.ToString("yyyy-MM-dd"),
                            stepLength1 = lstmd[matCount - 1].StepLength1,
                            stepLength2 = lstmd[matCount - 1].StepLength2,
                            stepLength3 = lstmd[matCount - 1].StepLength3,
                            stepLength4 = lstmd[matCount - 1].StepLength4,
                            strideLength1 = lstmd[matCount - 1].StrideLength1,
                            strideLength2 = lstmd[matCount - 1].StrideLength2,
                            strideLength3 = lstmd[matCount - 1].StrideLength3,
                            strideLength4 = lstmd[matCount - 1].StrideLength4,
                            singleStepTime1 = lstmd[matCount - 1].SingleStepTime1,
                            singleStepTime2 = lstmd[matCount - 1].SingleStepTime2,
                            singleStepTime3 = lstmd[matCount - 1].SingleStepTime3,
                            singleStepTime4 = lstmd[matCount - 1].SingleStepTime4,
                            stepAngle1 = lstmd[matCount - 1].StepAngle1,
                            stepAngle2 = lstmd[matCount - 1].StepAngle2,
                            stepAngle3 = lstmd[matCount - 1].StepAngle3,
                            stepAngle4 = lstmd[matCount - 1].StepAngle4,
                            stepCount1 = lstmd[matCount - 1].StepCount1,
                            stepCount2 = lstmd[matCount - 1].StepCount2,
                            stepCount3 = lstmd[matCount - 1].StepCount3,
                            stepCount4 = lstmd[matCount - 1].StepCount4,
                            baseOfGait1 = lstmd[matCount - 1].BaseOfGait1,
                            baseOfGait2 = lstmd[matCount - 1].BaseOfGait2,
                            baseOfGait3 = lstmd[matCount - 1].BaseOfGait3,
                            baseOfGait4 = lstmd[matCount - 1].BaseOfGait4,
                            stepForce1 = lstmd[matCount - 1].StepForce1,
                            stepForce2 = lstmd[matCount - 1].StepForce2,
                            stepForce3 = lstmd[matCount - 1].StepForce3,
                            stepForce4 = lstmd[matCount - 1].StepForce4,
                            stancePhase1 = lstmd[matCount - 1].StancePhase1,
                            stancePhase2 = lstmd[matCount - 1].StancePhase2,
                            stancePhase3 = lstmd[matCount - 1].StancePhase3,
                            stancePhase4 = lstmd[matCount - 1].StancePhase4,
                            swingPhase1 = lstmd[matCount - 1].SwingPhase1,
                            swingPhase2 = lstmd[matCount - 1].SwingPhase2,
                            swingPhase3 = lstmd[matCount - 1].SwingPhase3,
                            swingPhase4 = lstmd[matCount - 1].SwingPhase4,
                            singleSupport1 = lstmd[matCount - 1].SingleSupport1,
                            singleSupport2 = lstmd[matCount - 1].SingleSupport2,
                            singleSupport3 = lstmd[matCount - 1].SingleSupport3,
                            singleSupport4 = lstmd[matCount - 1].SingleSupport4,
                            totalDoubleSupport1 = lstmd[matCount - 1].TotalDoubleSupport1,
                            totalDoubleSupport2 = lstmd[matCount - 1].TotalDoubleSupport2,
                            totalDoubleSupport3 = lstmd[matCount - 1].TotalDoubleSupport3,
                            totalDoubleSupport4 = lstmd[matCount - 1].TotalDoubleSupport4,
                            loadResponse1 = lstmd[matCount - 1].LoadResponce1,
                            loadResponse2 = lstmd[matCount - 1].LoadResponce2,
                            loadResponse3 = lstmd[matCount - 1].LoadResponce3,
                            loadResponse4 = lstmd[matCount - 1].LoadResponce4,
                            preSwing1 = lstmd[matCount - 1].PreSwing1,
                            preSwing2 = lstmd[matCount - 1].PreSwing2,
                            preSwing3 = lstmd[matCount - 1].PreSwing3,
                            preSwing4 = lstmd[matCount - 1].PreSwing4,
                            stepPosition1 = lstmd[matCount - 1].StepPosition1,
                            stepPosition2 = lstmd[matCount - 1].StepPosition2,
                            stepPosition3 = lstmd[matCount - 1].StepPosition3,
                            stepPosition4 = lstmd[matCount - 1].StepPosition4,
                            strideTime1 = lstmd[matCount - 1].StrideTime1,
                            strideTime2 = lstmd[matCount - 1].StrideTime2,
                            strideTime3 = lstmd[matCount - 1].StrideTime3,
                            strideTime4 = lstmd[matCount - 1].StrideTime4,
                            stanceTime1 = lstmd[matCount - 1].StanceTime1,
                            stanceTime2 = lstmd[matCount - 1].StanceTime2,
                            stanceTime3 = lstmd[matCount - 1].StanceTime3,
                            stanceTime4 = lstmd[matCount - 1].StanceTime4,
                            copLength1 = lstmd[matCount - 1].CopLength1,
                            copLength2 = lstmd[matCount - 1].CopLength2,
                            copLength3 = lstmd[matCount - 1].CopLength3,
                            copLength4 = lstmd[matCount - 1].CopLength4
                        };
                    };
                    await CallApiResultMatt();



                    Guid serviceUUID = new Guid(Properties.Settings.Default.SERVICE_UUID);

                    // await BLEManager.Instance.DisconnectDevicesAsync(serviceUUID);

                    // var shoesL = BLEManager.Instance._parsedDataL;
                    // var shoesR = BLEManager.Instance._parsedDataR;
                    Console.WriteLine("신발 데이터 컨버팅 직전");
                    // var combinedData = ConvertData(shoesL, shoesR);

                    shoesData = new ShoesData
                    {

                        userSid = userid,
                        reportSid = reportSid,
                        // data = combinedData,
                        columns = new List<string>
                        {
                            "LAccelX",
                            "LAccelY",
                            "LAccelZ",
                            "RAccelX",
                            "RAccelY",
                            "RAccelZ"
                        }
                    };

                    await CallApiResultShoes();
                    //Console.WriteLine("결과 직전");
                    if (measureResult != null)
                    {
                        InsertResultData();
                    }
                    if (shoesResult != null)
                    {
                        InsertShoesData();
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void InsertResultData()
        {
            this.txtMatTM02.Text = measureResult.matLeftStancePhaseScore;
            this.txtMatTM04.Text = measureResult.matLeftSwingPhaseScore;
            this.txtMatTR02.Text = measureResult.matRightStancePhaseScore;
            this.txtMatTR04.Text = measureResult.matRightSwingPhaseScore;

            this.txtShoesTM01.Text = measureResult.shoesStancePhase1;
            this.txtShoesTM02.Text = measureResult.shoesLeftStancePhaseScore;
            this.txtShoesTM03.Text = measureResult.shoesSwingPhase1;
            this.txtShoesTM04.Text = measureResult.shoesLeftSwingPhaseScore;
            this.txtShoesTR01.Text = measureResult.shoesStancePhase2;
            this.txtShoesTR02.Text = measureResult.shoesRightStancePhaseScore;
            this.txtShoesTR03.Text = measureResult.shoesSwingPhase2;
            this.txtShoesTR04.Text = measureResult.shoesRightSwingPhaseScore;
            this.txtShoesTL01.Text = measureResult.shoesStancePhase1;
            this.txtShoesTL02.Text = measureResult.shoesSwingPhase1;
            this.txtShoesTL03.Text = measureResult.shoesSwingPhase2;
            this.txtShoesTL04.Text = measureResult.shoesStancePhase2;
            switch (measureResult.matBalanceLevel)
            {
                case "1":
                    this.GradePic.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources._1등급;
                    break;
                case "2":
                    this.GradePic.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources._2등급;
                    break;
                case "3":
                    this.GradePic.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources._3등급;
                    break;
                case "4":
                    this.GradePic.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources._4등급;
                    break;
                case "5":
                    this.GradePic.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources._5등급;
                    break;
            }
            string lg = measureResult.matLeftStepStable.Equals("False") ? "불안정" : "안정";
            string rg = measureResult.matRightStepStable.Equals("False") ? "불안정" : "안정";
            string bg = measureResult.matGaitSpacingStable.Equals("False") ? "불안정" : "안정";
            this.txtRemark.Text = "1. 한걸음 보행 상태 - " +
                "보행 최소값 : " + measureResult.matStepMinValue + "  " +
                "보행 최대값 : " + measureResult.matStepMaxValue + "  " +
                "왼발 보행 : " + lg + "  " +
                "오른발 보행 : " + rg + "\n\n" +
                "2. 보행평가 - " +
                "전체 보행 등급 : " + measureResult.matBalanceLevel + "등급  " +
                "보행 간격 : " + bg;
        }
        private void InsertShoesData()
        {

            this.txtShoesTM01.Text = shoesResult.shoesStancePhase1;
            this.txtShoesTM02.Text = shoesResult.shoesLeftStancePhaseScore;
            this.txtShoesTM03.Text = shoesResult.shoesSwingPhase1;
            this.txtShoesTM04.Text = shoesResult.shoesLeftSwingPhaseScore;
            this.txtShoesTR01.Text = shoesResult.shoesStancePhase2;
            this.txtShoesTR02.Text = shoesResult.shoesRightStancePhaseScore;
            this.txtShoesTR03.Text = shoesResult.shoesSwingPhase2;
            this.txtShoesTR04.Text = shoesResult.shoesRightSwingPhaseScore;

            this.txtShoesTL01.Text = shoesResult.shoesStancePhase1;
            this.txtShoesTL02.Text = shoesResult.shoesStancePhase2;
            this.txtShoesTL03.Text = shoesResult.shoesSwingPhase1;
            this.txtShoesTL04.Text = shoesResult.shoesSwingPhase2;




            return;
        }

        #region :: 측정데이터 클래스 ::
        private class MeasurementData
        {
            public long userSid { get; set; }
            public long reportSid { get; set; }
            public long height { get; set; }
            public string wdate { get; set; }
            public double stepLength1 { get; set; }
            public double stepLength2 { get; set; }
            public double stepLength3 { get; set; }
            public double stepLength4 { get; set; }
            public double strideLength1 { get; set; }
            public double strideLength2 { get; set; }
            public double strideLength3 { get; set; }
            public double strideLength4 { get; set; }
            public double singleStepTime1 { get; set; }
            public double singleStepTime2 { get; set; }
            public double singleStepTime3 { get; set; }
            public double singleStepTime4 { get; set; }
            public double stepAngle1 { get; set; }
            public double stepAngle2 { get; set; }
            public double stepAngle3 { get; set; }
            public double stepAngle4 { get; set; }
            public double stepCount1 { get; set; }
            public double stepCount2 { get; set; }
            public double stepCount3 { get; set; }
            public double stepCount4 { get; set; }
            public double baseOfGait1 { get; set; }
            public double baseOfGait2 { get; set; }
            public double baseOfGait3 { get; set; }
            public double baseOfGait4 { get; set; }
            public double stepForce1 { get; set; }
            public double stepForce2 { get; set; }
            public double stepForce3 { get; set; }
            public double stepForce4 { get; set; }
            public double stancePhase1 { get; set; }
            public double stancePhase2 { get; set; }
            public double stancePhase3 { get; set; }
            public double stancePhase4 { get; set; }
            public double swingPhase1 { get; set; }
            public double swingPhase2 { get; set; }
            public double swingPhase3 { get; set; }
            public double swingPhase4 { get; set; }
            public double singleSupport1 { get; set; }
            public double singleSupport2 { get; set; }
            public double singleSupport3 { get; set; }
            public double singleSupport4 { get; set; }
            public double totalDoubleSupport1 { get; set; }
            public double totalDoubleSupport2 { get; set; }
            public double totalDoubleSupport3 { get; set; }
            public double totalDoubleSupport4 { get; set; }
            public double loadResponse1 { get; set; }
            public double loadResponse2 { get; set; }
            public double loadResponse3 { get; set; }
            public double loadResponse4 { get; set; }
            public double preSwing1 { get; set; }
            public double preSwing2 { get; set; }
            public double preSwing3 { get; set; }
            public double preSwing4 { get; set; }
            public double stepPosition1 { get; set; }
            public double stepPosition2 { get; set; }
            public double stepPosition3 { get; set; }
            public double stepPosition4 { get; set; }
            public double strideTime1 { get; set; }
            public double strideTime2 { get; set; }
            public double strideTime3 { get; set; }
            public double strideTime4 { get; set; }
            public double stanceTime1 { get; set; }
            public double stanceTime2 { get; set; }
            public double stanceTime3 { get; set; }
            public double stanceTime4 { get; set; }
            public double copLength1 { get; set; }
            public double copLength2 { get; set; }
            public double copLength3 { get; set; }
            public double copLength4 { get; set; }
        }

        private class MeasureResult
        {
            public string matStepLengthScore { get; set; }
            public string matStepTimeScore { get; set; }
            public string matStrideTimeScore { get; set; }
            public string matStepAngleScore { get; set; }
            public string matStepForceScore { get; set; }
            public string matBaseOfGaitScore { get; set; }
            public string matTotalScore { get; set; }
            public string matLeftStancePhaseScore { get; set; }
            public string matRightStancePhaseScore { get; set; }
            public string matLeftSwingPhaseScore { get; set; }
            public string matRightSwingPhaseScore { get; set; }
            public string matStepMinValue { get; set; }
            public string matStepMaxValue { get; set; }
            public string matBalanceLevel { get; set; }
            public string matLeftStepStable { get; set; }
            public string matRightStepStable { get; set; }
            public string matGaitSpacingStable { get; set; }

            public string shoesStancePhase1 { get; set; }
            public string shoesSwingPhase1 { get; set; }
            public string shoesSwingPhase2 { get; set; }
            public string shoesStancePhase2 { get; set; }
            public string shoesLeftStancePhaseScore { get; set; }
            public string shoesLeftSwingPhaseScore { get; set; }
            public string shoesRightStancePhaseScore { get; set; }
            public string shoesRightSwingPhaseScore { get; set; }

        }

        private class ShoesData
        {
            public long userSid { get; set; }
            public long reportSid { get; set; }
            public List<string> columns { get; set; }

            public List<double[]> data { get; set; }
        }


        private class ShoesResult
        {
            public string shoesStancePhase1 { get; set; }
            public string shoesSwingPhase1 { get; set; }
            public string shoesSwingPhase2 { get; set; }
            public string shoesStancePhase2 { get; set; }
            public string shoesLeftStancePhaseScore { get; set; }
            public string shoesLeftSwingPhaseScore { get; set; }
            public string shoesRightStancePhaseScore { get; set; }
            public string shoesRightSwingPhaseScore { get; set; }

        }


        #endregion


    }
}
