using System;
using System.Collections.Generic;

namespace Endogine.Forms
{
	/// <summary>
	/// Summary description for Label.
	/// </summary>
	public class Label : Sprite
	{
		private List<Sprite> _characterSprites;
		private Endogine.Text.FontGenerator _fg;
		private string _text;
		private float _kerningStrength = 1;
		private float _characterSpacing = 0;
		private float _lineSpacingFactor = 1;

		public Label()
		{
		}

		public Endogine.Text.FontGenerator FontGenerator
		{
			get {return this._fg;}
			set {this._fg = value;}
		}

		public float KerningStrength
		{
			get {return this._kerningStrength;}
			set {this._kerningStrength = value;}
		}

		public float CharacterSpacing
		{
			get {return this._characterSpacing;}
			set {this._characterSpacing = value;}
		}

		public float LineSpacingFactor
		{
			get {return this._lineSpacingFactor;}
			set {this._lineSpacingFactor = value;}
		}

        public List<Sprite> Sprites
		{
			get { return this._characterSprites; }
		}

		public string Text
		{
			get {return this._text;}
			set {this._text = value; this.CreateTextSprites(this._text);}
		}

		public void CreateTextSprites(string text)
		{
			if (this._characterSprites!=null)
			{
				//optimize: don't remove characters that are the same
				foreach (Sprite sp in this._characterSprites)
					sp.Dispose();
			}
            this._characterSprites = new List<Sprite>();
			List<EPointF> locs = this._fg.GenerateLocList(text, 200, this._kerningStrength, this._lineSpacingFactor);
			for (int i = 0; i<text.Length; i++)
			{
				Sprite sp = new Sprite();
				sp.PicRef = this._fg[text[i]];
				sp.Parent = this;
                sp.Loc = locs[i] + new EPointF(this._characterSpacing * i, 0);
				this._characterSprites.Add(sp);
			}
			this.Loc+=new EPointF(0,0); //seems to be needed... why?
		}

		public override int Blend
		{
			get {	return base.Blend;}
			set
			{
				foreach (Sprite sp in this._characterSprites)
					sp.Blend = value;
				base.Blend = value;
			}
		}
	}
}
