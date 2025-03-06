using System;
using System.Windows.Forms;
using SmartShoes.Common.Forms;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartShoes.Client.UI
{
    public partial class MeasureNomalSecond : UserControl, IPageChangeNotifier
    {

        public event EventHandler<PageChangeEventArgs> PageChangeRequested;

        private LoadingPopup loadpop = new LoadingPopup();
        private DelphiHelper dph = new DelphiHelper(@"ghwlongdllS_64.dll");

        private System.Timers.Timer closeTimer;
        private bool endMeasureBool = false;
        private bool _leftFlag = false;
        private bool _rightFlag = false;
        private bool _isDataCollectionComplete = false;
				private WebSocketServerThread wsst;


        public MeasureNomalSecond()
        {
            InitializeComponent();

#if DEBUG
            this.panel1.Visible = true;
#endif
            // BLE 데이터 수집 완료 이벤트 구독
            BLEManager.Instance.DataCollectionCompleted += BLEManager_DataCollectionCompleted;

            MeasureFunction();
        }

        // BLE 데이터 수집 완료 이벤트 핸들러
        private void BLEManager_DataCollectionCompleted(object sender, BluetoothDataCollectionEventArgs e)
        {
            // 데이터 수집 완료 상태 설정
            _isDataCollectionComplete = true;
        }

        private async void MeasureFunction()
        {
            try
            {
                // 웹소켓 서버 초기화 (아직 초기화되지 않은 경우)
                if (wsst == null)
                {
                    wsst = new WebSocketServerThread("0.0.0.0", 8080);
                    wsst.SetLogCallback((message) => Console.WriteLine(message));
                    wsst.Start();
                    
                    // 클라이언트 연결 이벤트 핸들러 설정
                    wsst.OnClientConnected = (message) => {
                        Console.WriteLine($"카메라 클라이언트 연결됨: {message}");
                    };
                }
                
                // 델파이 폼 표시
                if (dph != null)
                {
                    var showForm = dph.GetFunction<DelphiHelper.TShowForm>("ShowForm");
                    showForm(panel1.Handle, 0, 0, 0, 0, true);
                    
                    // 측정 시작
                    var measurestart = dph.GetFunction<DelphiHelper.TMeasurestart>("Measurestart");
                    measurestart(20);
                    
                    // 웹소켓을 통해 카메라 측정 시작 신호 전송
                    // 현재 시간을 폴더명으로 사용 (TotalProcessForm.cs 참고)
                    string folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + UserInfo.Instance.UserName;
                    wsst.BroadcastMessage("start", folderName);
                    Console.WriteLine("카메라 측정 시작 신호 전송: " + folderName);
                    
                    // BLE 데이터 수집 시작
                    BLEManager.Instance.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"측정 시작 중 오류 발생: {ex.Message}");
            }
        }


        private void btnReStart_Click(object sender, EventArgs e)
        {
            if (endMeasureBool) { return; }

            if (!loadpop.Visible) { loadpop.Show(); }
            Application.DoEvents();

            // 데이터 수집이 완료되지 않았다면 비동기적으로 기다림
            if (!_isDataCollectionComplete)
            {
                // 비동기적으로 데이터 수집 완료를 기다림
                Task.Run(async () => 
                {
                    // 데이터 수집이 완료될 때까지 대기
                    while (!_isDataCollectionComplete && !endMeasureBool)
                    {
                        await Task.Delay(500); // 0.5초마다 확인
                    }
                    
                    // UI 스레드에서 완료 처리 실행
                    this.Invoke(new Action(() => 
                    {
                        if (!endMeasureBool) // 아직 종료되지 않았다면
                        {
                            RestartMeasurement();
                        }
                    }));
                });
                
                return;
            }
            
            // 이미 데이터 수집이 완료된 경우 바로 재시작 처리
            RestartMeasurement();
        }

        // 측정 재시작 메서드
        private void RestartMeasurement()
        {
            try
            {
                // 웹소켓을 통해 카메라 측정 중지 신호 전송
                if (wsst != null)
                {
                    string folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + UserInfo.Instance.UserName;
                    wsst.BroadcastMessage("stop", folderName);
                    Console.WriteLine("카메라 측정 중지 신호 전송: " + folderName);
                }
                
                var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
                measurestop(false);

                var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
                closeForm();

                this.endMeasureBool = true;

                this.Invoke(new Action(() => MovePage(typeof(MeasureReadyForm))));

                loadpop.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"측정 재시작 중 오류 발생: {ex.Message}");
            }
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (endMeasureBool) { return; }
            
            // 로딩 팝업 표시
            if (!loadpop.Visible) { loadpop.Show(); }
            
            Application.DoEvents();

            // 데이터 수집이 완료되지 않았다면 비동기적으로 기다림
            if (!_isDataCollectionComplete)
            {
                // 비동기적으로 데이터 수집 완료를 기다림
                Task.Run(async () => 
                {
                    // 데이터 수집이 완료될 때까지 대기
                    while (!_isDataCollectionComplete && !endMeasureBool)
                    {
                        await Task.Delay(500); // 0.5초마다 확인
                    }
                    
                    // UI 스레드에서 완료 처리 실행
                    this.Invoke(new Action(() => 
                    {
                        if (!endMeasureBool) // 아직 종료되지 않았다면
                        {
                            CompleteAndMoveToResult();
                        }
                    }));
                });
                
                return;
            }
            
            // 이미 데이터 수집이 완료된 경우 바로 완료 처리
            CompleteAndMoveToResult();
        }

        // 측정 완료 및 결과 화면으로 이동하는 메서드
        private void CompleteAndMoveToResult()
        {
            try
            {
                // 웹소켓을 통해 카메라 측정 중지 신호 전송
                if (wsst != null)
                {
                    string folderName = DateTime.Now.ToString("yyyyMMdd_HHmmss_") + UserInfo.Instance.UserName;
                    wsst.BroadcastMessage("stop", folderName);
                    Console.WriteLine("카메라 측정 중지 신호 전송: " + folderName);
                }
                
                // 측정 중지
                var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
                measurestop(true);

                var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
                closeForm();

                // 데이터베이스에서 데이터를 가져와 MatDataManager에 설정
                SaveMatDataFromDatabase();

                this.endMeasureBool = true;

                this.Invoke(new Action(() => MovePage(typeof(MeasureResultForm2))));

                loadpop.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"측정 완료 중 오류 발생: {ex.Message}");
            }
        }

        private void SaveMatDataFromDatabase()
        {
            Sqlite3Helper dbHelper = new Sqlite3Helper();
            var records = dbHelper.GetQueryRecords("SELECT * FROM ghwgaitchk1 ORDER BY ID DESC LIMIT 1;");

            #region ::: key value Division :::
            Dictionary<string, string> recordDict = new Dictionary<string, string>();
            foreach (var record in records)
            {
                foreach (var kvp in record)
                {
                    if (kvp.Key.Length > 3)
                    {
                        if (kvp.Key.Substring(0, 3).Equals("val"))
                        {
                            string keywordg = "";
                            switch (kvp.Key.Substring(3, 2))
                            {
                                case "01":
                                    keywordg = "StepLength";
                                    break;
                                case "02":
                                    keywordg = "StrideLength";
                                    break;
                                case "03":
                                    keywordg = "SingleStepTime";
                                    break;
                                case "04":
                                    keywordg = "StepAngle";
                                    break;
                                case "05":
                                    keywordg = "StepCount";
                                    break;
                                case "06":
                                    keywordg = "BaseOfGait";
                                    break;
                                case "07":
                                    keywordg = "StepForce";
                                    break;
                                case "08":
                                    keywordg = "StancePhase";
                                    break;
                                case "09":
                                    keywordg = "SwingPhase";
                                    break;
                                case "10":
                                    keywordg = "SingleSupport";
                                    break;
                                case "11":
                                    keywordg = "TotalDoubleSupport";
                                    break;
                                case "12":
                                    keywordg = "LoadResponce";
                                    break;
                                case "13":
                                    keywordg = "PreSwing";
                                    break;
                                case "14":
                                    keywordg = "StepPosition";
                                    break;
                                case "15":
                                    keywordg = "StrideTime";
                                    break;
                                case "16":
                                    keywordg = "StanceTime";
                                    break;
                                case "17":
                                    keywordg = "CopLength";
                                    break;
                                case "18":
                                    keywordg = "val18";
                                    break;
                                case "19":
                                    keywordg = "val19";
                                    break;
                            }
                            recordDict[keywordg + kvp.Key.Substring(kvp.Key.Length - 1)] = kvp.Value.ToString();
                        }
                        else
                        {
                            recordDict[kvp.Key] = kvp.Value.ToString();
                        }
                    }
                    else
                    {
                        recordDict[kvp.Key] = kvp.Value.ToString();
                    }
                }
            }
            #endregion

            #region ::: Value Input :::
            double StepLength1 = ParseDoubleOrDefault(recordDict["StepLength1"].ToString());
            double StepLength2 = ParseDoubleOrDefault(recordDict["StepLength2"].ToString());
            double StepLength3 = ParseDoubleOrDefault(recordDict["StepLength3"].ToString());
            double StepLength4 = ParseDoubleOrDefault(recordDict["StepLength4"].ToString());

            double StrideLength1 = ParseDoubleOrDefault(recordDict["StrideLength1"].ToString());
            double StrideLength2 = ParseDoubleOrDefault(recordDict["StrideLength2"].ToString());
            double StrideLength3 = ParseDoubleOrDefault(recordDict["StrideLength3"].ToString());
            double StrideLength4 = ParseDoubleOrDefault(recordDict["StrideLength4"].ToString());

            double SingleStepTime1 = ParseDoubleOrDefault(recordDict["SingleStepTime1"].ToString());
            double SingleStepTime2 = ParseDoubleOrDefault(recordDict["SingleStepTime2"].ToString());
            double SingleStepTime3 = ParseDoubleOrDefault(recordDict["SingleStepTime3"].ToString());
            double SingleStepTime4 = ParseDoubleOrDefault(recordDict["SingleStepTime4"].ToString());

            double StepAngle1 = ParseDoubleOrDefault(recordDict["StepAngle1"].ToString());
            double StepAngle2 = ParseDoubleOrDefault(recordDict["StepAngle2"].ToString());
            double StepAngle3 = ParseDoubleOrDefault(recordDict["StepAngle3"].ToString());
            double StepAngle4 = ParseDoubleOrDefault(recordDict["StepAngle4"].ToString());

            double StepCount1 = ParseDoubleOrDefault(recordDict["StepCount1"].ToString());
            double StepCount2 = ParseDoubleOrDefault(recordDict["StepCount2"].ToString());
            double StepCount3 = ParseDoubleOrDefault(recordDict["StepCount3"].ToString());
            double StepCount4 = ParseDoubleOrDefault(recordDict["StepCount4"].ToString());

            double BaseOfGait1 = ParseDoubleOrDefault(recordDict["BaseOfGait1"].ToString());
            double BaseOfGait2 = ParseDoubleOrDefault(recordDict["BaseOfGait2"].ToString());
            double BaseOfGait3 = ParseDoubleOrDefault(recordDict["BaseOfGait3"].ToString());
            double BaseOfGait4 = ParseDoubleOrDefault(recordDict["BaseOfGait4"].ToString());

            double StepForce1 = ParseDoubleOrDefault(recordDict["StepForce1"].ToString());
            double StepForce2 = ParseDoubleOrDefault(recordDict["StepForce2"].ToString());
            double StepForce3 = ParseDoubleOrDefault(recordDict["StepForce3"].ToString());
            double StepForce4 = ParseDoubleOrDefault(recordDict["StepForce4"].ToString());

            double StancePhase1 = ParseDoubleOrDefault(recordDict["StancePhase1"].ToString());
            double StancePhase2 = ParseDoubleOrDefault(recordDict["StancePhase2"].ToString());
            double StancePhase3 = ParseDoubleOrDefault(recordDict["StancePhase3"].ToString());
            double StancePhase4 = ParseDoubleOrDefault(recordDict["StancePhase4"].ToString());

            double SwingPhase1 = ParseDoubleOrDefault(recordDict["SwingPhase1"].ToString());
            double SwingPhase2 = ParseDoubleOrDefault(recordDict["SwingPhase2"].ToString());
            double SwingPhase3 = ParseDoubleOrDefault(recordDict["SwingPhase3"].ToString());
            double SwingPhase4 = ParseDoubleOrDefault(recordDict["SwingPhase4"].ToString());

            double SingleSupport1 = ParseDoubleOrDefault(recordDict["SingleSupport1"].ToString());
            double SingleSupport2 = ParseDoubleOrDefault(recordDict["SingleSupport2"].ToString());
            double SingleSupport3 = ParseDoubleOrDefault(recordDict["SingleSupport3"].ToString());
            double SingleSupport4 = ParseDoubleOrDefault(recordDict["SingleSupport4"].ToString());

            double TotalDoubleSupport1 = ParseDoubleOrDefault(recordDict["TotalDoubleSupport1"].ToString());
            double TotalDoubleSupport2 = ParseDoubleOrDefault(recordDict["TotalDoubleSupport2"].ToString());
            double TotalDoubleSupport3 = ParseDoubleOrDefault(recordDict["TotalDoubleSupport3"].ToString());
            double TotalDoubleSupport4 = ParseDoubleOrDefault(recordDict["TotalDoubleSupport4"].ToString());

            double LoadResponce1 = ParseDoubleOrDefault(recordDict["LoadResponce1"].ToString());
            double LoadResponce2 = ParseDoubleOrDefault(recordDict["LoadResponce2"].ToString());
            double LoadResponce3 = ParseDoubleOrDefault(recordDict["LoadResponce3"].ToString());
            double LoadResponce4 = ParseDoubleOrDefault(recordDict["LoadResponce4"].ToString());

            double PreSwing1 = ParseDoubleOrDefault(recordDict["PreSwing1"].ToString());
            double PreSwing2 = ParseDoubleOrDefault(recordDict["PreSwing2"].ToString());
            double PreSwing3 = ParseDoubleOrDefault(recordDict["PreSwing3"].ToString());
            double PreSwing4 = ParseDoubleOrDefault(recordDict["PreSwing4"].ToString());

            double StepPosition1 = ParseDoubleOrDefault(recordDict["StepPosition1"].ToString());
            double StepPosition2 = ParseDoubleOrDefault(recordDict["StepPosition2"].ToString());
            double StepPosition3 = ParseDoubleOrDefault(recordDict["StepPosition3"].ToString());
            double StepPosition4 = ParseDoubleOrDefault(recordDict["StepPosition4"].ToString());

            double StrideTime1 = ParseDoubleOrDefault(recordDict["StrideTime1"].ToString());
            double StrideTime2 = ParseDoubleOrDefault(recordDict["StrideTime2"].ToString());
            double StrideTime3 = ParseDoubleOrDefault(recordDict["StrideTime3"].ToString());
            double StrideTime4 = ParseDoubleOrDefault(recordDict["StrideTime4"].ToString());

            double StanceTime1 = ParseDoubleOrDefault(recordDict["StanceTime1"].ToString());
            double StanceTime2 = ParseDoubleOrDefault(recordDict["StanceTime2"].ToString());
            double StanceTime3 = ParseDoubleOrDefault(recordDict["StanceTime3"].ToString());
            double StanceTime4 = ParseDoubleOrDefault(recordDict["StanceTime4"].ToString());

            double CopLength1 = ParseDoubleOrDefault(recordDict["CopLength1"].ToString());
            double CopLength2 = ParseDoubleOrDefault(recordDict["CopLength2"].ToString());
            double CopLength3 = ParseDoubleOrDefault(recordDict["CopLength3"].ToString());
            double CopLength4 = ParseDoubleOrDefault(recordDict["CopLength4"].ToString());

            MatDataManager.Instance.SetMatData(StepLength1, StepLength2, StepLength3, StepLength4
                    , StrideLength1, StrideLength2, StrideLength3, StrideLength4
                    , SingleStepTime1, SingleStepTime2, SingleStepTime3, SingleStepTime4
                    , StepAngle1, StepAngle2, StepAngle3, StepAngle4
                    , StepCount1, StepCount2, StepCount3, StepCount4
                    , BaseOfGait1, BaseOfGait2, BaseOfGait3, BaseOfGait4
                    , StepForce1, StepForce2, StepForce3, StepForce4
                    , StancePhase1, StancePhase2, StancePhase3, StancePhase4
                    , SwingPhase1, SwingPhase2, SwingPhase3, SwingPhase4
                    , SingleSupport1, SingleSupport2, SingleSupport3, SingleSupport4
                    , TotalDoubleSupport1, TotalDoubleSupport2, TotalDoubleSupport3, TotalDoubleSupport4
                    , LoadResponce1, LoadResponce2, LoadResponce3, LoadResponce4
                    , PreSwing1, PreSwing2, PreSwing3, PreSwing4
                    , StepPosition1, StepPosition2, StepPosition3, StepPosition4
                    , StrideTime1, StrideTime2, StrideTime3, StrideTime4
                    , StanceTime1, StanceTime2, StanceTime3, StanceTime4
                    , CopLength1, CopLength2, CopLength3, CopLength4);
            #endregion
        }

        public double ParseDoubleOrDefault(string input)
        {
            // out 키워드를 사용하여 TryParse의 결과와 변환된 값을 반환합니다.
            if (double.TryParse(input, out double result))
            {
                return result; // 변환이 성공하면 해당 값을 반환
            }
            else
            {
                return 0; // 변환이 실패하면 0을 반환
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

        // 폼 종료 시 이벤트 구독 해제 및 리소스 정리
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            
            // 이벤트 구독 해제
            BLEManager.Instance.DataCollectionCompleted -= BLEManager_DataCollectionCompleted;
            
            // 웹소켓 서버 정리
            if (wsst != null)
            {
                wsst.Stop();
                wsst = null;
            }
        }

    }
}
