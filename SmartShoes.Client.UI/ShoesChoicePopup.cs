using Newtonsoft.Json.Linq;
using SmartShoes.Common.Forms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Windows.Data.Json;

namespace SmartShoes.Client.UI
{
	public partial class ShoesChoicePopup : Form
	{
		JArray shoesArray = JArray.Parse(Properties.Settings.Default.SHOES_JSON);

		public string deviceAddressL { get; set; }
		public string deviceAddressR { get; set; }

		public ShoesChoicePopup()
		{
			InitializeComponent();
			CreateLabelsAndPictureBoxes();
		}

		#region
		
		private void CreateLabelsAndPictureBoxes()
		{
			int jcnt = 0;
			
			foreach (JObject shoe in shoesArray)
			{
				string shoeID = shoe.GetValue("shoesSid").ToString();
				string shoeName = shoe.GetValue("shoesName").ToString();
				buttonCreate(shoeName, jcnt, shoeID, "shoes");
				jcnt++;
			}

			buttonCreate("종료하기", shoesArray.Count, null, "end");	
		}

		private void buttonCreate(string shoeName, int cnt, string shoeId, string gubun)
		{
			int fixedWidthCnt =  cnt < shoesArray.Count - cnt ? 80 : 500;
			int fixedHeightCnt = cnt < shoesArray.Count - cnt ? cnt : cnt - ((shoesArray.Count) / 2);

			// Label 생성
			Label label = new Label();
			label.Text = shoeName;
			label.Anchor = System.Windows.Forms.AnchorStyles.Top;
			label.BackColor = System.Drawing.Color.Transparent;
			label.Font = new System.Drawing.Font("맑은 고딕", 38.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			label.ForeColor = System.Drawing.Color.White;
			label.Size = new System.Drawing.Size(317, 63);
			label.TextAlign = System.Drawing.ContentAlignment.TopCenter;

			// PictureBox 생성
			PictureBox pictureBox = new PictureBox();
			pictureBox.Size = new Size(387, 103); // 크기 설정
			pictureBox.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.Frame;
			pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

			if (gubun.Equals("shoes"))
			{
				pictureBox.Location = new Point(fixedWidthCnt, 370 + fixedHeightCnt * 105); // 위치 설정
				pictureBox.Click += new System.EventHandler(this.ShoesSelect_Click);
				pictureBox.Tag = shoeId;
				label.Click += (s, e) => ShoesSelect_Click(pictureBox, e);
			}
			else
			{
				pictureBox.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.Frame2;
				pictureBox.Location = new Point(290, 410 + fixedHeightCnt * 105); // 위치 설정
				pictureBox.Click += new System.EventHandler(this.Close_Click);
				label.Click += (s, e) => Close_Click(pictureBox, e);
			}

			// PictureBox와 Label을 Form에 추가
			this.Controls.Add(label);
			this.Controls.Add(pictureBox);

			label.Parent = pictureBox;
			label.Location = new Point(45, 17);
		}


		#endregion

		private void Close_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ShoesSelect_Click(object sender, EventArgs e)
		{
			Console.WriteLine(sender);
			PictureBox pb = sender as PictureBox;
			string leftShoe = "";
			string rightShoe = "";
			foreach (JObject shoe in shoesArray)
			{
				string shoeID = shoe.GetValue("shoesSid").ToString();
				if (shoeID.Equals(pb.Tag))
				{
					//leftShoe = shoe.GetValue("leftMacAddress").ToString().Replace(":", "");
					//rightShoe = shoe.GetValue("rightMacAddress").ToString().Replace(":", "");
					leftShoe = shoe.GetValue("leftMacAddress").ToString();
					rightShoe = shoe.GetValue("rightMacAddress").ToString();
					break;
				}	
			}

			deviceAddressL = leftShoe;
			deviceAddressR = rightShoe;
			this.Close();
		}

	}
}

