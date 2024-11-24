using System;
using System.Drawing;
using System.Windows.Forms;
using Renci.SshNet.Security;

namespace SmartShoes.Client.UI
{
    public partial class KeyInputForm : Form
    {
        public string InputKey { get; private set; } = string.Empty;

        public KeyInputForm()
        {
            InitializeComponent();

            // 폼 크기 고정
            this.ClientSize = new Size(400, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            
            // flowLayoutPanel 위치 및 크기 조정
            flowLayoutPanel.Location = new Point(20, 80);
            flowLayoutPanel.Size = new Size(400, 500);
            flowLayoutPanel.WrapContents = true;
            flowLayoutPanel.AutoScroll = true; 
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            //flowLayoutPanel.Padding = new Padding(10);

            // txtInput 크기 및 위치 조정
            txtInput.Width = 360; // 너비 확대
            txtInput.Height = 100; // 높이 확대
            txtInput.Location = new Point(20, 20);
            txtInput.Font = new Font(txtInput.Font.FontFamily, 30, FontStyle.Bold); // 폰트 크기 증가
            txtInput.TextAlign = HorizontalAlignment.Center; // 텍스트 가운데 정렬
            txtInput.BorderStyle = BorderStyle.None; // 테두리 제거

            // txtInput 배경 변경 (선택 사항)
            txtInput.BackColor = Color.White; // 배경색 설정
        }

        private void KeyInputForm_Load(object sender, EventArgs e)
        {
            // 가상 키패드 버튼 생성 (0-9, Backspace, Enter)
            string keys = "123456789";
            foreach (char key in keys)
            {
                Button btn = new Button
                {
                    Text = key.ToString(),
                    Width = 100, // 버튼 너비
                    Height = 100, // 버튼 높이
                    Margin = new Padding(10),
                    Font = new Font("맑은 고딕", 12, FontStyle.Bold)
                };
                btn.Click += VirtualKey_Click;
                flowLayoutPanel.Controls.Add(btn);
            }


            // Backspace 버튼
            Button btnBackspace = new Button
            {
                Text = "←",
                Width = 100,
                Height = 100,
                Margin = new Padding(10),
                Font = new Font("맑은 고딕", 12, FontStyle.Bold)
            };
            btnBackspace.Click += BtnBackspace_Click;
            flowLayoutPanel.Controls.Add(btnBackspace);

            Button btn0 = new Button
            {
                Text = "0",
                Width = 100, // 버튼 너비
                Height = 100, // 버튼 높이
                Margin = new Padding(10),
                Font = new Font("맑은 고딕", 12, FontStyle.Bold)
            };
            btn0.Click += VirtualKey_Click;
            flowLayoutPanel.Controls.Add(btn0);
          

            // Enter 버튼
            Button btnEnter = new Button
            {
                Text = "Enter",
                Width = 100,
                Height = 100,
                Margin = new Padding(10),
                Font = new Font("맑은 고딕", 12, FontStyle.Bold)
            };
            btnEnter.Click += BtnEnter_Click;
            flowLayoutPanel.Controls.Add(btnEnter);
        }

        private void VirtualKey_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                if (txtInput.Text.Length < 3) // 최대 3자리 입력 제한
                {
                    txtInput.Text += btn.Text;
                }
            }
        }

        private void BtnBackspace_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Length > 0)
            {
                txtInput.Text = txtInput.Text.Substring(0, txtInput.Text.Length - 1);
            }
        }

        private void BtnEnter_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Length >= 2) // 최소 2자리 입력 확인
            {
                InputKey = txtInput.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("키는 최소 2자리여야 합니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}