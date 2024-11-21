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


		public MeasureNomalSecond()
		{
			InitializeComponent();

#if DEBUG
			this.panel1.Visible = true;
#endif

			MeasureFunction();
		}


		private async void MeasureFunction()
		{
			//BLEManager.Instance.ThresholdReachedR -= HandleThresholdReachedR;
			//BLEManager.Instance.ThresholdReachedL -= HandleThresholdReachedL;

			// Mat Measure
			if (dph != null)
			{
				var showForm = dph.GetFunction<DelphiHelper.TShowForm>("ShowForm");
				showForm(panel1.Handle, 0, 0, 0, 0, true);


				var measurestart = dph.GetFunction<DelphiHelper.TMeasurestart>("Measurestart");
				measurestart(20);

				StartCloseTimer();
			}

			//BLEManager.Instance.ThresholdReachedR += HandleThresholdReachedR;
			//BLEManager.Instance.ThresholdReachedL += HandleThresholdReachedL;


			// Sensor Device Measure
			var rightDevice = BLEManager.Instance.GetRightDevice();
			if (rightDevice != null)
			{
				Console.WriteLine($"Right device: {rightDevice.Name}");
			}
			else
			{
				Console.WriteLine("Right device is not connected.");
			}

			var leftDevice = BLEManager.Instance.GetLeftDevice();
			if (leftDevice != null)
			{
				Console.WriteLine($"Left device: {leftDevice.Name}");
			}
			else
			{
				Console.WriteLine("Left device is not connected.");
			}

			await functiontest(rightDevice);
			await functiontest(leftDevice);
		}


		private void HandleThresholdReachedL(object sender, EventArgs args)
		{
			this._leftFlag = true;
			
			var df = BLEManager.Instance._parsedDataL;
			var dfe = BLEManager.Instance._parsedDataR;

			if (!endMeasureBool) { return; }
			if (!(_leftFlag && _rightFlag)) { return; }
			// loadpop.Close()를 UI 스레드에서 실행
			if (loadpop.InvokeRequired)
			{
				loadpop.Invoke(new Action(() => loadpop.Close()));
			}
			else
			{
				loadpop.Close();
			}
			this.Invoke(new Action(() => MovePage(typeof(MeasureResultForm2))));
		}

		private void HandleThresholdReachedR(object sender, EventArgs args)
		{
			this._rightFlag = true;
			
			var df = BLEManager.Instance._parsedDataL;
			var dfe = BLEManager.Instance._parsedDataR;
			
			if (!endMeasureBool) { return; }
			if (!(_leftFlag && _rightFlag)) { return; }
			// loadpop.Close()를 UI 스레드에서 실행
			if (loadpop.InvokeRequired)
			{
				loadpop.Invoke(new Action(() => loadpop.Close()));
			}
			else
			{
				loadpop.Close();
			}
			this.Invoke(new Action(() => MovePage(typeof(MeasureResultForm2))));
		}

		static async Task functiontest(InTheHand.Bluetooth.BluetoothDevice device)
		{
			Guid writeUuid = Guid.Parse(Properties.Settings.Default.WRITE_UUID); // 데이터 송신 Characteristic UUID

			var services = await device.Gatt.GetPrimaryServicesAsync();
			foreach (var service in services)
			{
				var characteristics = await service.GetCharacteristicsAsync();
				Console.WriteLine(characteristics);
				foreach (var characteristic in characteristics)
				{
					if (characteristic.Uuid == writeUuid)
					{
						string dataToSend = $"@DATA,{Properties.Settings.Default.SENSOR_SET_TIME * 100}#\r\n";
						byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
						await characteristic.WriteValueWithResponseAsync(dataBytes);
					}
				}
			}
		}

		private void StartCloseTimer()
		{
			// 10초 (10000 밀리초) 타이머 설정
			closeTimer = new System.Timers.Timer(20000);
			closeTimer.Elapsed += OnTimedEvent;
			closeTimer.AutoReset = false; // 한 번만 실행하도록 설정
			closeTimer.Start();
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Action(() => OnTimedEvent(sender, e)));
			}
			else
			{
				if (endMeasureBool) { return; }
				if (!loadpop.Visible) { loadpop.Show(); }

				var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
				measurestop(true);

				var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
				closeForm();

				this.endMeasureBool = true;

				//if (!(_leftFlag && _rightFlag)) { return; }
				
				this.Invoke(new Action(() => MovePage(typeof(MeasureResultForm2))));
				loadpop.Close();
			}
		}

		private void btnReStart_Click(object sender, EventArgs e)
		{
			if (endMeasureBool) { return; }

			if (!loadpop.Visible) { loadpop.Show(); }
			Application.DoEvents();

			var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
			measurestop(false);

			var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
			closeForm();

			this.endMeasureBool = true;

			this.Invoke(new Action(() => MovePage(typeof(MeasureReadyForm))));

			loadpop.Close();
		}

		private void btnComplete_Click(object sender, EventArgs e)
		{
			if (endMeasureBool) { return; }
			if (!loadpop.Visible) { loadpop.Show(); }

			Application.DoEvents();

			var measurestop = dph.GetFunction<DelphiHelper.TMeasurestop>("Measurestop");
			measurestop(true);

			var closeForm = dph.GetFunction<DelphiHelper.TCloseForm>("CloseForm");
			closeForm();

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

			this.endMeasureBool = true;

			//if (!(_leftFlag && _rightFlag)) {
   //             Console.WriteLine("신발 플래그가 딸려서 리턴남");
   //             return; 
			//}
			this.Invoke(new Action(() => MovePage(typeof(MeasureResultForm2))));

			loadpop.Close();
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


	}
}
