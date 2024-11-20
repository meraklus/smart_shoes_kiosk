using System;
using System.Windows.Forms;

namespace SmartShoes.Common.Forms
{
	//public class PageChangeEventArgs : EventArgs
	//{
	//	public Control NextPage { get; set; }

	//	public PageChangeEventArgs(Control nextPage)
	//	{
	//		NextPage = nextPage;
	//	}


	//}


	public class PageChangeEventArgs : EventArgs
	{
		public Control NextPage { get; private set; }

		public PageChangeEventArgs(Type nextPageType)
		{
			try
			{
				// 새로운 페이지 인스턴스 생성
				NextPage = (Control)Activator.CreateInstance(nextPageType);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating instance of {nextPageType.Name}: {ex.Message}");
				// 예외 처리 또는 디버깅을 위한 추가 코드
				throw;
			}
		}
	}


}
