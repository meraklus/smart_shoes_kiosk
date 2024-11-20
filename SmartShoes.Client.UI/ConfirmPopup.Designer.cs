namespace SmartShoes.Client.UI
{
	partial class ConfirmPopup
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.txtMessage = new System.Windows.Forms.Label();
			this.btnConfirm = new System.Windows.Forms.PictureBox();
			this.btnCancel = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.popup_close;
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.pictureBox1.Location = new System.Drawing.Point(646, 46);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(102, 91);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.txtMessage.BackColor = System.Drawing.Color.Transparent;
			this.txtMessage.Font = new System.Drawing.Font("굴림", 38F, System.Drawing.FontStyle.Bold);
			this.txtMessage.Location = new System.Drawing.Point(48, 348);
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(700, 51);
			this.txtMessage.TabIndex = 2;
			this.txtMessage.Text = "메시지 입력";
			this.txtMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// btnConfirm
			// 
			this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
			this.btnConfirm.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btnConfirm;
			this.btnConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnConfirm.Location = new System.Drawing.Point(3, 616);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.Size = new System.Drawing.Size(394, 184);
			this.btnConfirm.TabIndex = 3;
			this.btnConfirm.TabStop = false;
			this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.Transparent;
			this.btnCancel.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.btnCancel;
			this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.btnCancel.Location = new System.Drawing.Point(397, 616);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(394, 184);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.TabStop = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// ConfirmPopup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.BackgroundImage = global::SmartShoes.Client.UI.Properties.Resources.popup_bg1;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(796, 800);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConfirm);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.pictureBox1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ConfirmPopup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnConfirm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label txtMessage;
		private System.Windows.Forms.PictureBox btnConfirm;
		private System.Windows.Forms.PictureBox btnCancel;
	}
}