using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Endogine.Text
{
	/// <summary>
	/// Summary description for FontGenerator.
	/// </summary>
	public class FontGenerator
	{

		public enum CharSet
		{
            Default,
			Numbers,
			Alphabet,
			AlphaNumeric
		}

		private Font _font;
		private List<object> _pensAndBrushes;

		private string _definedCharacters;

        private Dictionary<char, int[,]> _profiles;

		public FontGenerator()
		{
			this._definedCharacters = "";
            this._profiles = new Dictionary<char, int[,]>();
            this._pensAndBrushes = new List<object>();
		}

		//TODO: recreate bitmaps after any property change
		public Font Font
		{
			set
			{
				this._font = value;
			}
			get {return this._font; }
		}
		public string FontName
		{
			set
			{
				this._font = new Font(value, this._font.Size, this._font.Style);
				//this.DoLayout();
			}
		}
		public float FontSize
		{
			set
			{
				this._font = new Font(this._font.Name, value, this._font.Style);
				//this.DoLayout();
			}
		}
		public bool FontItalic
		{
			set
			{
				FontStyle style;
				if (value)
					style = this._font.Style | FontStyle.Italic;
				else
					style = this._font.Style & (~FontStyle.Italic); // ^

				this._font = new Font(this._font.Name, this._font.Size, style);
				//this.DoLayout();
			}
		}


		public List<object> PensAndBrushes
		{
			get {return this._pensAndBrushes;}
			set {this._pensAndBrushes = value;}
		}
		public void AddPenOrBrush(object o)
		{
			this._pensAndBrushes.Add(o);
		}

		public string DefinedCharacters
		{
			get {return this._definedCharacters;}
			set
			{
				if (this._font!=null)
				{
					string sNewChars = "";
					foreach (char c in value)
					{
						if (this._definedCharacters.IndexOf(c) == -1)
							sNewChars+=c;
					}
                    Dictionary<char, int[,]> newProfiles = FontGenerator.CreatePicRefs(this._font, sNewChars, this._pensAndBrushes);
					foreach (KeyValuePair<char, int[,]> de in newProfiles)
						this._profiles.Add(de.Key, de.Value);
					this._definedCharacters = value;
				}
			}
		}

		public int GetKerningBetween(char c1, char c2)
		{
			PicRef p1 = this[c1];
            if (p1 == null)
                return 0;
			PicRef p2 = this[c2];
            if (p2 == null)
                return 0;
            int kern = CalcKerningBetween(this.GetProfile(c1), this.GetProfile(c2), p1.Offset.Y - p2.Offset.Y);
			return kern;
		}

        private int[,] GetProfile(char c)
        {
            //TODO: check for replacements
            return this._profiles[c];
        }

		public PicRef this[char c]
		{
			get 
			{
				if (this._definedCharacters.IndexOf(c) == -1)
					this._definedCharacters+=c.ToString();
				return PicRef.Get(GetPrefix(this._font)+"_"+c.ToString());
			}
		}

		public static string GetPrefix(Font font)
		{
			string name = font.Name+"_"+font.Size;
			string style = "";
			if (font.Italic)
				style+="i";
			if (font.Bold)
				style+="b";
			if (font.Underline)
				style+="u";
			if (font.Strikeout)
				style+="s";
			if (style.Length > 0)
				name+="_"+style;
			return name;
		}

        public static Dictionary<char, int[,]> CreatePicRefs(Font font, string characters, List<object> pensAndBrushes)
		{
			Bitmap[] bitmaps = FontGenerator.Generate(font, characters, pensAndBrushes);
			ERectangle[] rects;
			EPoint[] offsets;

			Bitmap bmpLarge = Endogine.BitmapHelpers.TexturePacking.TreePack(bitmaps, out rects, out offsets); //.PackBitmapsIntoOneLarge(trimmedBmps, null, out node);

            Dictionary<char, int[,]> profiles = new Dictionary<char, int[,]>();
			for (int i=0; i<bitmaps.Length; i++)
			{
				Bitmap bmp = bitmaps[i];
				int[,] profile = GetProfile(bmp);
				profiles.Add(characters[i], profile);
			}

			MemberSpriteBitmap mb = new MemberSpriteBitmap(bmpLarge);
			string name = GetPrefix(font);
			mb.Name = name;

			string[] names = new string[characters.Length];
			for (int i=0; i<characters.Length;i++)
				names[i] = ((int)characters[i]).ToString();

            //bmpLarge.Save("_fonttest.png");
            //Endogine.BitmapHelpers.TexturePacking.CreateDocFromRectsAndOffsets(rects, offsets, names).Save("_fonttest.xml");

			for (int i=0; i<bitmaps.Length; i++)
			{
				PicRef pic = new PicRef(name+"_"+characters.Substring(i,1), mb);
				pic.Offset = new EPoint(0,offsets[i].Y); //Ignore the X offset
				pic.SourceRectangle = rects[i];
				//EH.Instance.CastLib.Pictures.AddPicture(pic);
			}

			return profiles;
		}

		public static Bitmap CreateCharacterSetBitmap(Font font, string characters, List<object> pensAndBrushes, out System.Xml.XmlDocument doc)
		{
			//TODO: not an Xml doc, return a Endogine.Node instead! where node.Name = character
			Bitmap[] bitmaps = FontGenerator.Generate(font, characters, pensAndBrushes);

			string[] names = new string[bitmaps.Length];
			for (int i=0; i<characters.Length; i++)
				names[i] = ((int)characters[i]).ToString();
			ERectangle[] rects;
			EPoint[] offsets;
			Bitmap bmpLarge = Endogine.BitmapHelpers.TexturePacking.TreePack(bitmaps, out rects, out offsets); //.PackBitmapsIntoOneLarge(trimmedBmps, null, out node);

			doc = Endogine.BitmapHelpers.TexturePacking.CreateDocFromRectsAndOffsets(rects, offsets, names);
			return bmpLarge;
		}


		public static Bitmap[] Generate(Font font, string characters, List<object> pensAndBrushes)
		{
			if (pensAndBrushes == null)
			{
				pensAndBrushes = new List<object>();
				pensAndBrushes.Add(new SolidBrush(Color.FromArgb(255,255,255)));
				}

			StringFormat format = new StringFormat();
			//			format.Alignment = StringAlignment.Near;
			//			format.Trimming = StringTrimming.Word; //EllipsisPath;
			//			format.FormatFlags = StringFormatFlags.FitBlackBox; // | StringFormatFlags.DirectionRightToLeft;

			Bitmap bmpMeasuring = new Bitmap(1,1);
			Graphics gMeasuring = Graphics.FromImage(bmpMeasuring);

			Point ptExtraSize = new Point(0,0);
			float extra = 0;
			foreach (object pOrB in pensAndBrushes)
			{
				if (pOrB.GetType() == typeof(Pen))
					extra = Math.Max(((Pen)pOrB).Width, extra);
			}
			int nExtra = (int)Math.Ceiling(extra);
			ptExtraSize = new Point(nExtra,nExtra);
		

			Bitmap[] bitmaps = new Bitmap[characters.Length];
			for (int i=0; i<characters.Length; i++)
			{
				string sChar = characters.Substring(i,1);
				SizeF size = gMeasuring.MeasureString(sChar, font, 1000, format);
				size.Width+=ptExtraSize.X*2+1;
				size.Height+=ptExtraSize.Y*2+1;

				Bitmap bmp = new Bitmap((int)size.Width,(int)size.Height, PixelFormat.Format32bppArgb);
				bitmaps[i] = bmp;

				Graphics g = Graphics.FromImage(bmp);
				g.SmoothingMode = SmoothingMode.HighQuality;

				if (font.Size < 10)
				{
					foreach (object oPenOrBrush in pensAndBrushes)
						if (oPenOrBrush.GetType() != typeof(Pen))
						{
							g.DrawString(sChar, font, (Brush)oPenOrBrush, new RectangleF(0,0,bmp.Width,bmp.Height), format);
							break;
						}
				}
				else
				{
					float pathSize = font.Size*1.3333333f; //font.Height
					GraphicsPath path = new GraphicsPath();
					path.AddString(sChar, font.FontFamily, (int)font.Style, pathSize, ptExtraSize, format);
					foreach (object oPenOrBrush in pensAndBrushes)
					{
						if (oPenOrBrush.GetType() == typeof(Pen))
							g.DrawPath((Pen)oPenOrBrush, path);
						else
							g.FillPath((Brush)oPenOrBrush, path);
					}
				}
			}
			return bitmaps;
		}

		#region Kerning related
		public static int[,] GetProfile(Bitmap bmp)
		{
			//BitmapHelpers.PixelManipulator pm = new Endogine.BitmapHelpers.PixelManipulator(bmp);
			int[,] profile = new int[bmp.Height,2];
			for (int y=0; y<bmp.Height; y++)
			{
				int x = 0;
				for (x=0; x<bmp.Width;x++)
				{
					//pm.GetPixel(x,y).A
					if (bmp.GetPixel(x,y).A > 0)
						break;
				}
				profile[y,0] = x;

				for (x=bmp.Width-1; x>=0;x--)
				{
					//pm.GetPixel(x,y).A
					if (bmp.GetPixel(x,y).A > 0)
						break;
				}
				profile[y,1] = bmp.Width-1-x;
			}
			//			//TODO: how come this is needed? the destructor should be called as soon as this method returns... right??
			//			if (pm!=null) pm.Dispose();

			return profile;
		}

		public static int CalcKerningBetween(Bitmap bmpA, Bitmap bmpB, int offsetYofB)
		{
			int[,] profileA = GetProfile(bmpA);
			int[,] profileB = GetProfile(bmpB);
			return CalcKerningBetween(profileA, profileB, offsetYofB);
		}
		public static int CalcKerningBetween(int[,] profileA, int[,] profileB, int offsetYofB)
		{
			int verticalBRelativeToAStart = offsetYofB;
			int verticalBRelativeToAEnd = offsetYofB + profileB.GetLength(0)-1;

			int commonYStart = Math.Max(0, verticalBRelativeToAStart);
			int commonYEnd = Math.Min(profileA.GetLength(0)-1, verticalBRelativeToAEnd);

			int kerningPixels = 99999;
			for (int y = commonYStart; y <= commonYEnd; y++)
			{
				//find rightmost pixel of A
				int rightMost = profileA[y,1];
				//and leftmost pixel of B on this line
				int leftMost = profileB[y-offsetYofB,0]; //bmpB.Width;
				int kernOnThisLine = leftMost  + rightMost;
				if (kernOnThisLine < kerningPixels)
					kerningPixels = kernOnThisLine;
			}
			return kerningPixels;
		}
		#endregion

		public List<EPointF> GenerateLocList(string text, int maxWidth, float kerningStrength, float lineSpacingFactor)
		{
            List<EPointF> locs = new List<EPointF>();

            //TODO: only works for certain charsets!!
			int bottomOfG = this['g'].SourceRectangle.Height-this['g'].Offset.Y; 
			int topOfAuml = -this['Å'].Offset.Y;
			int lineHeight = bottomOfG-topOfAuml;
			lineHeight=(int)(lineSpacingFactor*lineHeight);

			float extraSpace = 0;
			EPointF loc = new EPointF(0,0);
			for (int i=0; i<text.Length;i++)
			{
				PicRef p = this[text[i]];
                if (p == null)
                {
                    loc.X += 0.2f * lineHeight; //TODO: define space width!
                    locs.Add(loc.Copy());
                    loc.X += extraSpace;
                }
                else
                {
                    int kern = 0;
                    if (i > 0)
                        kern = this.GetKerningBetween(text[i - 1], text[i]);
                    loc.X -= kerningStrength * kern;
                    locs.Add(loc.Copy());
                    loc.X += (int)p.SourceRectangle.Width + extraSpace;
                }
				if (loc.X > maxWidth)
				{
					//TODO: go back to last whitespace or hypen and break there
					loc.Y+=lineHeight;
					loc.X = 0;
				}
			}
			return locs;
		}

		public static string GetCharSet(CharSet charSet, bool onlyUpperCase, bool onlyLowerCase)
		{
			string numbers = "";
			for (int i=48;i<=57;i++)
				numbers+=((char)i).ToString();

			string alphabet = ""; 
			for (int i=65;i<=90;i++)
				alphabet+=((char)i).ToString();

			//TODO: use System.Globalization somehow to retrieve alphabet?
			alphabet+="ÅÄÖ";

			if (!(onlyUpperCase || onlyLowerCase))
			{
				int numLetters = alphabet.Length;
				for (int i=0; i<numLetters; i++)
					alphabet+=alphabet[i].ToString().ToLower();
			}
			else if (onlyLowerCase)
				alphabet = alphabet.ToLower();

			switch (charSet)
			{
				case CharSet.Default:
					return alphabet + numbers + "!\"#%&/()=?@+´*'-_,.;:";
				case CharSet.Alphabet:
					return alphabet;
				case CharSet.AlphaNumeric:
					return alphabet + numbers;
				case CharSet.Numbers:
					return numbers;
			}
			return "";
		}

		public void UseStyleTemplate(string name)
		{
			FontGenerator.GetStyleTemplate(name, out this._pensAndBrushes, out this._font);
		}

		public static void GetStyleTemplate(string name, out List<object> pensAndBrushes, out Font font)
		{
            pensAndBrushes = new List<object>();
			font = null;

			switch (name)
			{
				case "Test1":
					font = new Font("Verdana", 36); // FontStyle.Bold | FontStyle.Italic); //Verdana Times New Roman
					pensAndBrushes.Add(new System.Drawing.Pen(System.Drawing.Color.FromArgb(127,255,255,0), 6));
					pensAndBrushes.Add(new System.Drawing.Pen(System.Drawing.Color.FromArgb(220,255,255,0), 4));
					int gradSize = (int)(0.85*font.Height);
					pensAndBrushes.Add(new LinearGradientBrush(new Rectangle(0,0,gradSize,gradSize), Color.FromArgb(255,0,0), Color.FromArgb(0,0,255), 45, false));
					pensAndBrushes.Add(new System.Drawing.Pen(System.Drawing.Color.FromArgb(255,255,255), 1.8f));
					break;
			}
		}
	}
}
