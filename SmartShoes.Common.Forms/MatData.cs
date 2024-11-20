using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartShoes.Common.Forms
{
	public class MatData
	{
		public double StepLength1 { get; set; }
		public double StepLength2 { get; set; }
		public double StepLength3 { get; set; }
		public double StepLength4 { get; set; }

		public double StrideLength1 { get; set; }
		public double StrideLength2 { get; set; }
		public double StrideLength3 { get; set; }
		public double StrideLength4 { get; set; }

		public double SingleStepTime1 { get; set; }
		public double SingleStepTime2 { get; set; }
		public double SingleStepTime3 { get; set; }
		public double SingleStepTime4 { get; set; }

		public double StepAngle1 { get; set; }
		public double StepAngle2 { get; set; }
		public double StepAngle3 { get; set; }
		public double StepAngle4 { get; set; }

		public double StepCount1 { get; set; }
		public double StepCount2 { get; set; }
		public double StepCount3 { get; set; }
		public double StepCount4 { get; set; }

		public double BaseOfGait1 { get; set; }
		public double BaseOfGait2 { get; set; }
		public double BaseOfGait3 { get; set; }
		public double BaseOfGait4 { get; set; }

		public double StepForce1 { get; set; }
		public double StepForce2 { get; set; }
		public double StepForce3 { get; set; }
		public double StepForce4 { get; set; }

		public double StancePhase1 { get; set; }
		public double StancePhase2 { get; set; }
		public double StancePhase3 { get; set; }
		public double StancePhase4 { get; set; }

		public double SwingPhase1 { get; set; }
		public double SwingPhase2 { get; set; }
		public double SwingPhase3 { get; set; }
		public double SwingPhase4 { get; set; }

		public double SingleSupport1 { get; set; }
		public double SingleSupport2 { get; set; }
		public double SingleSupport3 { get; set; }
		public double SingleSupport4 { get; set; }

		public double TotalDoubleSupport1 { get; set; }
		public double TotalDoubleSupport2 { get; set; }
		public double TotalDoubleSupport3 { get; set; }
		public double TotalDoubleSupport4 { get; set; }

		public double LoadResponce1 { get; set; }
		public double LoadResponce2 { get; set; }
		public double LoadResponce3 { get; set; }
		public double LoadResponce4 { get; set; }

		public double PreSwing1 { get; set; }
		public double PreSwing2 { get; set; }
		public double PreSwing3 { get; set; }
		public double PreSwing4 { get; set; }

		public double StepPosition1 { get; set; }
		public double StepPosition2 { get; set; }
		public double StepPosition3 { get; set; }
		public double StepPosition4 { get; set; }

		public double StrideTime1 { get; set; }
		public double StrideTime2 { get; set; }
		public double StrideTime3 { get; set; }
		public double StrideTime4 { get; set; }

		public double StanceTime1 { get; set; }
		public double StanceTime2 { get; set; }
		public double StanceTime3 { get; set; }
		public double StanceTime4 { get; set; }

		public double CopLength1 { get; set; }
		public double CopLength2 { get; set; }
		public double CopLength3 { get; set; }
		public double CopLength4 { get; set; }

		// 추가적인 사용자 정보를 여기에 추가할 수 있습니다.

		public MatData(double StepLength1, double StepLength2, double StepLength3, double StepLength4
					, double StrideLength1, double StrideLength2, double StrideLength3, double StrideLength4
					, double SingleStepTime1, double SingleStepTime2, double SingleStepTime3, double SingleStepTime4
					, double StepAngle1, double StepAngle2, double StepAngle3, double StepAngle4
					, double StepCount1, double StepCount2, double StepCount3, double StepCount4
					, double BaseOfGait1, double BaseOfGait2, double BaseOfGait3, double BaseOfGait4
					, double StepForce1, double StepForce2, double StepForce3, double StepForce4
					, double StancePhase1, double StancePhase2, double StancePhase3, double StancePhase4
					, double SwingPhase1, double SwingPhase2, double SwingPhase3, double SwingPhase4
					, double SingleSupport1, double SingleSupport2, double SingleSupport3, double SingleSupport4
					, double TotalDoubleSupport1, double TotalDoubleSupport2, double TotalDoubleSupport3, double TotalDoubleSupport4
					, double LoadResponce1, double LoadResponce2, double LoadResponce3, double LoadResponce4
					, double PreSwing1, double PreSwing2, double PreSwing3, double PreSwing4
					, double StepPosition1, double StepPosition2, double StepPosition3, double StepPosition4
					, double StrideTime1, double StrideTime2, double StrideTime3, double StrideTime4
					, double StanceTime1, double StanceTime2, double StanceTime3, double StanceTime4
					, double CopLength1, double CopLength2, double CopLength3, double CopLength4)
		{

			this.StepLength1 = StepLength1;
			this.StepLength2 = StepLength2;
			this.StepLength3 = StepLength3;
			this.StepLength4 = StepLength4;

			this.StrideLength1 = StrideLength1;
			this.StrideLength2 = StrideLength2;
			this.StrideLength3 = StrideLength3;
			this.StrideLength4 = StrideLength4;

			this.SingleStepTime1 = SingleStepTime1;
			this.SingleStepTime2 = SingleStepTime2;
			this.SingleStepTime3 = SingleStepTime3;
			this.SingleStepTime4 = SingleStepTime4;

			this.StepAngle1 = StepAngle1;
			this.StepAngle2 = StepAngle2;
			this.StepAngle3 = StepAngle3;
			this.StepAngle4 = StepAngle4;

			this.StepCount1 = StepCount1;
			this.StepCount2 = StepCount2;
			this.StepCount3 = StepCount3;
			this.StepCount4 = StepCount4;

			this.BaseOfGait1 = BaseOfGait1;
			this.BaseOfGait2 = BaseOfGait2;
			this.BaseOfGait3 = BaseOfGait3;
			this.BaseOfGait4 = BaseOfGait4;

			this.StepForce1 = StepForce1;
			this.StepForce2 = StepForce2;
			this.StepForce3 = StepForce3;
			this.StepForce4 = StepForce4;

			this.StancePhase1 = StancePhase1;
			this.StancePhase2 = StancePhase2;
			this.StancePhase3 = StancePhase3;
			this.StancePhase4 = StancePhase4;

			this.SwingPhase1 = SwingPhase1;
			this.SwingPhase2 = SwingPhase2;
			this.SwingPhase3 = SwingPhase3;
			this.SwingPhase4 = SwingPhase4;

			this.SingleSupport1 = SingleSupport1;
			this.SingleSupport2 = SingleSupport2;
			this.SingleSupport3 = SingleSupport3;
			this.SingleSupport4 = SingleSupport4;

			this.TotalDoubleSupport1 = TotalDoubleSupport1;
			this.TotalDoubleSupport2 = TotalDoubleSupport2;
			this.TotalDoubleSupport3 = TotalDoubleSupport3;
			this.TotalDoubleSupport4 = TotalDoubleSupport4;

			this.LoadResponce1 = LoadResponce1;
			this.LoadResponce2 = LoadResponce2;
			this.LoadResponce3 = LoadResponce3;
			this.LoadResponce4 = LoadResponce4;

			this.PreSwing1 = PreSwing1;
			this.PreSwing2 = PreSwing2;
			this.PreSwing3 = PreSwing3;
			this.PreSwing4 = PreSwing4;

			this.StepPosition1 = StepPosition1;
			this.StepPosition2 = StepPosition2;
			this.StepPosition3 = StepPosition3;
			this.StepPosition4 = StepPosition4;

			this.StrideTime1 = StrideTime1;
			this.StrideTime2 = StrideTime2;
			this.StrideTime3 = StrideTime3;
			this.StrideTime4 = StrideTime4;

			this.StanceTime1 = StanceTime1;
			this.StanceTime2 = StanceTime2;
			this.StanceTime3 = StanceTime3;
			this.StanceTime4 = StanceTime4;

			this.CopLength1 = CopLength1;
			this.CopLength2 = CopLength2;
			this.CopLength3 = CopLength3;
			this.CopLength4 = CopLength4;
		}



	}
}
