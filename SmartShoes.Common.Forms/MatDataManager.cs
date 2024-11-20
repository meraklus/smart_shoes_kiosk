using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartShoes.Common.Forms
{
	public class MatDataManager
	{
		private static MatDataManager _instance = null;
		private List<MatData> _savedMatData;

		// Singleton 인스턴스를 가져오는 메서드
		public static MatDataManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MatDataManager();
				}
				return _instance;
			}
		}

		// 생성자를 private으로 하여 외부에서 인스턴스를 생성하지 못하게 함
		private MatDataManager()
		{
			_savedMatData = new List<MatData>();
		}

		public void SetMatData(double StepLength1, double StepLength2, double StepLength3, double StepLength4
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
			var matData = new MatData(StepLength1, StepLength2, StepLength3, StepLength4
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

			_savedMatData.Add(matData);
		}


		public void ResetMatData()
		{
			_savedMatData.Clear();
		}


		public List<MatData> GetAllMatData()
		{
			return _savedMatData;
		}

	}
}
