using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using SmartShoes.Common.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing.Printing;

namespace SmartShoes.Client.UI
{
    public partial class NewResultForm : UserControl, IPageChangeNotifier
    {
        public event EventHandler<PageChangeEventArgs> PageChangeRequested;

        private string prefixUrl = "https://www.smartshoes.kr/api/";
        private MeasurementData measurementData = null;
        private MeasureResult measureResult = null;
        private ShoesData shoesData = null;
        private ShoesResult shoesResult = null;
        private LoadingPopup loadingPopup = null;
        private bool isMatDataProcessed = false;
        private bool isShoesDataProcessed = false;
        private bool isCameraDataProcessed = false;
        private long reportSid = 0;
        private System.Drawing.Printing.PrintDocument printDocument;
        private int currentPrintPage = 0;

        public NewResultForm()
        {
            InitializeComponent();

            // 초기에는 panel2와 print 버튼을 숨깁니다.
            this.panel2.Visible = false;
            this.picPrint.Visible = false;
        }

        private void NewResultForm_Load(object sender, EventArgs e)
        {
            // 로딩 화면 표시
            ShowLoadingScreen();

            // 비동기로 데이터 수집 및 분석 요청
            ProcessAllData();
        }

        /// <summary>
        /// 로딩 화면을 표시합니다.
        /// </summary>
        private void ShowLoadingScreen()
        {
            // LoadingPopup 클래스를 사용하여 로딩 화면 표시
            loadingPopup = new LoadingPopup();
            loadingPopup.Show();
            Application.DoEvents(); // UI 업데이트를 위해 필요
        }

        /// <summary>
        /// 로딩 화면을 닫습니다.
        /// </summary>
        private void HideLoadingScreen()
        {
            if (loadingPopup != null && !loadingPopup.IsDisposed)
            {
                loadingPopup.Close();
                loadingPopup.Dispose();
                loadingPopup = null;
            }
        }

        /// <summary>
        /// 모든 데이터 처리를 시작합니다.
        /// </summary>
        private async void ProcessAllData()
        {
            try
            {
                // 리포트 ID 생성
                reportSid = await CreateReportAsync();

                this.txtFootSize.Text = UserInfo.Instance.FootSize.ToString();
                this.txtHeight.Text = UserInfo.Instance.Height.ToString();
                this.txtSex.Text = UserInfo.Instance.Sex.ToString();
                this.txtAge.Text = UserInfo.Instance.BirthYear.ToString();
                this.txtName.Text = UserInfo.Instance.UserName.ToString();
                this.txtMeasureDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                this.txtFootSize2.Text = UserInfo.Instance.FootSize.ToString();
                this.txtHeight2.Text = UserInfo.Instance.Height.ToString();
                this.txtSex2.Text = UserInfo.Instance.Sex.ToString();
                this.txtAge2.Text = UserInfo.Instance.BirthYear.ToString();
                this.txtName2.Text = UserInfo.Instance.UserName.ToString();
                this.txtMeasureDate2.Text = DateTime.Now.ToString("yyyy-MM-dd");

                if (reportSid > 0)
                {
                    // 각 데이터 처리를 병렬로 시작
                    var matTask = ProcessMatDataAsync();
                    var shoesTask = ProcessShoesDataAsync();
                    var cameraTask = ProcessCameraDataAsync();

                    // 모든 작업이 완료될 때까지 대기
                    await Task.WhenAll(matTask, shoesTask, cameraTask);

                    // 모든 데이터 처리가 완료되면 로딩 화면 닫기
                    HideLoadingScreen();

                    // 결과 데이터 화면에 표시
                    DisplayResults();
                }
                else
                {
                    HideLoadingScreen();
                    MessageBox.Show("리포트 생성에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                HideLoadingScreen();
                MessageBox.Show($"데이터 처리 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"데이터 처리 오류: {ex}");
            }
        }

        /// <summary>
        /// 리포트 ID를 생성합니다.
        /// </summary>
        private async Task<long> CreateReportAsync()
        {
            try
            {
                ApiCallHelper apiCallHelper = new ApiCallHelper();

                // POST 요청 URL
                string postUrl = prefixUrl + "report";

                // POST 요청 데이터 (JSON 형식)
                var postData = new
                {
                    userSid = UserInfo.Instance.UserId
                };

                // POST 요청
                string postResponse = await apiCallHelper.PostAsync(postUrl, postData);

                // JSON 응답 파싱
                JObject json = JObject.Parse(postResponse);

                // 리포트 ID 추출
                foreach (var pair in json)
                {
                    string key = pair.Key;
                    string value = pair.Value.ToString();

                    Console.WriteLine($"Key: {key}, Value: {value}");
                    return Convert.ToInt64(value);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"리포트 생성 오류: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Mat 데이터를 처리합니다.
        /// </summary>
        private async Task ProcessMatDataAsync()
        {
            try
            {
                // Mat 데이터 수집
                MatData matData = MatDataManager.Instance.GetMatData();

                if (matData != null)
                {
                    // MeasurementData 객체 생성
                    measurementData = CreateMeasurementData(matData);

                    // API 요청 및 결과 수신
                    await CallApiResultMatt(matData);

                    isMatDataProcessed = true;
                    Console.WriteLine("Mat 데이터 처리 완료");
                }
                else
                {
                    Console.WriteLine("Mat 데이터가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mat 데이터 처리 오류: {ex.Message}");
                //throw;
            }
        }

        /// <summary>
        /// Mat 데이터로부터 MeasurementData 객체를 생성합니다.
        /// </summary>
        private MeasurementData CreateMeasurementData(MatData matData)
        {
            int userid = UserInfo.Instance.UserId == null ? 0 : Convert.ToInt32(UserInfo.Instance.UserId);
            int containerSid = Convert.ToInt32(Properties.Settings.Default.CONTAINER_ID);

            return new MeasurementData
            {
                userSid = userid,
                reportSid = reportSid,
                containerSid = containerSid,
                stepLength1 = matData.StepLength1,
                stepLength2 = matData.StepLength2,
                stepLength3 = matData.StepLength3,
                stepLength4 = matData.StepLength4,
                strideLength1 = matData.StrideLength1,
                strideLength2 = matData.StrideLength2,
                strideLength3 = matData.StrideLength3,
                strideLength4 = matData.StrideLength4,
                singleStepTime1 = matData.SingleStepTime1,
                singleStepTime2 = matData.SingleStepTime2,
                singleStepTime3 = matData.SingleStepTime3,
                singleStepTime4 = matData.SingleStepTime4,
                stepAngle1 = matData.StepAngle1,
                stepAngle2 = matData.StepAngle2,
                stepAngle3 = matData.StepAngle3,
                stepAngle4 = matData.StepAngle4,
                stepCount1 = matData.StepCount1,
                stepCount2 = matData.StepCount2,
                stepCount3 = matData.StepCount3,
                stepCount4 = matData.StepCount4,
                baseOfGait1 = matData.BaseOfGait1,
                baseOfGait2 = matData.BaseOfGait2,
                baseOfGait3 = matData.BaseOfGait3,
                baseOfGait4 = matData.BaseOfGait4,
                stepForce1 = matData.StepForce1,
                stepForce2 = matData.StepForce2,
                stepForce3 = matData.StepForce3,
                stepForce4 = matData.StepForce4,
                stancePhase1 = matData.StancePhase1,
                stancePhase2 = matData.StancePhase2,
                stancePhase3 = matData.StancePhase3,
                stancePhase4 = matData.StancePhase4,
                swingPhase1 = matData.SwingPhase1,
                swingPhase2 = matData.SwingPhase2,
                swingPhase3 = matData.SwingPhase3,
                swingPhase4 = matData.SwingPhase4,
                singleSupport1 = matData.SingleSupport1,
                singleSupport2 = matData.SingleSupport2,
                singleSupport3 = matData.SingleSupport3,
                singleSupport4 = matData.SingleSupport4,
                totalDoubleSupport1 = matData.TotalDoubleSupport1,
                totalDoubleSupport2 = matData.TotalDoubleSupport2,
                totalDoubleSupport3 = matData.TotalDoubleSupport3,
                totalDoubleSupport4 = matData.TotalDoubleSupport4,
                loadResponce1 = matData.LoadResponce1,
                loadResponce2 = matData.LoadResponce2,
                loadResponce3 = matData.LoadResponce3,
                loadResponce4 = matData.LoadResponce4,
                preSwing1 = matData.PreSwing1,
                preSwing2 = matData.PreSwing2,
                preSwing3 = matData.PreSwing3,
                preSwing4 = matData.PreSwing4,
                stepPosition1 = matData.StepPosition1,
                stepPosition2 = matData.StepPosition2,
                stepPosition3 = matData.StepPosition3,
                stepPosition4 = matData.StepPosition4,
                strideTime1 = matData.StrideTime1,
                strideTime2 = matData.StrideTime2,
                strideTime3 = matData.StrideTime3,
                strideTime4 = matData.StrideTime4,
                stanceTime1 = matData.StanceTime1,
                stanceTime2 = matData.StanceTime2,
                stanceTime3 = matData.StanceTime3,
                stanceTime4 = matData.StanceTime4,
                copLength1 = matData.CopLength1,
                copLength2 = matData.CopLength2,
                copLength3 = matData.CopLength3,
                copLength4 = matData.CopLength4
            };
        }

        /// <summary>
        /// Mat 데이터 분석을 위한 API 호출
        /// </summary>
        private async Task CallApiResultMatt(MatData matData)
        {
            try
            {
                // 새로운 API 엔드포인트 URL
                string getUrl = prefixUrl + "report/mat-result";

                // API 호출 헬퍼 생성
                ApiCallHelper apiCallHelper = new ApiCallHelper();

                // 요청 데이터 구조 변경 (data 객체로 감싸기)
                var requestData = measurementData;

                // POST 요청 보내기
                string getResponse = await apiCallHelper.PostAsync(getUrl, requestData);
                Console.WriteLine("Mat API 응답: " + getResponse);

                if (!string.IsNullOrEmpty(getResponse))
                {
                    // 응답 파싱
                    JObject jObject = JObject.Parse(getResponse);

                    // 새로운 API 응답 형식에 맞게 파싱
                    if (jObject != null)
                    {
                        // MeasureResult 객체가 없으면 생성
                        if (measureResult == null)
                        {
                            measureResult = new MeasureResult();
                        }

                        // API 응답에서 점수 값 추출
                        measureResult.matScoreLength = jObject["score_lenght"]?.ToString() ?? "0";
                        measureResult.matScoreSingleTime = jObject["score_singletime"]?.ToString() ?? "0";
                        measureResult.matScoreStrideTime = jObject["score_stridetime"]?.ToString() ?? "0";
                        measureResult.matScoreAngle = jObject["score_angle"]?.ToString() ?? "0";
                        measureResult.matScoreForce = jObject["score_force"]?.ToString() ?? "0";
                        // 소수점 2자리까지 표시하도록 수정
                        var baseOfGaitValue = jObject["score_baseofgait"];
                        if (baseOfGaitValue != null && double.TryParse(baseOfGaitValue.ToString(), out double value))
                        {
                            measureResult.matScoreBaseOfGait = value.ToString("F2");
                        }
                        else
                        {
                            measureResult.matScoreBaseOfGait = "0.00";
                        }
                        measureResult.matTotalScore = jObject["score_grede"]?.ToString() ?? "0";
                        measureResult.matComment = jObject["comment"]?.ToString() ?? "";

                        // 등급에 따라 pictureBoxGrade 이미지 설정
                        if (float.TryParse(measureResult.matTotalScore, out float gradeValue))
                        {
                            int grade = 1;
                            if (gradeValue < 2.0f) grade = 1;
                            else if (gradeValue < 3.0f) grade = 2;
                            else if (gradeValue < 4.0f) grade = 3;
                            else if (gradeValue < 5.0f) grade = 4;
                            else grade = 5;

                            // 등급에 맞는 이미지 설정
                            switch (grade)
                            {
                                case 1:
                                    this.pictureBoxGrade.Image = global::SmartShoes.Client.UI.Properties.Resources.grade1;
                                    this.picTxtGrade.Text = "1등급";
                                    this.labelGrade.Text = "1등급";
                                    this.labelGradeTitle.Text = "[바른 보행]";
                                    this.labelGradeTxt.Text = "일반적인 보행 상태이며, 일상적인 활동에 어려움이 없습니다.";
                                    break;
                                case 2:
                                    this.pictureBoxGrade.Image = global::SmartShoes.Client.UI.Properties.Resources.grade2;
                                    this.picTxtGrade.Text = "2등급";
                                    this.labelGrade.Text = "2등급";
                                    this.labelGradeTitle.Text = "[경미한 불안정 보행]";
                                    this.labelGradeTxt.Text = "일반적인 보행 상태이며, 일상적인 활동에 어려움이 없습니다.";
                                    break;
                                case 3:
                                    this.pictureBoxGrade.Image = global::SmartShoes.Client.UI.Properties.Resources.grade3;
                                    this.picTxtGrade.Text = "3등급";
                                    this.labelGrade.Text = "3등급";
                                    this.labelGradeTitle.Text = "[불안정 보행]";
                                    this.labelGradeTxt.Text = "보행 중 균형 유지가 필요할 수 있으며, 일정한 환경에서 변화가 나타날 수 있습니다.";
                                    break;
                                case 4:
                                    this.pictureBoxGrade.Image = global::SmartShoes.Client.UI.Properties.Resources.grade4;
                                    this.picTxtGrade.Text = "4등급";
                                    this.labelGrade.Text = "4등급";
                                    this.labelGradeTitle.Text = "[균형 저하 보행]";
                                    this.labelGradeTxt.Text = "보행 시 균형 유지가 어려운 경우가 있으며, 보행 보조기구 사용이 고려될 수 있습니다.";
                                    break;
                                case 5:
                                    this.pictureBoxGrade.Image = global::SmartShoes.Client.UI.Properties.Resources.grade5;
                                    this.picTxtGrade.Text = "5등급";
                                    this.labelGrade.Text = "5등급";
                                    this.labelGradeTitle.Text = "[심각한 불균형 보행]";
                                    this.labelGradeTxt.Text = "보행 중 균형 유지가 어려울 가능성이 있으며, 보조기구 사용이 필요할 수 있습니다.";
                                    break;
                            }
                        }

                        this.txtLeftStancePhase.Text = matData.StancePhase1.ToString("F0"); // 왼발 입각
                        this.txtLeftSwingPhase.Text = matData.SwingPhase1.ToString("F0"); //  왼발 유각
                        this.txtRightStancePhase.Text = matData.SwingPhase2.ToString("F0"); // 오른발 유각
                        this.txtRightSwingPhase.Text = matData.StancePhase2.ToString("F0"); // 오른발 입각

                        this.txtLeftLength.Text = matData.StepLength1.ToString("F1");
                        this.txtLeftTime.Text = matData.SingleStepTime1.ToString("F2");
                        this.txtLeftSpeed.Text = matData.StrideTime1.ToString("F2");
                        this.txtLeftAngle.Text = matData.StepAngle1.ToString("F2");
                        this.txtLeftForce.Text = matData.StepForce1.ToString("F2");
                        this.txtLeftGap.Text = matData.BaseOfGait1.ToString("F2");

                        this.txtRightLength.Text = matData.StepLength2.ToString("F1");
                        this.txtRightTime.Text = matData.SingleStepTime2.ToString("F2");
                        this.txtRightSpeed.Text = matData.StrideTime2.ToString("F2");
                        this.txtRightAngle.Text = matData.StepAngle2.ToString("F2");
                        this.txtRightForce.Text = matData.StepForce2.ToString("F2");
                        this.txtRightGap.Text = matData.BaseOfGait2.ToString("F2");

                        // 점수 항목 설정 (기존 측정값 대신 점수 값으로 변경)
                        this.txtStandardLength.Text = measureResult.matScoreLength;
                        this.txtStandardTime.Text = measureResult.matScoreSingleTime;
                        this.txtStandardSpeed.Text = measureResult.matScoreStrideTime;
                        this.txtStandardAngle.Text = measureResult.matScoreAngle;
                        this.txtStandardForce.Text = measureResult.matScoreForce;
                        this.txtStandardGap.Text = measureResult.matScoreBaseOfGait;

                        this.picLeftAngle.Text = matData.StepAngle1.ToString("F2") + "˚";
                        this.picLeftLength.Text = matData.StepLength1.ToString("F1") + "cm";
                        this.picRightAngle.Text = matData.StepAngle2.ToString("F2") + "˚";
                        this.picRightLength.Text = matData.StepLength2.ToString("F1") + "cm";
                        this.picStandardLength.Text = matData.StrideLength4.ToString("F0") + "cm";

                        this.picLeftForce.Text = matData.StepForce1.ToString("F0") + "%";
                        this.picRightForce.Text = matData.StepForce2.ToString("F0") + "%";

                        this.textBoxReport.Text = measureResult.matComment;
                    }
                    else
                    {
                        // 오류 응답 처리
                        // string errorMessage = "API 응답 형식이 올바르지 않습니다.";
                        // MessageBox.Show(errorMessage, "API 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw new Exception("API 응답 형식이 올바르지 않습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                // setBlankMatResult();
                Console.WriteLine("Mat API 호출 중 오류 발생: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Shoes 데이터를 처리합니다.
        /// </summary>
        private async Task ProcessShoesDataAsync()
        {
            // 신발 데이터 처리 로직 구현
            // 실제 구현은 Mat 데이터와 유사한 패턴으로 작성
            // await Task.Delay(1000); // 임시 지연
            // isShoesDataProcessed = true;
            // Console.WriteLine("Shoes 데이터 처리 완료");

            var leftData = BLEManager.Instance.GetLeftData();
            var rightData = BLEManager.Instance.GetRightData();
            // 왼쪽 신발 데이터를 문자열로 변환 (공백 없이 조인)
            string lDataStr = string.Join("", leftData.SelectMany(arr => arr.Select(val => val.ToString())));
            // 오른쪽 신발 데이터를 문자열로 변환 (공백 없이 조인)
            string rDataStr = string.Join("", rightData.SelectMany(arr => arr.Select(val => val.ToString())));

            if (leftData != null && rightData != null)
            {
                shoesData = CreateShoesData(lDataStr, rDataStr);
                await CallApiResultShoes();
            }

        }

        private ShoesData CreateShoesData(string leftData, string rightData)
        {
            int userid = UserInfo.Instance.UserId == null ? 0 : Convert.ToInt32(UserInfo.Instance.UserId);
            int containerSid = Convert.ToInt32(Properties.Settings.Default.CONTAINER_ID);
            return new ShoesData
            {
                userSid = userid,
                reportSid = reportSid,
                containerSid = containerSid,
                LData = leftData,
                RData = rightData
            };
        }

        private async Task CallApiResultShoes()
        {
            try
            {
                // 새로운 API 엔드포인트 URL
                string getUrl = prefixUrl + "report/shoes-result";

                // API 호출 헬퍼 생성
                ApiCallHelper apiCallHelper = new ApiCallHelper();

                // 요청 데이터 구조 변경 (data 객체로 감싸기)
                var requestData = shoesData;

                // POST 요청 보내기
                string getResponse = await apiCallHelper.PostAsync(getUrl, requestData);
                Console.WriteLine("Shoes API 응답: " + getResponse);

                if (!string.IsNullOrEmpty(getResponse))
                {
                    // 응답 파싱
                    JObject jObject = JObject.Parse(getResponse);

                    this.txt2LeftStancePhase.Text = jObject["l_standing_time_avg"]?.ToString() ?? "0"; // 왼발 입각
                    this.txt2LeftSwingPhase.Text = jObject["l_swing_time_avg"]?.ToString() ?? "0"; //  왼발 유각
                    this.txt2RightSwingPhase.Text = jObject["r_swing_time_avg"]?.ToString() ?? "0"; // 오른발 유각
                    this.txt2RightStancePhase.Text = jObject["r_standing_time_avg"]?.ToString() ?? "0"; // 오른발 입각

                    this.txt2LeftLength.Text = jObject["l_step_lenght"]?.ToString() ?? "0";
                    this.txt2RightLength.Text = jObject["r_step_lenght"]?.ToString() ?? "0";
                    this.txt2LeftTime.Text = jObject["l_stance_time"]?.ToString() ?? "0";
                    this.txt2RightTime.Text = jObject["r_stance_time"]?.ToString() ?? "0";

                }
                else
                {
                    // 오류 응답 처리
                    // string errorMessage = "API 응답 형식이 올바르지 않습니다.";
                    // MessageBox.Show(errorMessage, "API 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // 여기서 throw 하면 아래 catch 블록에서 오류 처리됨
                    throw new Exception("");
                }
            }
            catch (Exception ex)
            {
                // setBlankShoesResult();
                Console.WriteLine("Shoes API 호출 중 오류 발생: " + ex.Message);
                //throw;
            }
        }


        /// <summary>
        /// Camera 데이터를 처리합니다.
        /// </summary>
        private async Task ProcessCameraDataAsync()
        {
            try
            {
                // 카메라 데이터 파일 찾기
                string cameraDataFilePath = FindCameraDataFile();

                if (!string.IsNullOrEmpty(cameraDataFilePath))
                {
                    // 파일에서 카메라 데이터 불러오기
                    string jsonData = File.ReadAllText(cameraDataFilePath);

                    // JSON 데이터 파싱
                    var cameraData = JsonConvert.DeserializeObject<List<object>>(jsonData);

                    if (cameraData != null && cameraData.Count > 0)
                    {
                        Console.WriteLine($"카메라 데이터 불러오기 성공: {cameraData.Count}개 데이터");

                        // 여기에 카메라 데이터 처리 로직 추가
                        // 예: API 호출, 데이터 분석 등
                        bool apiSuccess = await SendCameraDataToApi(cameraData);

                        if (apiSuccess)
                        {
                            // API 전송 성공 시 파일 삭제
                            try
                            {
                                File.Delete(cameraDataFilePath);
                                Console.WriteLine($"카메라 데이터 파일 삭제 성공: {cameraDataFilePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"카메라 데이터 파일 삭제 중 오류 발생: {ex.Message}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("카메라 데이터가 비어 있습니다.");
                    }
                }
                else
                {
                    Console.WriteLine("카메라 데이터 파일을 찾을 수 없습니다.");
                }

                isCameraDataProcessed = true;
                Console.WriteLine("Camera 데이터 처리 완료");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"카메라 데이터 처리 중 오류 발생: {ex.Message}");
                isCameraDataProcessed = true; // 오류 발생 시 처리 완료로 간주
            }
        }

        /// <summary>
        /// 카메라 데이터를 API로 전송합니다.
        /// </summary>
        private async Task<bool> SendCameraDataToApi(List<object> cameraData)
        {
            try
            {
                // API 엔드포인트 URL
                string apiUrl = prefixUrl + "report/camera-result";

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // 타임아웃 설정

                    // MultipartFormDataContent 생성
                    using (var multipartContent = new MultipartFormDataContent())
                    {
                        // 카메라 데이터를 JSON 문자열로 변환
                        string jsonData = JsonConvert.SerializeObject(cameraData);

                        // JSON 데이터를 바이트 배열로 변환
                        var fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        var fileContent = new ByteArrayContent(fileBytes);

                        // 파일 이름 설정
                        string fileName = "camera_data.json";
                        multipartContent.Add(fileContent, "cameraFile", fileName);

                        // 파라미터 추가
                        multipartContent.Add(new StringContent(UserInfo.Instance.UserId.ToString()), "userSid");
                        multipartContent.Add(new StringContent(Properties.Settings.Default.CONTAINER_ID), "containerSid");
                        multipartContent.Add(new StringContent(reportSid.ToString()), "reportSid");

                        // 요청 전송 전 로그 출력
                        Console.WriteLine("전송할 파라미터:");
                        Console.WriteLine($"userSid: {UserInfo.Instance.UserId}");
                        Console.WriteLine($"containerSid: {Properties.Settings.Default.CONTAINER_ID}");
                        Console.WriteLine($"reportSid: {reportSid}");
                        Console.WriteLine($"cameraFile: {fileName} (크기: {fileBytes.Length} 바이트)");

                        // 요청 전송
                        var response = await client.PostAsync(apiUrl, multipartContent);

                        // 응답 확인
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            JObject jObject = JObject.Parse(responseContent);
                            // 바로 필요한 값에 접근
                            JObject angleData = (JObject)jObject?["angle"];
                            if (angleData != null)
                            {
                                // 각도 정보 추출 및 표시
                                // center 데이터
                                JObject centerData = (JObject)angleData["center"];
                                if (centerData != null)
                                {
                                    // F1 - 어깨 수평각도
                                    double f1Min = (double?)centerData["F1"]?["min"] ?? 0;
                                    double f1Max = (double?)centerData["F1"]?["max"] ?? 0;
                                    this.centerF1.Text = $"당신의 어깨 수평각도는 {f1Min:F1}/{f1Max:F1}도 입니다";

                                    // F2 - 골반 수평각도
                                    double f2Min = (double?)centerData["F2"]?["min"] ?? 0;
                                    double f2Max = (double?)centerData["F2"]?["max"] ?? 0;
                                    this.centerF2.Text = $"당신의 골반 수평각도는 {f2Min:F1}/{f2Max:F1}도 입니다";

                                    // LU2 - 왼쪽 팔꿈치 각도
                                    double lu2Min = (double?)centerData["LU2"]?["min"] ?? 0;
                                    double lu2Max = (double?)centerData["LU2"]?["max"] ?? 0;
                                    this.centerLU2.Text = $"당신의 팔꿈치각도는 {lu2Min:F1}/{lu2Max:F1}도 입니다";

                                    // RU2 - 오른쪽 팔꿈치 각도
                                    double ru2Min = (double?)centerData["RU2"]?["min"] ?? 0;
                                    double ru2Max = (double?)centerData["RU2"]?["max"] ?? 0;
                                    this.centerRU2.Text = $"당신의 팔꿈치각도는 {ru2Min:F1}/{ru2Max:F1}도 입니다";

                                    // LL1 - 왼쪽 골반각도
                                    double ll1Min = (double?)centerData["LL1"]?["min"] ?? 0;
                                    double ll1Max = (double?)centerData["LL1"]?["max"] ?? 0;
                                    this.centerLL1.Text = $"당신의 골반각도는 {ll1Min:F1}/{ll1Max:F1}도 입니다";

                                    // RL1 - 오른쪽 골반각도 (이 속성이 존재하는지 확인 필요)
                                    double rl1Min = (double?)centerData["RL1"]?["min"] ?? 0;
                                    double rl1Max = (double?)centerData["RL1"]?["max"] ?? 0;
                                    this.centerRL1.Text = $"당신의 골반각도는 {rl1Min:F1}/{rl1Max:F1}도 입니다";
                                }

                                // left 데이터
                                JObject leftData = (JObject)angleData["left"];
                                if (leftData != null)
                                {
                                    // U1 - 어깨각도
                                    double u1Min = (double?)leftData["U1"]?["min"] ?? .0;
                                    double u1Max = (double?)leftData["U1"]?["max"] ?? 0;
                                    this.leftU1.Text = $"당신의 어깨각도는 {u1Min:F1}/{u1Max:F1}도 입니다";

                                    // U2 - 팔꿈치각도
                                    double u2Min = (double?)leftData["U2"]?["min"] ?? 0;
                                    double u2Max = (double?)leftData["U2"]?["max"] ?? 0;
                                    this.leftU2.Text = $"당신의 팔꿈치각도는 {u2Min:F1}/{u2Max:F1}도 입니다";

                                    // L1 - 골반각도
                                    double l1Min = (double?)leftData["L1"]?["min"] ?? 0;
                                    double l1Max = (double?)leftData["L1"]?["max"] ?? 0;
                                    this.leftL1.Text = $"당신의 골반각도는 {l1Min:F1}/{l1Max:F1}도 입니다";

                                    // L2 - 무릎각도
                                    double l2Min = (double?)leftData["L2"]?["min"] ?? 0;
                                    double l2Max = (double?)leftData["L2"]?["max"] ?? 0;
                                    this.leftL2.Text = $"당신의 무릎각도는 {l2Min:F1}/{l2Max:F1}도 입니다";
                                }

                                // right 데이터
                                JObject rightData = (JObject)angleData["right"];
                                if (rightData != null)
                                {
                                    // U1 - 어깨각도
                                    double u1Min = (double?)rightData["U1"]?["min"] ?? 0;
                                    double u1Max = (double?)rightData["U1"]?["max"] ?? 0;
                                    this.rightU1.Text = $"당신의 어깨각도는 {u1Min:F1}/{u1Max:F1}도 입니다";

                                    // U2 - 팔꿈치각도
                                    double u2Min = (double?)rightData["U2"]?["min"] ?? 0;
                                    double u2Max = (double?)rightData["U2"]?["max"] ?? 0;
                                    this.rightU2.Text = $"당신의 팔꿈치각도는 {u2Min:F1}/{u2Max:F1}도 입니다";

                                    // L1 - 골반각도
                                    double l1Min = (double?)rightData["L1"]?["min"] ?? 0;
                                    double l1Max = (double?)rightData["L1"]?["max"] ?? 0;
                                    this.rightL1.Text = $"당신의 골반각도는 {l1Min:F1}/{l1Max:F1}도 입니다";

                                    // L2 - 무릎각도
                                    double l2Min = (double?)rightData["L2"]?["min"] ?? 0;
                                    double l2Max = (double?)rightData["L2"]?["max"] ?? 0;
                                    this.rightL2.Text = $"당신의 무릎각도는 {l2Min:F1}/{l2Max:F1}도 입니다";
                                }

                                Console.WriteLine($"카메라 데이터 API 응답: {responseContent}");
                                return true;
                            }else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"카메라 데이터 API 호출 실패: {response.StatusCode} - {response.ReasonPhrase}\n오류 내용: {errorContent}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"카메라 데이터 API 호출 중 오류 발생: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 카메라 데이터 파일을 찾습니다.
        /// </summary>
        private string FindCameraDataFile()
        {
            try
            {
                // 먼저 WebSocketServerThread에서 사용하는 경로 확인
                string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CameraData", "merged_data.json");
                if (File.Exists(appDataPath))
                {
                    Console.WriteLine("카메라 데이터 파일 찾음 (앱 경로): " + appDataPath);
                    return appDataPath;
                }

                // 문서 폴더에서도 확인
                string documentsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SmartShoes",
                    "CameraData",
                    "merged_data.json"
                );

                if (File.Exists(documentsPath))
                {
                    Console.WriteLine("카메라 데이터 파일 찾음 (문서 경로): " + documentsPath);
                    return documentsPath;
                }

                Console.WriteLine("카메라 데이터 파일을 찾을 수 없습니다.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"카메라 데이터 파일 검색 중 오류 발생: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 결과 데이터를 화면에 표시합니다.
        /// </summary>
        private void DisplayResults()
        {
            // 여기에 결과 데이터를 화면에 표시하는 로직 구현
            // 예: 라벨에 텍스트 설정, 차트 업데이트 등
            Console.WriteLine("결과 데이터 화면에 표시 완료");
        }

        /// <summary>
        /// 페이지 이동을 요청합니다.
        /// </summary>
        /// <param name="pageType">이동할 페이지의 타입</param>
        protected void MovePage(Type pageType)
        {
            PageChangeRequested?.Invoke(this, new PageChangeEventArgs(pageType));
        }

        #region :: 측정데이터 클래스 ::
        private class MeasurementData
        {
            public long userSid { get; set; }
            public long reportSid { get; set; }
            public double containerSid { get; set; }
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
            public double loadResponce1 { get; set; }
            public double loadResponce2 { get; set; }
            public double loadResponce3 { get; set; }
            public double loadResponce4 { get; set; }
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
            public string matScoreLength { get; set; }
            public string matScoreSingleTime { get; set; }
            public string matScoreStrideTime { get; set; }
            public string matScoreAngle { get; set; }
            public string matScoreForce { get; set; }
            public string matScoreBaseOfGait { get; set; }
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
            public string matComment { get; set; }
        }

        private class ShoesData
        {
            public long userSid { get; set; }
            public long reportSid { get; set; }
            public long containerSid { get; set; }
            public string LData { get; set; }
            public string RData { get; set; }
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
            public string shoesLeftLength { get; set; }
            public string shoesRightLength { get; set; }
            public string shoesLeftTime { get; set; }
            public string shoesRightTime { get; set; }

        }
        #endregion

        private void btnNext_Click(object sender, EventArgs e)
        {
            // panel2와 print 버튼을 보이게 합니다.
            this.panel2.Visible = true;
            this.picPrint.Visible = true;

            // Next 버튼은 숨깁니다.
            this.btnNext.Visible = false;
        }

        private void picPrint_Click(object sender, EventArgs e)
        {
            // 프린트 확인 메시지 박스 표시
            DialogResult result = MessageBox.Show("프린트하시겠습니까?", "프린트 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 사용자가 '예'를 선택한 경우에만 프린트 작업을 시작
            if (result == DialogResult.Yes)
            {
                // 프린트 작업 초기화
                printDocument = new System.Drawing.Printing.PrintDocument();
                printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintPage);

                // 인쇄 완료 이벤트 핸들러 추가
                printDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(PrintDocument_EndPrint);

                // 인쇄 시작 페이지 설정
                currentPrintPage = 0;

                try
                {
                    // 프린트 작업 시작
                    printDocument.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("인쇄 중 오류가 발생했습니다: " + ex.Message, "인쇄 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // 인쇄 오류 시 바로 로그인 페이지로 이동
                    this.Invoke(new Action(() => MovePage(typeof(LoginForm))));
                }
            }
            else
            {
                this.Invoke(new Action(() => MovePage(typeof(LoginForm))));

            }
        }


        // 인쇄 완료 이벤트 핸들러
        private void PrintDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            // 인쇄가 완료된 후 로그인 페이지로 이동
            this.Invoke(new Action(() => MovePage(typeof(LoginForm))));
        }

        private void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            // 인쇄할 패널 결정
            Panel panelToPrint = (currentPrintPage == 0) ? panel1 : panel2;

            // 패널의 비트맵 생성
            Bitmap bmp = new Bitmap(panelToPrint.Width, panelToPrint.Height);
            panelToPrint.DrawToBitmap(bmp, new Rectangle(0, 0, panelToPrint.Width, panelToPrint.Height));

            // 프린트 페이지 크기
            var pageWidth = e.PageBounds.Width;
            var pageHeight = e.PageBounds.Height;

            // 페이지에 가득 차도록 그리기
            g.DrawImage(bmp, 0, 0, pageWidth, pageHeight);

            // 다음 페이지 설정
            currentPrintPage++;

            // 아직 인쇄할 페이지가 남아있는지 확인
            e.HasMorePages = (currentPrintPage < 2);

            // 비트맵 자원 해제
            bmp.Dispose();
        }

        private void txtSex2_Click(object sender, EventArgs e)
        {

        }
    }
}
