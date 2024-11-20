namespace SmartShoes.Client.UI
{
	partial class MeasureForm
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.btnTiny = new System.Windows.Forms.PictureBox();
			this.btnNomal = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnTiny)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnNomal)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox3
			// 
			this.pictureBox3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox3.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.welcome;
			this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Default;
			this.pictureBox3.Location = new System.Drawing.Point(161, 299);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(750, 209);
			this.pictureBox3.TabIndex = 6;
			this.pictureBox3.TabStop = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox1.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.kio_img_people;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
			this.pictureBox1.Location = new System.Drawing.Point(123, 514);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(829, 435);
			this.pictureBox1.TabIndex = 5;
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox2.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.kio_logo;
			this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Default;
			this.pictureBox2.Location = new System.Drawing.Point(161, 84);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(750, 209);
			this.pictureBox2.TabIndex = 4;
			this.pictureBox2.TabStop = false;
			// 
			// btnTiny
			// 
			this.btnTiny.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnTiny.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_measu_tiny;
			this.btnTiny.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnTiny.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnTiny.Location = new System.Drawing.Point(268, 1185);
			this.btnTiny.Name = "btnTiny";
			this.btnTiny.Size = new System.Drawing.Size(535, 156);
			this.btnTiny.TabIndex = 1;
			this.btnTiny.TabStop = false;
			this.btnTiny.Click += new System.EventHandler(this.btnTiny_Click);
			// 
			// btnNomal
			// 
			this.btnNomal.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnNomal.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_measu_normal;
			this.btnNomal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnNomal.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnNomal.Location = new System.Drawing.Point(268, 1004);
			this.btnNomal.Name = "btnNomal";
			this.btnNomal.Size = new System.Drawing.Size(535, 156);
			this.btnNomal.TabIndex = 0;
			this.btnNomal.TabStop = false;
			this.btnNomal.Click += new System.EventHandler(this.btnNomal_Click);
			// 
			// MeasureForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.btnTiny);
			this.Controls.Add(this.btnNomal);
			this.Name = "MeasureForm";
			this.Size = new System.Drawing.Size(1080, 1420);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnTiny)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnNomal)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox btnNomal;
		private System.Windows.Forms.PictureBox btnTiny;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox3;
	}
}
