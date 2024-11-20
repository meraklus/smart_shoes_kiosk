namespace SmartShoes.Client.UI
{
	partial class MeasureTiny
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
			this.btnComplete = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.btnReStart = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.btnComplete)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnReStart)).BeginInit();
			this.SuspendLayout();
			// 
			// btnComplete
			// 
			this.btnComplete.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_complete;
			this.btnComplete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnComplete.Location = new System.Drawing.Point(270, 912);
			this.btnComplete.Name = "btnComplete";
			this.btnComplete.Size = new System.Drawing.Size(526, 225);
			this.btnComplete.TabIndex = 15;
			this.btnComplete.TabStop = false;
			this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.panel1.Location = new System.Drawing.Point(163, 80);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(750, 805);
			this.panel1.TabIndex = 14;
			this.panel1.Visible = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("굴림", 80F);
			this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label2.Location = new System.Drawing.Point(464, 668);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(153, 107);
			this.label2.TabIndex = 13;
			this.label2.Text = "...";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("굴림", 34.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label1.Location = new System.Drawing.Point(245, 818);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(599, 47);
			this.label1.TabIndex = 12;
			this.label1.Text = "잔걸음 측정 진행중입니다.";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox2.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.kio_logo;
			this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Default;
			this.pictureBox2.Location = new System.Drawing.Point(163, 440);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(750, 209);
			this.pictureBox2.TabIndex = 11;
			this.pictureBox2.TabStop = false;
			// 
			// btnReStart
			// 
			this.btnReStart.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btn_measu_re;
			this.btnReStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.btnReStart.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnReStart.Location = new System.Drawing.Point(270, 1185);
			this.btnReStart.Name = "btnReStart";
			this.btnReStart.Size = new System.Drawing.Size(526, 156);
			this.btnReStart.TabIndex = 10;
			this.btnReStart.TabStop = false;
			this.btnReStart.Click += new System.EventHandler(this.btnReStart_Click);
			// 
			// MeasureTiny
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.btnComplete);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.btnReStart);
			this.Name = "MeasureTiny";
			this.Size = new System.Drawing.Size(1080, 1420);
			((System.ComponentModel.ISupportInitialize)(this.btnComplete)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnReStart)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox btnComplete;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox btnReStart;
	}
}
