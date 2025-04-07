using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
this.txtFootSize.Text = UserInfo.Instance.FootSize.ToString();
this.txtHeight.Text = UserInfo.Instance.Height.ToString();
this.txtSex.Text = UserInfo.Instance.Sex.ToString();
this.txtAge.Text = UserInfo.Instance.BirthYear.ToString();
this.txtName.Text = UserInfo.Instance.UserName.ToString();
*/
namespace SmartShoes.Common.Forms
{
	public class UserInfo
	{
		// Singleton 인스턴스 저장용 변수
		private static UserInfo _instance = null;

		// 사용자 정보 예시
		public string UserName { get; private set; }
		public string UserId { get; private set; }
		public int Height { get; private set; }
		public int FootSize { get; private set; }
		public string Sex { get; private set; }
		public int BirthYear { get; private set; }

		// 생성자를 private으로 하여 외부에서 인스턴스를 생성하지 못하게 함
		private UserInfo() { }

		// Singleton 인스턴스를 가져오는 메서드
		public static UserInfo Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UserInfo();
				}
				return _instance;
			}
		}

		// 로그인 시 사용자 정보를 설정하는 메서드
		public void SetUserInfo(string userName, string userId, int height, int footSize, string sex, int birthYear)
		{
			this.UserName = userName;
			this.UserId = userId;
			this.Height = height;
			this.FootSize = footSize;
			this.Sex = sex;
			this.BirthYear = birthYear;
		}


		// 로그아웃 시 사용자 정보를 초기화하는 메서드
		public void ClearUserInfo()
		{
			this.UserName = string.Empty;
			this.UserId = string.Empty;
			this.Height = 0;
			this.FootSize = 0;
			this.Sex = string.Empty;
			this.BirthYear = 0;
		}
	}


}
