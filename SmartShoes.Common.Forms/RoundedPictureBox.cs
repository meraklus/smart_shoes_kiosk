using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace SmartShoes.Common.Forms
{
	public class RoundedPictureBox : PictureBox
	{
		private int _cornerRadius = 20;

		public int CornerRadius
		{
			get { return _cornerRadius; }
			set { _cornerRadius = value; this.Invalidate(); }
		}

		public RoundedPictureBox()
		{
			this.SizeMode = PictureBoxSizeMode.StretchImage;
			this.DoubleBuffered = true;
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

			Rectangle rect = this.ClientRectangle;
			int diameter = _cornerRadius * 5;
			GraphicsPath path = new GraphicsPath();

			// 둥근 사각형 경로 그리기
			path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
			path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
			path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
			path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
			path.CloseFigure();

			// PictureBox의 Region을 둥근 경로로 설정
			this.Region = new Region(path);

			// 이미지가 null인지 확인 후 사용
			if (this.Image != null)
			{
				try
				{
					using (TextureBrush brush = new TextureBrush(this.Image))
					{
						pe.Graphics.FillPath(brush, path);
					}
				}
				catch (Exception ex)
				{
					// 예외 처리
					Console.WriteLine($"TextureBrush 사용 중 오류 발생: {ex.Message}");
					// 대체 옵션으로 배경색을 사용
					using (SolidBrush brush = new SolidBrush(this.BackColor))
					{
						pe.Graphics.FillPath(brush, path);
					}
				}
			}
			else
			{
				// 이미지가 없을 경우 배경색으로 채우기
				using (SolidBrush brush = new SolidBrush(this.BackColor))
				{
					pe.Graphics.FillPath(brush, path);
				}
			}
		}

	}
}
