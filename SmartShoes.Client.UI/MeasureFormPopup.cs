using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmartShoes.Common.Forms;

namespace SmartShoes.Client.UI
{
	public partial class MeasureFormPopup : Form
	{
		[DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
		private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect
													  , int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		public MeasureFormPopup()
		{
			InitializeComponent();
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void MeasureFormPopup_Load(object sender, EventArgs e)
		{
			Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 50, 50));
		}


		private void pictureBox2_Click(object sender, EventArgs e)
		{
			this.Close();
		}

	}
}
