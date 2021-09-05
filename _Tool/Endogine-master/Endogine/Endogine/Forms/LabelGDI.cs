using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for LabelGDI.
	/// </summary>
	public class LabelGDI : Sprite
	{
		private StringFormat m_format;
		private Font m_font;
		private int _textWidth = 100;
		private string _text;

		private bool _layoutSuspended;

		public LabelGDI()
		{
			//TODO: use a default static font
			m_font = new Font("Verdana", 8, FontStyle.Regular);

			m_format = new StringFormat();
			m_format.Alignment = StringAlignment.Near;
			m_format.Trimming = StringTrimming.Word; //EllipsisPath;
			m_format.FormatFlags = StringFormatFlags.FitBlackBox; // | StringFormatFlags.DirectionRightToLeft;
		}

		public string FontName
		{
			set
			{
				this.m_font = new Font(value, this.m_font.Size, this.m_font.Style);
				this.DoLayout();
			}
		}
		public float FontSize
		{
			set
			{
				this.m_font = new Font(this.m_font.Name, value, this.m_font.Style);
				this.DoLayout();
			}
		}
		public bool FontItalic
		{
			set
			{
				FontStyle style;
				if (value)
					style = this.m_font.Style | FontStyle.Italic;
				else
					style = this.m_font.Style & (~FontStyle.Italic); // ^

				this.m_font = new Font(this.m_font.Name, this.m_font.Size, style);
				this.DoLayout();
			}
		}

		//TODO: suspend and resume layout should be available to all form elements (controls)
		public void SuspendLayout()
		{
			this._layoutSuspended = true;
		}
		public void ResumeLayout()
		{
			this._layoutSuspended = false;
			this.DoLayout();
		}
		public void DoLayout()
		{
			if (this._layoutSuspended==false && this._text!=null)
				this.Text = this.Text;
		}

		public int TextWidth
		{
			set
			{
				this._textWidth = value;
				this.DoLayout();
			}
			get {return this._textWidth;}
		}

		public string Text
		{
			get {return this._text;}
			set
			{
				this._text = value;

				//TODO: stupid to create a bitmap just to get a graphics object... But how else?
				Bitmap bmp = new Bitmap(1,1);
				Graphics g = Graphics.FromImage(bmp);
				SizeF size = g.MeasureString(value, m_font, this._textWidth, m_format);

				//Example how to measure which text that goes on each line
//				string t = value;
//				System.Collections.ArrayList lines = new System.Collections.ArrayList();
//				while (true)
//				{
//					int charFit, linesFilled;
//					g.MeasureString(t, m_font, new Size(this._textWidth, 24), m_format, out charFit, out linesFilled);
//					lines.Add(t.Substring(0, charFit));
//					t = t.Remove(0,charFit);
//					if (t.Length == 0)
//						break;
//				}

				if (size.Width == 0)
					size.Width = 1;
				if (size.Height == 0)
					size.Height = 1;

				bmp = new Bitmap(this._textWidth,(int)size.Height, PixelFormat.Format32bppArgb);
				g = Graphics.FromImage(bmp);
				g.DrawString(value, m_font, new SolidBrush(Color.White),
					new RectangleF(0,0,bmp.Width,bmp.Height), m_format);

				MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);
				this.Member = mb;
			}
		}
	}
}
