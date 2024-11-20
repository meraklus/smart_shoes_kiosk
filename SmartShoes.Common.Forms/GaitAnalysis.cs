using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartShoes.Common.Forms
{
	public class GaitAnalysis
	{
		public static Dictionary<string, string> Algorithm1(MatData data, double pHeight)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data), "data cannot be null");

			double userHeight = pHeight;
			double rStepLength = data.StepLength2; // 오른발 보행 길이
			double lStepLength = data.StepLength1; // 왼발 보행 길이

			double height = userHeight - 100; // 자기 키 - 100
			double standMin = height * 0.9; // -10% 계산
			double standMax = height * 1.1; // +10% 계산

			// 결과 딕셔너리 생성
			var results = new Dictionary<string, string>
			{
				{ "Min", standMin.ToString("F1") },
				{ "Max", standMax.ToString("F1") },
				{ "Right", standMin <= rStepLength && rStepLength <= standMax ? "안정" : "불안정" },
				{ "Left", standMin <= lStepLength && lStepLength <= standMax ? "안정" : "불안정" }
			};

			return results;
		}

		public static Dictionary<string, string> Algorithm2(MatData data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data), "data cannot be null");

			double userBaseOfGait4 = data.BaseOfGait4;

			var results = new Dictionary<string, string>
			{
				{ "Min", "5.0" },  // 최소값 5
				{ "Max", "10.0" }, // 최대값 10
				{ "Right", (userBaseOfGait4 >= 5 && userBaseOfGait4 <= 10) ? "안정" : "불안정" }
			};

			return results;
		}


		public static double DataResult(double left, double right, double minAvg, double maxAvg)
		{
			if (Math.Abs(left) <= minAvg && Math.Abs(right) <= minAvg)
			{
				return ((Math.Abs(left) * 100 / minAvg) + (Math.Abs(right) * 100 / minAvg)) / 2;
			}
			else if (Math.Abs(left) > minAvg && Math.Abs(right) > minAvg)
			{
				return ((maxAvg - Math.Abs(left)) * 100 / minAvg + (maxAvg - Math.Abs(right)) * 100 / minAvg) / 2;
			}
			else if (Math.Abs(left) <= minAvg && Math.Abs(right) > minAvg)
			{
				return ((Math.Abs(left) * 100 / minAvg) + (maxAvg - Math.Abs(right)) * 100 / minAvg) / 2;
			}
			else if (Math.Abs(left) > minAvg && Math.Abs(right) <= minAvg)
			{
				return ((maxAvg - Math.Abs(left)) * 100 / minAvg + Math.Abs(right) * 100 / minAvg) / 2;
			}
			else
			{
				return 0;
			}
		}

		public static double OneDataResult(double data, double minAvg, double maxAvg)
		{
			if (Math.Abs(data) <= minAvg)
			{
				return (Math.Abs(data) * 100) / minAvg;
			}
			else if (Math.Abs(data) > minAvg)
			{
				return (maxAvg - Math.Abs(data)) * 100 / minAvg;
			}
			else
			{
				return 0;
			}
		}

		public static Dictionary<string, double> Standard1(MatData data)
		{
			double lStepLength = data.StepLength1;
			double rStepLength = data.StepLength2;
			double lStepTime = data.SingleStepTime1;
			double rStepTime = data.SingleStepTime2;
			double lStrideTime = data.StrideTime1;
			double rStrideTime = data.StrideTime2;
			double lStepAngle = data.StepAngle1;
			double rStepAngle = data.StepAngle2;
			double lStepForce = data.StepForce1;
			double rStepForce = data.StepForce2;
			double baseOfGait = data.BaseOfGait4;
			double lStancePhase = data.StancePhase1;
			double rStancePhase = data.StancePhase2;
			double lSwingPhase = data.SwingPhase1;
			double rSwingPhase = data.SwingPhase2;

			double stepLengthMinAvg = 72.0, stepLengthMaxAvg = 144.0;
			double stepTimeMinAvg = 0.73, stepTimeMaxAvg = 1.46;
			double strideTimeMinAvg = 1.37, strideTimeMaxAvg = 2.74;
			double stepAngleMinAvg = 8.0, stepAngleMaxAvg = 16.0;
			double stepForceMinAvg = 52.0, stepForceMaxAvg = 104.0;
			double baseOfGaitMinAvg = 10.0, baseOfGaitMaxAvg = 20.0;
			double stancePhaseMinAvg = 70.0, stancePhaseMaxAvg = 140.0;
			double swingPhaseMinAvg = 30.0, swingPhaseMaxAvg = 60.0;

			double stepLengthResult = DataResult(lStepLength, rStepLength, stepLengthMinAvg, stepLengthMaxAvg);
			double stepTimeResult = DataResult(lStepTime, rStepTime, stepTimeMinAvg, stepTimeMaxAvg);
			double strideTimeResult = DataResult(lStrideTime, rStrideTime, strideTimeMinAvg, strideTimeMaxAvg);
			double stepAngleResult = DataResult(lStepAngle, rStepAngle, stepAngleMinAvg, stepAngleMaxAvg);
			double stepForceResult = DataResult(lStepForce, rStepForce, stepForceMinAvg, stepForceMaxAvg);

			double baseOfGaitResult;
			if (Math.Abs(baseOfGait) <= baseOfGaitMinAvg)
			{
				baseOfGaitResult = (Math.Abs(baseOfGait) * 100) / baseOfGaitMinAvg;
			}
			else if (Math.Abs(baseOfGait) <= baseOfGaitMaxAvg)
			{
				baseOfGaitResult = ((baseOfGaitMaxAvg - Math.Abs(baseOfGait)) * 100) / baseOfGaitMinAvg;
			}
			else
			{
				baseOfGaitResult = 0;
			}

			double lStancePhaseResult = OneDataResult(lStancePhase, stancePhaseMinAvg, stancePhaseMaxAvg);
			double lSwingPhaseResult = OneDataResult(lSwingPhase, swingPhaseMinAvg, swingPhaseMaxAvg);
			double rStancePhaseResult = OneDataResult(rStancePhase, stancePhaseMinAvg, stancePhaseMaxAvg);
			double rSwingPhaseResult = OneDataResult(rSwingPhase, swingPhaseMinAvg, swingPhaseMaxAvg);

			double totalScore = (stepLengthResult + stepTimeResult + strideTimeResult + stepAngleResult + stepForceResult + baseOfGaitResult) / 6;
			double levelScore = (100 - totalScore) / 20;

			var results = new Dictionary<string, double>
			{
				{"stepLengthResult",stepLengthResult},
				{"stepTimeResult",stepTimeResult},
				{"strideTimeResult",strideTimeResult},
				{"stepAngleResult",stepAngleResult},
				{"stepForceResult",stepForceResult},
				{"baseOfGaitResult",baseOfGaitResult},
				{"totalScore",totalScore},
				{"levelScore",levelScore},
				{"lStancePhaseResult",lStancePhaseResult},
				{"lSwingPhaseResult",lSwingPhaseResult},
				{"rStancePhaseResult",rStancePhaseResult},
				{"rSwingPhaseResult",rSwingPhaseResult}
			};

			return results;
		}
	}
}
