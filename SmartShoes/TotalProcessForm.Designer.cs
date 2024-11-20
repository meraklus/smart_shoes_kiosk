namespace SmartShoes
{
	partial class TotalProcessForm
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
            this.btnMatSetup = new System.Windows.Forms.Button();
            this.btnMatSetupClose = new System.Windows.Forms.Button();
            this.btnConnectSql = new System.Windows.Forms.Button();
            this.txtTest = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMeasureStart = new System.Windows.Forms.Button();
            this.btnMeasureStop = new System.Windows.Forms.Button();
            this.btnCameraStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cameraStatus1 = new System.Windows.Forms.Label();
            this.cameraStatus2 = new System.Windows.Forms.Label();
            this.cameraStatus3 = new System.Windows.Forms.Label();
            this.cameraStatus4 = new System.Windows.Forms.Label();
            this.cameraStatus5 = new System.Windows.Forms.Label();
            this.cameraStatus6 = new System.Windows.Forms.Label();
            this.cameraStatus7 = new System.Windows.Forms.Label();
            this.cameraStatus8 = new System.Windows.Forms.Label();
            this.cameraStatus9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.measureTimer = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.dteBirth = new System.Windows.Forms.DateTimePicker();
            this.genderCmb = new System.Windows.Forms.ComboBox();
            this.txtShoeSize = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnCameraTestStart = new System.Windows.Forms.Button();
            this.btnCameraTestStop = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.btnCameraStop = new System.Windows.Forms.Button();
            this.btnOpenposeCommand = new System.Windows.Forms.Button();
            this.txtOpenPoseDir = new System.Windows.Forms.TextBox();
            this.txtVideoDir = new System.Windows.Forms.TextBox();
            this.txtModelDir = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtSaveDir = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtVideoNm = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.txtFrameSep = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txtWinHeight = new System.Windows.Forms.TextBox();
            this.txtWinWidth = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.btnSensorRead = new System.Windows.Forms.Button();
            this.btnSensorDataRead = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.txtShoesMT = new System.Windows.Forms.NumericUpDown();
            this.label32 = new System.Windows.Forms.Label();
            this.txtShoesSensorL = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtShoesSensorR = new System.Windows.Forms.TextBox();
            this.sqLiteCommandBuilder1 = new System.Data.SQLite.SQLiteCommandBuilder();
            ((System.ComponentModel.ISupportInitialize)(this.txtShoesMT)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMatSetup
            // 
            this.btnMatSetup.Location = new System.Drawing.Point(11, 836);
            this.btnMatSetup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMatSetup.Name = "btnMatSetup";
            this.btnMatSetup.Size = new System.Drawing.Size(116, 49);
            this.btnMatSetup.TabIndex = 0;
            this.btnMatSetup.Text = "Mat Setup";
            this.btnMatSetup.UseVisualStyleBackColor = true;
            this.btnMatSetup.Click += new System.EventHandler(this.btnMatSetup_Click);
            // 
            // btnMatSetupClose
            // 
            this.btnMatSetupClose.Location = new System.Drawing.Point(132, 836);
            this.btnMatSetupClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMatSetupClose.Name = "btnMatSetupClose";
            this.btnMatSetupClose.Size = new System.Drawing.Size(116, 49);
            this.btnMatSetupClose.TabIndex = 1;
            this.btnMatSetupClose.Text = "Mat Setup Close";
            this.btnMatSetupClose.UseVisualStyleBackColor = true;
            this.btnMatSetupClose.Click += new System.EventHandler(this.btnMatClose_Click);
            // 
            // btnConnectSql
            // 
            this.btnConnectSql.Location = new System.Drawing.Point(251, 836);
            this.btnConnectSql.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnectSql.Name = "btnConnectSql";
            this.btnConnectSql.Size = new System.Drawing.Size(116, 49);
            this.btnConnectSql.TabIndex = 7;
            this.btnConnectSql.Text = "Connect Sqlite";
            this.btnConnectSql.UseVisualStyleBackColor = true;
            this.btnConnectSql.Click += new System.EventHandler(this.btnConnectSql_Click);
            // 
            // txtTest
            // 
            this.txtTest.Location = new System.Drawing.Point(620, 814);
            this.txtTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTest.Name = "txtTest";
            this.txtTest.Size = new System.Drawing.Size(505, 124);
            this.txtTest.TabIndex = 8;
            this.txtTest.Text = "";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(12, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1114, 430);
            this.panel1.TabIndex = 10;
            // 
            // btnMeasureStart
            // 
            this.btnMeasureStart.Location = new System.Drawing.Point(373, 836);
            this.btnMeasureStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMeasureStart.Name = "btnMeasureStart";
            this.btnMeasureStart.Size = new System.Drawing.Size(116, 49);
            this.btnMeasureStart.TabIndex = 20;
            this.btnMeasureStart.Text = "Measure Start";
            this.btnMeasureStart.UseVisualStyleBackColor = true;
            this.btnMeasureStart.Click += new System.EventHandler(this.btnMeasureStart_Click);
            // 
            // btnMeasureStop
            // 
            this.btnMeasureStop.Enabled = false;
            this.btnMeasureStop.Location = new System.Drawing.Point(495, 836);
            this.btnMeasureStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnMeasureStop.Name = "btnMeasureStop";
            this.btnMeasureStop.Size = new System.Drawing.Size(116, 49);
            this.btnMeasureStop.TabIndex = 21;
            this.btnMeasureStop.Text = "Measure Stop";
            this.btnMeasureStop.UseVisualStyleBackColor = true;
            this.btnMeasureStop.Click += new System.EventHandler(this.btnMeasureStop_Click);
            // 
            // btnCameraStart
            // 
            this.btnCameraStart.Location = new System.Drawing.Point(887, 586);
            this.btnCameraStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCameraStart.Name = "btnCameraStart";
            this.btnCameraStart.Size = new System.Drawing.Size(116, 49);
            this.btnCameraStart.TabIndex = 22;
            this.btnCameraStart.Text = "Camera Connect Start";
            this.btnCameraStart.UseVisualStyleBackColor = true;
            this.btnCameraStart.Click += new System.EventHandler(this.btnCameraStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 604);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 23;
            this.label1.Text = "Camera1 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 604);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 12);
            this.label2.TabIndex = 24;
            this.label2.Text = "Camera2 : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 604);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "Camera3 : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(664, 604);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "Camera5 : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(501, 604);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 12);
            this.label5.TabIndex = 27;
            this.label5.Text = "Camera4 : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 639);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 12);
            this.label6.TabIndex = 28;
            this.label6.Text = "Camera6 : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(175, 639);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 12);
            this.label7.TabIndex = 29;
            this.label7.Text = "Camera7 : ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(338, 639);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 12);
            this.label8.TabIndex = 30;
            this.label8.Text = "Camera8 : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(501, 639);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 12);
            this.label9.TabIndex = 31;
            this.label9.Text = "Camera9 : ";
            // 
            // cameraStatus1
            // 
            this.cameraStatus1.AutoSize = true;
            this.cameraStatus1.Location = new System.Drawing.Point(87, 604);
            this.cameraStatus1.Name = "cameraStatus1";
            this.cameraStatus1.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus1.TabIndex = 32;
            this.cameraStatus1.Text = "disconnect";
            // 
            // cameraStatus2
            // 
            this.cameraStatus2.AutoSize = true;
            this.cameraStatus2.Location = new System.Drawing.Point(250, 604);
            this.cameraStatus2.Name = "cameraStatus2";
            this.cameraStatus2.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus2.TabIndex = 33;
            this.cameraStatus2.Text = "disconnect";
            // 
            // cameraStatus3
            // 
            this.cameraStatus3.AutoSize = true;
            this.cameraStatus3.Location = new System.Drawing.Point(413, 604);
            this.cameraStatus3.Name = "cameraStatus3";
            this.cameraStatus3.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus3.TabIndex = 34;
            this.cameraStatus3.Text = "disconnect";
            // 
            // cameraStatus4
            // 
            this.cameraStatus4.AutoSize = true;
            this.cameraStatus4.Location = new System.Drawing.Point(576, 604);
            this.cameraStatus4.Name = "cameraStatus4";
            this.cameraStatus4.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus4.TabIndex = 35;
            this.cameraStatus4.Text = "disconnect";
            // 
            // cameraStatus5
            // 
            this.cameraStatus5.AutoSize = true;
            this.cameraStatus5.Location = new System.Drawing.Point(739, 604);
            this.cameraStatus5.Name = "cameraStatus5";
            this.cameraStatus5.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus5.TabIndex = 36;
            this.cameraStatus5.Text = "disconnect";
            // 
            // cameraStatus6
            // 
            this.cameraStatus6.AutoSize = true;
            this.cameraStatus6.Location = new System.Drawing.Point(87, 639);
            this.cameraStatus6.Name = "cameraStatus6";
            this.cameraStatus6.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus6.TabIndex = 37;
            this.cameraStatus6.Text = "disconnect";
            // 
            // cameraStatus7
            // 
            this.cameraStatus7.AutoSize = true;
            this.cameraStatus7.Location = new System.Drawing.Point(250, 639);
            this.cameraStatus7.Name = "cameraStatus7";
            this.cameraStatus7.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus7.TabIndex = 38;
            this.cameraStatus7.Text = "disconnect";
            // 
            // cameraStatus8
            // 
            this.cameraStatus8.AutoSize = true;
            this.cameraStatus8.Location = new System.Drawing.Point(413, 639);
            this.cameraStatus8.Name = "cameraStatus8";
            this.cameraStatus8.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus8.TabIndex = 39;
            this.cameraStatus8.Text = "disconnect";
            // 
            // cameraStatus9
            // 
            this.cameraStatus9.AutoSize = true;
            this.cameraStatus9.Location = new System.Drawing.Point(576, 639);
            this.cameraStatus9.Name = "cameraStatus9";
            this.cameraStatus9.Size = new System.Drawing.Size(67, 12);
            this.cameraStatus9.TabIndex = 40;
            this.cameraStatus9.Text = "disconnect";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 12);
            this.label10.TabIndex = 41;
            this.label10.Text = "검지매트 화면";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1068, 800);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(57, 12);
            this.label11.TabIndex = 42;
            this.label11.Text = "상태 로그";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 814);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(106, 12);
            this.label12.TabIndex = 43;
            this.label12.Text = "Measure Status : ";
            // 
            // measureTimer
            // 
            this.measureTimer.AutoSize = true;
            this.measureTimer.Location = new System.Drawing.Point(135, 814);
            this.measureTimer.Name = "measureTimer";
            this.measureTimer.Size = new System.Drawing.Size(57, 12);
            this.measureTimer.TabIndex = 44;
            this.measureTimer.Text = "측정 대기";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 480);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 45;
            this.label13.Text = "성함 : ";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(100, 477);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 21);
            this.txtName.TabIndex = 46;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 541);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 47;
            this.label14.Text = "생년월일 : ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(12, 572);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 49;
            this.label15.Text = "성별 : ";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(416, 507);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(149, 21);
            this.txtHeight.TabIndex = 52;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(328, 510);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 51;
            this.label16.Text = "키 : ";
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(416, 538);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(149, 21);
            this.txtWeight.TabIndex = 54;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(328, 541);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 12);
            this.label17.TabIndex = 53;
            this.label17.Text = "몸무게 : ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(568, 510);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(23, 12);
            this.label18.TabIndex = 55;
            this.label18.Text = "cm";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(571, 541);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(18, 12);
            this.label19.TabIndex = 56;
            this.label19.Text = "kg";
            // 
            // dteBirth
            // 
            this.dteBirth.Location = new System.Drawing.Point(100, 538);
            this.dteBirth.Name = "dteBirth";
            this.dteBirth.Size = new System.Drawing.Size(200, 21);
            this.dteBirth.TabIndex = 57;
            // 
            // genderCmb
            // 
            this.genderCmb.DisplayMember = "남";
            this.genderCmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.genderCmb.FormattingEnabled = true;
            this.genderCmb.Items.AddRange(new object[] {
            "남",
            "여"});
            this.genderCmb.Location = new System.Drawing.Point(100, 569);
            this.genderCmb.Name = "genderCmb";
            this.genderCmb.Size = new System.Drawing.Size(66, 20);
            this.genderCmb.TabIndex = 58;
            // 
            // txtShoeSize
            // 
            this.txtShoeSize.Location = new System.Drawing.Point(416, 569);
            this.txtShoeSize.Name = "txtShoeSize";
            this.txtShoeSize.Size = new System.Drawing.Size(149, 21);
            this.txtShoeSize.TabIndex = 60;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(328, 572);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(65, 12);
            this.label20.TabIndex = 59;
            this.label20.Text = "발사이즈 : ";
            // 
            // btnCameraTestStart
            // 
            this.btnCameraTestStart.Enabled = false;
            this.btnCameraTestStart.Location = new System.Drawing.Point(887, 640);
            this.btnCameraTestStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCameraTestStart.Name = "btnCameraTestStart";
            this.btnCameraTestStart.Size = new System.Drawing.Size(116, 49);
            this.btnCameraTestStart.TabIndex = 61;
            this.btnCameraTestStart.Text = "Camera Test Start";
            this.btnCameraTestStart.UseVisualStyleBackColor = true;
            this.btnCameraTestStart.Click += new System.EventHandler(this.btnCameraTestStart_Click);
            // 
            // btnCameraTestStop
            // 
            this.btnCameraTestStop.Enabled = false;
            this.btnCameraTestStop.Location = new System.Drawing.Point(1009, 640);
            this.btnCameraTestStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCameraTestStop.Name = "btnCameraTestStop";
            this.btnCameraTestStop.Size = new System.Drawing.Size(116, 49);
            this.btnCameraTestStop.TabIndex = 62;
            this.btnCameraTestStop.Text = "Camera Test Stop";
            this.btnCameraTestStop.UseVisualStyleBackColor = true;
            this.btnCameraTestStop.Click += new System.EventHandler(this.btnCameraTestStop_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(12, 510);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(187, 12);
            this.label21.TabIndex = 63;
            this.label21.Text = "(형식 문제로 인한 한글 입력불가)";
            // 
            // btnCameraStop
            // 
            this.btnCameraStop.Enabled = false;
            this.btnCameraStop.Location = new System.Drawing.Point(1009, 586);
            this.btnCameraStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCameraStop.Name = "btnCameraStop";
            this.btnCameraStop.Size = new System.Drawing.Size(116, 49);
            this.btnCameraStop.TabIndex = 65;
            this.btnCameraStop.Text = "Camera Connect Stop";
            this.btnCameraStop.UseVisualStyleBackColor = true;
            this.btnCameraStop.Click += new System.EventHandler(this.btnCameraStop_Click);
            // 
            // btnOpenposeCommand
            // 
            this.btnOpenposeCommand.Location = new System.Drawing.Point(1009, 693);
            this.btnOpenposeCommand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOpenposeCommand.Name = "btnOpenposeCommand";
            this.btnOpenposeCommand.Size = new System.Drawing.Size(116, 49);
            this.btnOpenposeCommand.TabIndex = 66;
            this.btnOpenposeCommand.Text = "Openpose Command Test";
            this.btnOpenposeCommand.UseVisualStyleBackColor = true;
            this.btnOpenposeCommand.Click += new System.EventHandler(this.btnOpenposeCommand_Click);
            // 
            // txtOpenPoseDir
            // 
            this.txtOpenPoseDir.Location = new System.Drawing.Point(98, 668);
            this.txtOpenPoseDir.Name = "txtOpenPoseDir";
            this.txtOpenPoseDir.Size = new System.Drawing.Size(437, 21);
            this.txtOpenPoseDir.TabIndex = 67;
            this.txtOpenPoseDir.Text = "C:\\Users\\gkdis\\Desktop\\openpose-1.7.0-binaries-win64-cpu-python3.7-flir-3d\\openpo" +
    "se\\bin";
            // 
            // txtVideoDir
            // 
            this.txtVideoDir.Location = new System.Drawing.Point(98, 695);
            this.txtVideoDir.Name = "txtVideoDir";
            this.txtVideoDir.Size = new System.Drawing.Size(437, 21);
            this.txtVideoDir.TabIndex = 68;
            this.txtVideoDir.Text = "C:\\camera\\20240802_135808_jangwonseok1\\camera\\05";
            // 
            // txtModelDir
            // 
            this.txtModelDir.Location = new System.Drawing.Point(98, 751);
            this.txtModelDir.Name = "txtModelDir";
            this.txtModelDir.Size = new System.Drawing.Size(437, 21);
            this.txtModelDir.TabIndex = 69;
            this.txtModelDir.Text = "C:\\Users\\gkdis\\Desktop\\openpose-1.7.0-binaries-win64-cpu-python3.7-flir-3d\\openpo" +
    "se\\models";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(11, 754);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 12);
            this.label22.TabIndex = 70;
            this.label22.Text = "모델 dir : ";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(11, 698);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(71, 12);
            this.label23.TabIndex = 71;
            this.label23.Text = "동영상 dir : ";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(11, 674);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(83, 12);
            this.label24.TabIndex = 72;
            this.label24.Text = "오픈포즈 dir : ";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(12, 782);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(59, 12);
            this.label25.TabIndex = 74;
            this.label25.Text = "저장 dir : ";
            // 
            // txtSaveDir
            // 
            this.txtSaveDir.Location = new System.Drawing.Point(98, 779);
            this.txtSaveDir.Name = "txtSaveDir";
            this.txtSaveDir.Size = new System.Drawing.Size(437, 21);
            this.txtSaveDir.TabIndex = 73;
            this.txtSaveDir.Text = "C:\\json\\opentest";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(11, 726);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(65, 12);
            this.label26.TabIndex = 76;
            this.label26.Text = "동영상명 : ";
            // 
            // txtVideoNm
            // 
            this.txtVideoNm.Location = new System.Drawing.Point(98, 723);
            this.txtVideoNm.Name = "txtVideoNm";
            this.txtVideoNm.Size = new System.Drawing.Size(437, 21);
            this.txtVideoNm.TabIndex = 75;
            this.txtVideoNm.Text = "20240802_135808_jangwonseok1";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(554, 672);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(81, 12);
            this.label27.TabIndex = 78;
            this.label27.Text = "프레임 분할 : ";
            // 
            // txtFrameSep
            // 
            this.txtFrameSep.Location = new System.Drawing.Point(657, 668);
            this.txtFrameSep.Name = "txtFrameSep";
            this.txtFrameSep.Size = new System.Drawing.Size(32, 21);
            this.txtFrameSep.TabIndex = 77;
            this.txtFrameSep.Text = "3";
            this.txtFrameSep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(556, 698);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(53, 12);
            this.label28.TabIndex = 80;
            this.label28.Text = "해상도 : ";
            // 
            // txtWinHeight
            // 
            this.txtWinHeight.Location = new System.Drawing.Point(657, 695);
            this.txtWinHeight.Name = "txtWinHeight";
            this.txtWinHeight.Size = new System.Drawing.Size(46, 21);
            this.txtWinHeight.TabIndex = 79;
            this.txtWinHeight.Text = "640";
            this.txtWinHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtWinWidth
            // 
            this.txtWinWidth.Location = new System.Drawing.Point(724, 696);
            this.txtWinWidth.Name = "txtWinWidth";
            this.txtWinWidth.Size = new System.Drawing.Size(46, 21);
            this.txtWinWidth.TabIndex = 81;
            this.txtWinWidth.Text = "320";
            this.txtWinWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(706, 699);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(12, 12);
            this.label29.TabIndex = 82;
            this.label29.Text = "x";
            // 
            // btnSensorRead
            // 
            this.btnSensorRead.Location = new System.Drawing.Point(373, 889);
            this.btnSensorRead.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSensorRead.Name = "btnSensorRead";
            this.btnSensorRead.Size = new System.Drawing.Size(116, 49);
            this.btnSensorRead.TabIndex = 83;
            this.btnSensorRead.Text = "Shoes Sensor Read";
            this.btnSensorRead.UseVisualStyleBackColor = true;
            this.btnSensorRead.Click += new System.EventHandler(this.btnSensorRead_Click);
            // 
            // btnSensorDataRead
            // 
            this.btnSensorDataRead.Enabled = false;
            this.btnSensorDataRead.Location = new System.Drawing.Point(495, 889);
            this.btnSensorDataRead.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSensorDataRead.Name = "btnSensorDataRead";
            this.btnSensorDataRead.Size = new System.Drawing.Size(116, 49);
            this.btnSensorDataRead.TabIndex = 84;
            this.btnSensorDataRead.Text = "Shoes Sensor Data Read";
            this.btnSensorDataRead.UseVisualStyleBackColor = true;
            this.btnSensorDataRead.Click += new System.EventHandler(this.btnSensorDataRead_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(556, 724);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(93, 12);
            this.label30.TabIndex = 85;
            this.label30.Text = "신발 측정시간 : ";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(709, 726);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(21, 12);
            this.label31.TabIndex = 87;
            this.label31.Text = "초 ";
            // 
            // txtShoesMT
            // 
            this.txtShoesMT.Location = new System.Drawing.Point(657, 724);
            this.txtShoesMT.Name = "txtShoesMT";
            this.txtShoesMT.Size = new System.Drawing.Size(46, 21);
            this.txtShoesMT.TabIndex = 88;
            this.txtShoesMT.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtShoesMT.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(556, 754);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(57, 12);
            this.label32.TabIndex = 90;
            this.label32.Text = "신발 좌 : ";
            // 
            // txtShoesSensorL
            // 
            this.txtShoesSensorL.Location = new System.Drawing.Point(657, 751);
            this.txtShoesSensorL.Name = "txtShoesSensorL";
            this.txtShoesSensorL.Size = new System.Drawing.Size(113, 21);
            this.txtShoesSensorL.TabIndex = 89;
            this.txtShoesSensorL.Text = "C40AA6207C6F";
            this.txtShoesSensorL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(556, 781);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(57, 12);
            this.label33.TabIndex = 92;
            this.label33.Text = "신발 우 : ";
            // 
            // txtShoesSensorR
            // 
            this.txtShoesSensorR.Location = new System.Drawing.Point(657, 778);
            this.txtShoesSensorR.Name = "txtShoesSensorR";
            this.txtShoesSensorR.Size = new System.Drawing.Size(113, 21);
            this.txtShoesSensorR.TabIndex = 91;
            this.txtShoesSensorR.Text = "E6161C3E9F6A";
            this.txtShoesSensorR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // sqLiteCommandBuilder1
            // 
            this.sqLiteCommandBuilder1.DataAdapter = null;
            this.sqLiteCommandBuilder1.QuoteSuffix = "]";
            // 
            // TotalProcessForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1137, 947);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.txtShoesSensorR);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.txtShoesSensorL);
            this.Controls.Add(this.txtShoesMT);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.btnSensorDataRead);
            this.Controls.Add(this.btnSensorRead);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.txtWinWidth);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.txtWinHeight);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.txtFrameSep);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.txtVideoNm);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.txtSaveDir);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtModelDir);
            this.Controls.Add(this.txtVideoDir);
            this.Controls.Add(this.txtOpenPoseDir);
            this.Controls.Add(this.btnOpenposeCommand);
            this.Controls.Add(this.btnCameraStop);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.btnCameraTestStop);
            this.Controls.Add(this.btnCameraTestStart);
            this.Controls.Add(this.txtShoeSize);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.genderCmb);
            this.Controls.Add(this.dteBirth);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtWeight);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.measureTimer);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cameraStatus9);
            this.Controls.Add(this.cameraStatus8);
            this.Controls.Add(this.cameraStatus7);
            this.Controls.Add(this.cameraStatus6);
            this.Controls.Add(this.cameraStatus5);
            this.Controls.Add(this.cameraStatus4);
            this.Controls.Add(this.cameraStatus3);
            this.Controls.Add(this.cameraStatus2);
            this.Controls.Add(this.cameraStatus1);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTest);
            this.Controls.Add(this.btnConnectSql);
            this.Controls.Add(this.btnCameraStart);
            this.Controls.Add(this.btnMeasureStop);
            this.Controls.Add(this.btnMeasureStart);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnMatSetupClose);
            this.Controls.Add(this.btnMatSetup);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "TotalProcessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "통합프로그램";
            ((System.ComponentModel.ISupportInitialize)(this.txtShoesMT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnMatSetup;
		private System.Windows.Forms.Button btnMatSetupClose;
		private System.Windows.Forms.Button btnConnectSql;
		private System.Windows.Forms.RichTextBox txtTest;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnMeasureStart;
		private System.Windows.Forms.Button btnMeasureStop;
		private System.Windows.Forms.Button btnCameraStart;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label cameraStatus1;
		private System.Windows.Forms.Label cameraStatus2;
		private System.Windows.Forms.Label cameraStatus3;
		private System.Windows.Forms.Label cameraStatus4;
		private System.Windows.Forms.Label cameraStatus5;
		private System.Windows.Forms.Label cameraStatus6;
		private System.Windows.Forms.Label cameraStatus7;
		private System.Windows.Forms.Label cameraStatus8;
		private System.Windows.Forms.Label cameraStatus9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label measureTimer;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtWeight;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.DateTimePicker dteBirth;
		private System.Windows.Forms.ComboBox genderCmb;
		private System.Windows.Forms.TextBox txtShoeSize;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.Button btnCameraTestStart;
		private System.Windows.Forms.Button btnCameraTestStop;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Button btnCameraStop;
		private System.Windows.Forms.Button btnOpenposeCommand;
		private System.Windows.Forms.TextBox txtOpenPoseDir;
		private System.Windows.Forms.TextBox txtVideoDir;
		private System.Windows.Forms.TextBox txtModelDir;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox txtSaveDir;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.TextBox txtVideoNm;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.TextBox txtFrameSep;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.TextBox txtWinHeight;
		private System.Windows.Forms.TextBox txtWinWidth;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Button btnSensorRead;
		private System.Windows.Forms.Button btnSensorDataRead;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.NumericUpDown txtShoesMT;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.TextBox txtShoesSensorL;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.TextBox txtShoesSensorR;
        private System.Data.SQLite.SQLiteCommandBuilder sqLiteCommandBuilder1;
    }
}