using System;
using System.Drawing;
using System.Data;
using System.Collections; //TODO: remove
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

using Endogine.ResourceManagement;

namespace Endogine
{

	[Serializable]
	/// <summary>
	/// Summary description for Sprite.
	/// </summary>
	public class Sprite
	{
		public delegate void MouseEventDelegate(Sprite sender, MouseEventArgs e, MouseEventType t);
		public delegate void EnterFrameEventDelegate();
		/// <summary>
		/// return false if an add should not be allowed
		/// </summary>
		public delegate bool ChildListChangedDelegate(object sender, Sprite child, bool wasAdded);

		public enum MouseEventType
		{
			Down, StillDown, Up, UpOutside, Click, Enter, Leave
		}
		public enum TextureFilters
		{
			None, Low, High
		}
	
		protected ERectangle m_rctSrcClip;

		protected ERectangleF m_rctDstParent;
		protected ERectangleF m_rctDstDraw;
		protected Sprite m_spParent = null;
		protected Sprite m_spDrawTarget = null;

		protected Hashtable m_plChildrenByHash;
		protected DataTable m_dtChildren; //TODO: hardly needed anymore - replace with Dictionary for name/sprite

		protected SortedFloatList _slChildrenLocZSorted;
		public event ChildListChangedDelegate ChildListChangedBefore;
		public event ChildListChangedDelegate ChildListChangedAfter;

		protected string m_sID;
		protected int m_nInk;
		protected int m_nBlend = 255;
		protected bool m_bVisible = true;
		protected Color m_clr;
		protected object m_tag;
		protected bool m_bMeInvisibleButNotChildren = false;

		protected EndogineHub m_endogine;

        //protected Animation.Animator m_autoAnimator = null;

		protected bool m_bMouseActive = false;
		protected bool m_bMouseDown = false;
		protected bool m_bMouseIsInside = false;
		protected bool m_bMousePassButton = false;
        protected bool m_bMouseReactToOtherThanLeftButton = false;
		public event MouseEventDelegate MouseEvent;
		protected EPoint m_pntMouseDown;
		protected EPoint m_pntMouseLast;
		protected EPoint _pntMouse;
		private bool _passEvent;

		//these are "derived" values - for user interaction and faster calculations.
		//m_rctDstParent is the result of loc, scale, srcCrop and regPoint

		//TODO: use matrix/pivot instead of loc/locZ/scale/rot/regpoint below
		protected Matrix4 m_matrix;
		protected Vector3 m_vPivot;

		protected float m_fLocZ;
		protected EPointF m_pntLoc;
		protected EPointF m_pntScale;
		protected bool m_bNoScalingOnSetRect = false; //only applicable to dummy sprites: if the children should not inherit scaling (what actually happens is that the sourcerect is set to correspond to the output rect
		protected EPoint m_pntRegPoint; //this value originally comes from the member, but can be set individually for each sprite.
		protected float m_fRotation;
		protected EPoint _regPointInternal = new EPoint(); //TODO: This should be the one that's exposed, and then we have an internal that is used when calculating final offset. Important when finishing the transition to PicRef.

		protected object m_cursor; //can be a windows cursor or a member

		protected MemberSpriteBitmap m_mb;
		protected PicRef _picRef;
		protected int m_nMemberAnimFrame;

		protected ArrayList m_aChildrenToRemove; //when removing a child during looping, they remain in lists until looping is done
		protected bool m_bDisposing = false;
		protected bool m_bLooping = false; //true when inside a loop that doesn't allow child sprites to be added/deleted. If so, they're added to a list which will be processed later
		protected SpriteRenderStrategy m_renderStrategy;

		protected ArrayList m_aBehaviors;
		public event EnterFrameEventDelegate EnterFrameEvent;

		protected TextureFilters _minFilter = TextureFilters.None;
		protected TextureFilters _magFilter = TextureFilters.None;

		public Sprite()
		{
			m_endogine = EndogineHub.Instance;
			Init(true);
		}

		public Sprite(bool bAutoSetParent)
		{
			m_endogine = EndogineHub.Instance;
			Init(bAutoSetParent);
		}

		public Sprite(EndogineHub a_endogine)
		{
			m_endogine = a_endogine;
			Init(true);
		}

		private void Init(bool bAutoSetParent)
		{
			m_aChildrenToRemove = new ArrayList();
			m_plChildrenByHash = new Hashtable();
			this._slChildrenLocZSorted = new SortedFloatList();

			m_sID = "";
			m_nInk = (int)RasterOps.ROPs.Copy;
			m_nBlend = 255;


			m_matrix = new Matrix4();
			m_vPivot = new Vector3();

			m_fLocZ = 0.0f;
			m_pntLoc = new EPointF(0.0f, 0.0f);
			m_pntScale = new EPointF(1.0f, 1.0f);
			m_pntRegPoint = new EPoint(0,0);


			m_rctSrcClip = new ERectangle(0,0,1,1);
			m_clr = Color.White;
			m_rctDstParent = new ERectangleF(0,0,0,0);

			m_aBehaviors = new ArrayList();

			m_nMemberAnimFrame = 0;

			m_dtChildren = new DataTable();
			m_dtChildren.Columns.Add("Name", typeof(System.String));
			m_dtChildren.Columns.Add("LocZ", typeof(System.Double));
			m_dtChildren.Columns.Add("Hash", typeof(System.Int32));

			m_pntMouseDown = new EPoint();
			m_pntMouseLast = new EPoint();
			_pntMouse = new EPoint();

            if (bAutoSetParent && m_endogine!=null && m_endogine.Stage != null)
				Parent = m_endogine.Stage.DefaultParent;

			m_renderStrategy = m_endogine.Stage.CreateRenderStrategy();
			
			m_renderStrategy.SetEndogine(m_endogine);
			m_renderStrategy.SetSprite(this);
			m_renderStrategy.Init();

			//TODO: make this optional (takes some resources)
			Sprite[] lcs = new Sprite[1];
			lcs[0] = this;
			EH.Instance.LatestCreatedSprites = lcs;
		}


		public virtual void Dispose()
		{
			m_bDisposing = true;
            //if (m_autoAnimator != null)
            //    m_autoAnimator.Dispose();
			if (m_mb != null)
				m_mb.RemoveSprite(this);
			if (m_spParent != null)
				m_spParent.RemoveChild(this);

			for (int i = m_aBehaviors.Count-1; i >= 0; i--)
			{
				Behavior bh = (Behavior)m_aBehaviors[i];
				bh.Dispose();
			}

            for (int i = this.ChildCount - 1; i >= 0; i--)
                this[i].Dispose();
            
			m_renderStrategy.Dispose();
		}

        public virtual Sprite Copy()
        {
            Sprite sp = new Sprite();
            this.CopyTo(sp);
            return sp;
        }
        public virtual void CopyTo(Sprite sp)
        {
            //TODO: use serialization instead!
            //sp.EndogineHub = this.EndogineHub;
            sp.Parent = this.Parent;
            sp.Blend = this.Blend;
            sp.Color = this.Color;
            sp.Cursor = this.Cursor;
            sp.DrawToSprite = this.DrawToSprite;
            sp.Ink = this.Ink;
            sp.Loc = this.Loc;
            sp.LocZ = this.LocZ;
            sp.MouseActive = this.MouseActive;
            sp.MousePassButton = this.MousePassButton;
            sp.Name = this.Name;
            sp.PassMouseEvent = this.PassMouseEvent;
            sp.PicRef = this.PicRef;
            sp.RegPoint = this.RegPoint;
            sp.Rotation = this.Rotation;
            sp.Scaling = this.Scaling;
            //TODO: some other stuff
            sp.Visible = this.Visible;
        }

		[Browsable(false)]
		public bool Disposing
		{
			get {return m_bDisposing;}
		}

		
		[Browsable(false)]
		public EndogineHub EndogineHub
		{
			get {return m_endogine;}
		}

		#region Mouse control
        [DefaultValue(false)]
        public bool MousePassButton
		{
			get {return m_bMousePassButton;}
			set {m_bMousePassButton = value;}
		}

		public object Cursor
		{
			get {return this.m_cursor;}
			set {this.m_cursor = value;}
		}

		public bool CheckMouse(MouseEventArgs e, EPointF a_pntLocalLoc, bool a_bButtonAction, bool a_bDown)
		{
			_pntMouse.X = e.X; //a_pntLocalLoc;
			_pntMouse.Y = e.Y;

            if (e.Button != MouseButtons.Left && this.m_bMouseReactToOtherThanLeftButton == false)
                return true;

			bool bReactedToButton = false;
			bool bReactedRoll = false;

			//TODO: make property of bMouseNotOutsideWhileDown
			bool bMouseNotOutsideWhileDown = true;

			if (m_bMouseDown && a_bDown && bMouseNotOutsideWhileDown)
			{
				OnMouse(e, MouseEventType.StillDown);
			}
			else
			{
				if (Rect.Contains(a_pntLocalLoc))
				{
					if (!m_bMouseIsInside)
					{
						//enter
						bReactedRoll = true;
						m_bMouseIsInside = true;
						OnMouse(e, MouseEventType.Enter);
					}
	
					if (a_bButtonAction)
					{
						if (a_bDown)
						{
							m_pntMouseDown.X = e.X;
							m_pntMouseDown.Y = e.Y;
							m_pntMouseLast = m_pntMouseDown.Copy();
							OnMouse(e, MouseEventType.Down);
							//EH.Put("Down:"+this.GetSceneGraphName());
						}
						else
						{
							if (m_bMouseDown)
								OnMouse(e, MouseEventType.Click);
							else
								OnMouse(e, MouseEventType.Up);
						}
						bReactedToButton = true;
					}
					else if (m_bMouseDown)
					{
						OnMouse(e, MouseEventType.StillDown);
						bReactedToButton = true;
					}
				}
				else
				{
					if (m_bMouseIsInside)
					{
						//leave
						bReactedRoll = true;
						m_bMouseIsInside = false;
						OnMouse(e, MouseEventType.Leave);
					}
					if (a_bButtonAction && !a_bDown && m_bMouseDown) //got mouseUp event, and mouse was down before
					{
						m_bMouseDown = false;
						OnMouse(e, MouseEventType.UpOutside);
					}
				}
			}
			m_pntMouseLast.X = e.X;
			m_pntMouseLast.Y = e.Y;

			if (bReactedToButton)
				m_bMouseDown = a_bDown;

			if (!this._passEvent)
			{
				//Can't stop checking completely: sprites under might need to send Leave, or UpOutside...
				if (bReactedToButton && !m_bMousePassButton)
					return false;
				if (m_bMouseIsInside && false) //TODO: pass?   bReactedRoll
					return false;
			}
			else
				this._passEvent = false;
			return true;
		}

		/// <summary>
		/// If set to true, this overrides the specific MousePassButton etc. Immediately reset to false after being used.
		/// </summary>
        [DefaultValue(false)]
        public bool PassMouseEvent
		{
			get {return this._passEvent;}
			set {this._passEvent = value;}
		}

		public void GetMouseCheckOrder(EPointF a_pntLocalLoc, ArrayList sprites)
		{
			a_pntLocalLoc = this.ConvParentLocToSrcLoc(a_pntLocalLoc);
			for (int i=this._slChildrenLocZSorted.Count-1; i>=0; i--)
			{
				Sprite sp = (Sprite)this._slChildrenLocZSorted.GetByIndex(i);
			//foreach (DataRowView row in m_dvChildrenLocZInverseSorted)
			//{
			//	Sprite sp = ((Sprite)m_plChildrenByHash[(int)row["Hash"]]);
				if (sp.ChildCount > 0)
					sp.GetMouseCheckOrder(a_pntLocalLoc, sprites);

				if (sp.MouseActive)
				{
					if (sp.Rect.Contains(a_pntLocalLoc))
						sprites.Add(sp);
				}
			}
		}

		public bool CheckChildrenMouse(MouseEventArgs e, EPointF a_pntLocalLoc, bool a_bButtonAction, bool a_bDown)
		{
			bool bContinueChecking = true;

			m_bLooping = true;
			a_pntLocalLoc = this.ConvParentLocToSrcLoc(a_pntLocalLoc);

			for (int i=this._slChildrenLocZSorted.Count-1; i>=0; i--)
			{
				Sprite sp = (Sprite)this._slChildrenLocZSorted.GetByIndex(i);
//			foreach (DataRowView row in m_dvChildrenLocZInverseSorted) //m_dvChildrenLocZInverseSorted m_dvChildrenLocZSorted
//			{
//				Sprite sp = ((Sprite)m_plChildrenByHash[(int)row["Hash"]]);
				if (sp.ChildCount > 0)
					bContinueChecking = sp.CheckChildrenMouse(e, a_pntLocalLoc, a_bButtonAction, a_bDown);

				if (bContinueChecking && sp.MouseActive)
					bContinueChecking = sp.CheckMouse(e,a_pntLocalLoc, a_bButtonAction, a_bDown);

				if (!bContinueChecking)
					break;
			}

			m_bLooping = false;

			return bContinueChecking;
		}

		protected virtual void OnMouse(MouseEventArgs e, MouseEventType t)
		{
			if (MouseEvent!=null)
				MouseEvent(this,e,t);
		}
		[Browsable(false)]
		public EPoint MouseLastLoc
		{
			get {return m_pntMouseLast;}
		}
		[Browsable(false)]
		public EPoint MouseDownLoc
		{
			get {return m_pntMouseDown;}
		}
		[Browsable(false)]
		public EPoint MouseLoc
		{
			get {return _pntMouse;}
		}
		

		public void GetMouseActiveOffspringZSorted(ref SortedList a_plSprites, ref int a_nOrderCount)
		{
			foreach (DictionaryEntry de in this._slChildrenLocZSorted)
			{
				Sprite sp = (Sprite)de.Value;
//			foreach (DataRowView row in m_dvChildrenLocZSorted)
//			{
//				Sprite sp = ((Sprite)m_plChildrenByHash[(int)row["Hash"]]);
	
				if (sp.MouseActive)
					a_plSprites.Add(a_nOrderCount, sp);

				a_nOrderCount++;
				if (sp.ChildCount > 0)
				{
					sp.GetMouseActiveOffspringZSorted(ref a_plSprites, ref a_nOrderCount);
				}
			}
		}
		public void GetVisibleOffspringZSorted(ref SortedList a_plSprites, ref int a_nOrderCount)
		{
			foreach (DictionaryEntry de in this._slChildrenLocZSorted)
			{
				Sprite sp = (Sprite)de.Value;
//			foreach (DataRowView row in m_dvChildrenLocZSorted)
//			{
//				Sprite sp = ((Sprite)m_plChildrenByHash[(int)row["Hash"]]);

				if (sp.Visible && !sp.m_bMeInvisibleButNotChildren)
					a_plSprites.Add(a_nOrderCount, sp);

				a_nOrderCount++;
				if (sp.ChildCount > 0)
				{
					sp.GetVisibleOffspringZSorted(ref a_plSprites, ref a_nOrderCount);
				}
			}
		}

		[DefaultValue(false)]
		public bool MouseActive
		{
			get
			{
				return m_bMouseActive;
				//if (m_spMouse == null) return false;
				//return m_spMouse.Active;
			}
			set 
			{
				m_bMouseActive = value;
				/*if (m_spMouse == null)
				{
					if (value == true) ConfigMouse();
					else return;
				}
				else
					m_spMouse.Active = value;*/
			}
		}
		#endregion

		public TextureFilters TextureMinFilter
		{
			get {return this._minFilter;}
		}
		public TextureFilters TextureMagFilter
		{
			get {return this._magFilter;}
		}

		public Sprite.TextureFilters TextureFilter
		{
			get {	return this._magFilter;}
			set
			{
				this._magFilter = value;
				this._minFilter = value;
			}
		}
		public string GetSceneGraphName()
		{
			string sText = Name +  "/";
			if (Member != null)
				sText+=Member.Name;
			sText+= ":"+this.ToString().Replace("Endogine", ""); //remove "Endogine" from class name, it's understood...
			return sText;
		}

		public object Tag
		{
			get {return m_tag;}
			set {m_tag = value;}
		}
		public String Name
		{
			get {return m_sID;}
			set 
			{
				m_sID = value;
				if (m_spParent != null)
					m_spParent.UpdateChildTableForChild(this);
			}
		}

		public void CenterRegPoint()
		{
			if (Member!=null)
				this.RegPoint = Member.Size/2;
		}

		public ArrayList GetSpritesUnderLoc(EPointF a_pnt, int a_nMaxNumSprites)
		{
			ArrayList aSprites = new ArrayList();
			SortedList plSprites = new SortedList();
			int n = 0;
			GetVisibleOffspringZSorted(ref plSprites, ref n);
			for (int i = plSprites.Count-1; i >= 0; i--)
			{
				Sprite sp = (Sprite)plSprites.GetByIndex(i);
				EPointF pntSprite = sp.ConvRootLocToParentLoc(a_pnt);
				if (sp.Rect == null)
					continue;

				if (sp.Rect.Contains(pntSprite))
				{
					aSprites.Add(sp);
					if (aSprites.Count == a_nMaxNumSprites)
						break;
				}
			}
			return aSprites;
		}

		public ERectangleF GetPortionOfMemberToDisplay()
		{
			//TODO: doesn't care about SrcCrop!
			ERectangleF rctfCropped;

            EPoint size = Member.Size; // Member.TotalSize;
			EPoint sizeFrame = Member.Size;
			EPoint sizeReal = size;//GetUpperPowerTextureSize(size);

            //if (Member.GotAnimation)
            //{
            //    ERectangle rctAnim = SourceRect;

            //    rctfCropped = new ERectangleF((float)rctAnim.X/sizeReal.X, (float)rctAnim.Y/sizeReal.Y, 
            //        (float)rctAnim.Width/sizeReal.X, (float)rctAnim.Height/sizeReal.Y);
            //}
            //else
            //{
				ERectangle rctAnim = SourceRect;
				rctfCropped = new ERectangleF((float)rctAnim.X/sizeReal.X, (float)rctAnim.Y/sizeReal.Y, 
					(float)rctAnim.Width/sizeReal.X, (float)rctAnim.Height/sizeReal.Y);
            //}

			return rctfCropped;
		}

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ERectangleF Rect
		{
			get {return m_rctDstParent;}
			set
			{
				if (m_bNoScalingOnSetRect)
				{
					m_rctSrcClip = new ERectangle(0,0,(int)value.Width,(int)value.Height);
				}
				else
				{
					m_pntScale.X = value.Width/m_rctSrcClip.Width;
					m_pntScale.Y = value.Height/m_rctSrcClip.Height;
				}

				m_pntLoc.X = value.Left + m_pntScale.X*m_pntRegPoint.X;
				m_pntLoc.Y = value.Top + m_pntScale.Y*m_pntRegPoint.Y;

				m_rctDstParent = value;
			}
		}

		public ERectangle SourceRect
		{
			get
			{
                //if (Member!=null && Member.GotAnimation)
                //{
                //    ERectangle rctAnim = Member.GetRectForFrame(m_nMemberAnimFrame);
                //    ERectangle rct = m_rctSrcClip.Copy();
                //    rct.Offset(rctAnim.Location);
                //    return rct;
                //}
				return m_rctSrcClip;
			}
			set
			{
				m_rctSrcClip = value;
				CalcInParent();
			}
		}

		[Category("Appearance")]
		public virtual RasterOps.ROPs Ink
		{
			get {return (RasterOps.ROPs)m_nInk;}
			set {m_nInk = (int)value;}
		}
		[Category("Appearance"), DefaultValue(255)]
		public virtual int Blend
		{
			get {return m_nBlend;}
			set
			{
				m_nBlend = Math.Min(255,Math.Max(0,value));
				this.Color = Color.FromArgb(m_nBlend, this.m_clr.R, this.m_clr.G, this.m_clr.B);
			}
		}
		[Category("Appearance"), DefaultValue(true)]
		public bool Visible
		{
			get {return m_bVisible;}
			set {m_bVisible = value;}
		}

        //TODO: DefaultValue(Color.White) doesn't work...
		[Category("Appearance")]
		public virtual Color Color
		{
			get {return m_clr;}
			set {m_clr = value; m_renderStrategy.SetColor(m_clr);}
		}

		[Category("Appearance"), DefaultValue(0)]
		public int MemberAnimationFrame
		{
			get {return m_nMemberAnimFrame;}
			set {
				if (value < 0)
					m_nMemberAnimFrame = -value;
				else
					m_nMemberAnimFrame = value;
				this.CalcInParent();
				m_renderStrategy.SetMemberAnimationFrame(m_nMemberAnimFrame);
			}
		}

        //[Browsable(false),
        //DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public Animation.Animator AutoAnimator
        //{	get {return m_autoAnimator;} }

        //public Animation.Animator CreateAnimator(string a_sProp)
        //{
        //    return new Animation.Animator(this, a_sProp);
        //    //return new Animator(m_endogine, a_sProp, new AnimateEvent(SetOneProp));
        //}

		[Category("Appearance")]
		public string MemberName
		{
			set 
			{
				Member = (MemberSpriteBitmap)m_endogine.CastLib.GetOrCreate(value);
			}
			get {return Member.Name;}
		}

		public string PicRefName
		{
			set
            {
                PicRef p = Endogine.PicRef.Get(value);
                if (this.PicRef!=p)
                    this.PicRef = Endogine.PicRef.Get(value);
            }
            get { if (this.PicRef != null) return this.PicRef.OriginalName; return null; }
		}

        //public Endogine.BitmapHelpers.PixelManipulatorBase PixelManipulator
        //{
        //    get {return this.Member.PixelManipulator;}
        //    set {this.Member.PixelManipulator = null;}
        //}
        //public Endogine.BitmapHelpers.Canvas Canvas
        //{
        //    get {return this.Member.C
        //}

        /// <summary>
        /// Look for a resource in this order: Animation, FrameSet, PicRef, Member.
        /// </summary>
        /// <param name="name"></param>
        public void SetGraphics(string name)
        {
            //EH.Instance.CastLib.Animations[name];

            Endogine.Animation.BhAnimator an = (Endogine.Animation.BhAnimator)this["_anim"];

            List<string> fs = EH.Instance.CastLib.FrameSets[name];
            if (fs != null)
            {
                if (an != null)
                    an.FrameSet = name;
                else
                    an = new Endogine.Animation.BhAnimator(this, name);
                return;
            }

            PicRef pr = PicRef.Get(name);
            if (pr!=null)
            {
                 if (an != null)
                     an.Dispose();
                 this.PicRef = pr;
                 return;
            }

            MemberBase mb = EH.Instance.CastLib.GetByName(name);
            if (mb != null && mb is MemberSpriteBitmap)
            {
                this.Member = (MemberSpriteBitmap)mb;
                return;
            }

            this.MemberName = name;
        }

        public Endogine.Animation.BhAnimator Animator
        {
            get { return (Endogine.Animation.BhAnimator)this["_anim"]; }
        }

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PicRef PicRef
		{
			get {return _picRef;}
			set
			{
				if (value==null)
				{
					this.Member = null;
					if (this._picRef!=null)
						_picRef.Changed-=new Endogine.PicRef.ChangedDelegate(_picRef_Changed);
					_picRef = null;
				}
				else
				{
                    if (this.m_mb!=value.Member)
                        this.Member = value.Member; //TODO: something's very wrong here - repeatedly changing member slows the system down

					this._picRef = value;
					
					this._picRef_Changed();

					//TODO: never create autoanimator in Member!
                    //if (m_autoAnimator != null)
                    //{
                    //    m_autoAnimator.Dispose();
                    //    m_autoAnimator = null;
                    //}
				}
			}
		}
		private void _picRef_Changed()
		{
			ERectangle rct = _picRef.SourceRectangle;
            this._regPointInternal = rct.TopLeft + _picRef.Offset;
			//this.RegPoint = rct.TopLeft + _picRef.Offset + this._regPointInternal;
			this.SourceRect = rct;
		}


		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual MemberSpriteBitmap Member
		{
			get {return m_mb;}
			set
			{
                //can't exit if m_mb == value - things might have changed in the member (size etc)
				if (m_mb != null)
					m_mb.RemoveSprite(this);
//				if (value == null)
//					throw(new Exception("No member!"));

				_picRef = null;

                //if (m_autoAnimator != null)
                //{
                //    m_autoAnimator.Dispose();
                //    m_autoAnimator = null;
                //}

				m_mb = value;
				if (m_mb == null)
				{

				}
				else
				{
					//m_pntRegPoint = m_mb.RegPoint;
					m_rctSrcClip = new ERectangle(new EPoint(0,0), m_mb.Size);
					CalcInParent();

                    //if (value.AutoAnimate)
                    //{
                    //    m_autoAnimator = this.CreateAnimator("MemberAnimationFrame");
                    //    //m_autoAnimator = new Animator(m_endogine, "MemberAnimationFrame", new AnimateEvent(SetOneProp));
                    //}

					m_renderStrategy.SetMember(m_mb);
					m_mb.AddSprite(this);
				}
			}
		}

		#region Move/Scale/Rotate methods
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual EPointF Loc
		{
			get {return m_pntLoc;}
			set
			{
				m_pntLoc = value;
				this.m_matrix.M41=value.X;
				this.m_matrix.M42=value.Y;
				CalcInParent();
			}
		}
		[Category("Loc/Scale/Rot"), DefaultValue(0)]
		public virtual float LocX
		{
			get {return m_pntLoc.X;}
			set {Move(new EPointF(value-m_pntLoc.X, 0));}//m_pntLoc.X = value; CalcInParent();}
		}
		[Category("Loc/Scale/Rot"), DefaultValue(0)]
		public virtual float LocY
		{
			get {return m_pntLoc.Y;}
			set {Move(new EPointF(0, value-m_pntLoc.Y));}//m_pntLoc.Y = value; CalcInParent();}
		}
		public virtual void Move(EPointF a_pnt)
		{
			this.Loc = new EPointF(a_pnt.X+this.Loc.X, a_pnt.Y+this.Loc.Y);
		}
		public virtual void Move(float a_fX, float a_fY)
		{
			this.Loc = new EPointF(a_fX+this.Loc.X, a_fY+this.Loc.Y);
		}
		[Category("Loc/Scale/Rot"), DefaultValue(0)]
		public float LocZ
		{
			get {return m_fLocZ;}
			set
			{
				this.m_matrix.M43=value;
				m_fLocZ = value;
				if (m_spParent != null)
					m_spParent.UpdateChildTableForChild(this);
			}
		}
		[Category("Loc/Scale/Rot")]
		public EPoint RegPoint
		{
			get {return m_pntRegPoint;}
			set
			{
				m_pntRegPoint = value;
				this.m_vPivot.X = value.X;
				this.m_vPivot.Y = value.Y;
				CalcInParent();
			}
		}

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EPointF Scaling
		{
			get {return m_pntScale;}
			set
			{
				m_pntScale = value;
				this.m_matrix.M11 = value.X;
				this.m_matrix.M22 = value.Y;
				CalcInParent();
			}
		}

		[Category("Loc/Scale/Rot"), DefaultValue(1f)]
		public float ScaleX
		{
			get {return m_pntScale.X;}
			set
			{
				m_pntScale.X = value;
				this.m_matrix.M11 = value;
				CalcInParent();
			}
		}
		[Category("Loc/Scale/Rot"), DefaultValue(1f)]
		public float ScaleY
		{
			get {return m_pntScale.Y;}
			set
			{
				m_pntScale.Y = value;
				this.m_matrix.M22 = value;
				CalcInParent();
			}
		}
		[Category("Loc/Scale/Rot"), DefaultValue(0f)]
		public float Rotation
		{
			get {return m_fRotation;}
			set
			{
				m_fRotation = value;
				//this.m_matrix.RotateZ(value);
			}
		}
		#endregion


		#region Output rect/quad calculation methods
		public virtual void CalcInParent()
		{
			m_rctDstParent = m_rctSrcClip.ToERectangleF();

			m_rctDstParent.X *= m_pntScale.X;
			m_rctDstParent.Y *= m_pntScale.Y;
			m_rctDstParent.Width *= m_pntScale.X;
			m_rctDstParent.Height *= m_pntScale.Y;

			//TODO: use sprite pntLoc instead of member's! (hm, did I mean regPoint instead of pntLoc??)
			//if (Member != null)

			m_rctDstParent.Offset(m_pntLoc.X - (m_pntRegPoint.X+this._regPointInternal.X)*m_pntScale.X, m_pntLoc.Y - (m_pntRegPoint.Y+this._regPointInternal.Y)*m_pntScale.Y);
			//else
			//	m_rctDstParent.Offset(m_pntLoc);

            //if (Member != null && Member.GotAnimation)
            //    m_rctDstParent.Offset(Member.GetOffsetForFrame(this.MemberAnimationFrame).ToEPointF()*-1);

			m_renderStrategy.RecalcedParentOutput();
		}

		public ERectangleF CalcRectInDrawTarget()
		{
//			ERectangleF rctTmp = m_rctDstParent.Copy();
//			if (Member != null && Member.GotAnimation)
//				rctTmp.Offset(Member.GetOffsetForFrame(this.MemberAnimationFrame).ToEPointF()*-1);

			//TODO: check if anything was invalidated since last frame - if not, use a cached value instead of re-calculating.
			EPointF pnt1 = ConvParentLocToDrawLoc(m_rctDstParent.Location, m_spDrawTarget);
			
			EPointF pnt2 = ConvParentLocToDrawLoc(m_rctDstParent.BottomRight, m_spDrawTarget);
			return new ERectangleF(pnt1.X, pnt1.Y, pnt2.X-pnt1.X, pnt2.Y-pnt1.Y);
		}
		#endregion


		#region Loc conversion methods (between source/parent/drawTarget/root)
		//ParentLoc = point relative to the output rect (Rect) - source loc after move/scale/rotate
		public EPointF ConvSrcLocToParentLoc(EPointF a_pntLoc)
		{
			//if (Member==null)
			//	return a_pntLoc;
//			PointF pntFract = new PointF(a_pntLoc.X/m_mb.Size.Width, a_pntLoc.Y/m_mb.Size.Height);
			EPointF pntFract = new EPointF(a_pntLoc.X/SourceRect.Width, a_pntLoc.Y/SourceRect.Height);
			pntFract.X = pntFract.X*m_rctDstParent.Width + m_rctDstParent.Left;
			pntFract.Y = pntFract.Y*m_rctDstParent.Height + m_rctDstParent.Top;
			return pntFract;
		}

		public EPointF ConvParentLocToDrawLoc(EPointF a_pntLoc, Sprite a_spDraw)
		{
			if (m_spParent == null || (m_spParent!=null && m_spParent == a_spDraw))
				return a_pntLoc;

			a_pntLoc = m_spParent.ConvSrcLocToParentLoc(a_pntLoc);
			return m_spParent.ConvParentLocToDrawLoc(a_pntLoc, a_spDraw);
		}

		public EPointF ConvParentLocToRootLoc(EPointF a_pntLoc)
		{
			if (m_spParent == null)
				return a_pntLoc;

			return m_spParent.ConvParentLocToRootLoc(m_spParent.ConvSrcLocToParentLoc(a_pntLoc));
		}

		public EPointF MapPointFromRectAToRectB(EPointF a_pnt, ERectangleF a_rctA, ERectangleF a_rctB)
		{
			EPointF pntNew = new EPointF(a_pnt.X-a_rctA.Left, a_pnt.Y-a_rctA.Top);
			pntNew.X/=a_rctA.Width;
			pntNew.Y/=a_rctA.Height;

			pntNew.X = pntNew.X*a_rctB.Width + a_rctB.Left;
			pntNew.Y = pntNew.Y*a_rctB.Height + a_rctB.Top;
			return pntNew;
		}

		public EPointF ConvParentLocToSrcLoc(EPointF a_pntLoc)
		{
			return MapPointFromRectAToRectB(a_pntLoc, Rect, this.SourceRect.ToERectangleF());
		}

		public EPointF ConvRootLocToSrcLoc(EPointF a_pnt)
		{
			//convert a loc in root (normally stage) space to source space.
			//One calculation per link can make it slow in deep trees.
			if (Parent != null)
				a_pnt = Parent.ConvRootLocToSrcLoc(a_pnt);
			return ConvParentLocToSrcLoc(a_pnt);
		}
		public EPointF ConvRootLocToParentLoc(EPointF a_pnt)
		{
			if (Parent != null)
				a_pnt = Parent.ConvRootLocToSrcLoc(a_pnt);
			return a_pnt;
		}
		#endregion


		#region Behavior stuff
		//TODO: I want to move these methods out to a BehaviorCollection, but I have to read about creating such classes first.
		//This is true for many collections throughout the project. Important: the owner of such a collection must be notified about adds/removes
		public void AddBehavior(Behavior bh)
		{
			m_aBehaviors.Add(bh);
			if (bh.Sprite != this)
				bh.Sprite = this;
		}
		public void RemoveBehavior(Behavior bh)
		{
			m_aBehaviors.Remove(bh);
			//TODO if (.Disposed == false), dispose it!
		}
		public bool HasBehavior(Behavior bh)
		{
			return m_aBehaviors.Contains(bh);
		}
		public int GetNumBehaviors()
		{
			return m_aBehaviors.Count;
		}
		public Behavior GetBehaviorByIndex(int i)
		{
			return (Behavior)m_aBehaviors[i];
		}
		#endregion

		public virtual void EnterFrame()
		{
			if (EnterFrameEvent!=null)
				EnterFrameEvent();

			m_renderStrategy.EnterFrame();

			m_bLooping = true;

			Sprite[] sprites = new Sprite[this._slChildrenLocZSorted.Count];
			for (int i=sprites.Length-1;i>=0;i--)
			{
				Sprite sp = (Sprite)this._slChildrenLocZSorted.GetByIndex(i);
				sprites[i] = sp;
			}
			foreach (Sprite sp in sprites)
			{
				if (sp.Disposing)
					continue;
				sp.EnterFrame();
			}
			m_bLooping = false;

			foreach (Sprite sp in m_aChildrenToRemove)
				RemoveChild(sp);
			m_aChildrenToRemove.Clear();
		}
		public virtual void Draw()
		{
			if (!Visible)
				return;

			if ((Name != "root" && Member!=null) && m_bMeInvisibleButNotChildren == false) // && Visible
				m_renderStrategy.SubDraw();

			int nCnt = this._slChildrenLocZSorted.Count;
			for (int i=0; i<nCnt; i++)
			{
				Sprite sp = (Sprite)this._slChildrenLocZSorted.GetByIndex(i);
				sp.Draw();
			}
		}

		#region Parent/child relation methods
		/// <summary>
		/// Draw Targets only work in GDI mode - in 3D, the target is always the frame buffer. TODO: RenderToTexture
		/// </summary>
		public Sprite DrawToSprite
		{
			set {m_spDrawTarget = value; if (m_spParent == null) m_spParent = value;}
			get {return m_spDrawTarget;}
		}

		[Category("Parent/Child"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		ReadOnly(true)]
		public virtual Sprite Parent
		{
			get {return m_spParent;}
			set
			{
				if (m_spParent != null)
					m_spParent.RemoveChild(this);

				if (value != null)
				{
					//it's not certain that the suggested parent allows me to be added as a child:
					if (value.AddChild(this))
						m_spParent = value;
				}
			}
		}
		public bool AddChild(Sprite a_sp)
		{
			if (ChildListChangedBefore!=null)
			{
				if (ChildListChangedBefore(this, a_sp, true) == false)
					return false;
			}

			//TODO: I need a sorted list class that allows duplicate keys! DataTables are slooow
			DataRow row = m_dtChildren.NewRow();
			row["Name"] = a_sp.Name;
			row["LocZ"] = a_sp.LocZ;
			row["Hash"] = a_sp.GetHashCode();
			m_dtChildren.Rows.Add(row);

			this._slChildrenLocZSorted.Add(a_sp.LocZ, a_sp);

			m_plChildrenByHash[a_sp.GetHashCode()] = a_sp;

			if (ChildListChangedAfter!=null)
				ChildListChangedAfter(this, a_sp, true);

			return true;
		}
		public virtual void RemoveChild(Sprite a_sp)
		{
			if (m_bLooping)
			{
				m_aChildrenToRemove.Add(a_sp);
				return;
			}
			if (ChildListChangedBefore!=null)
				ChildListChangedBefore(this, a_sp, false);

			//DataRow[] rows = m_dtChildren.Select("Name = '"+a_sp.Name+"'");
			DataRow[] rows = m_dtChildren.Select("Hash = '"+a_sp.GetHashCode()+"'");
			if (rows.GetLength(0) > 0)
				m_dtChildren.Rows.Remove(rows[0]);

			int index = this._slChildrenLocZSorted.IndexOfValue(a_sp);
			if (index >= 0) //TODO: how can it not be..?
				this._slChildrenLocZSorted.RemoveAt(index);
			m_plChildrenByHash.Remove(a_sp.GetHashCode());

			if (ChildListChangedAfter!=null)
				ChildListChangedAfter(this, a_sp, false);
		}
		protected void UpdateChildTableForChild(Sprite a_sp)
		{
			//Use when changing any one of the properties in the sprite that are used in m_dtChildren
			int index = this._slChildrenLocZSorted.IndexOfValue(a_sp);
			if (index >= 0)
			{
				this._slChildrenLocZSorted.RemoveAt(index);
				this._slChildrenLocZSorted.Add(a_sp.LocZ, a_sp);
			}
			DataRow[] rows = m_dtChildren.Select("Hash = '"+a_sp.GetHashCode()+"'");
			if (rows.GetLength(0) > 0)
			{
				rows[0]["Name"] = a_sp.Name;
				rows[0]["LocZ"] = a_sp.LocZ;
			}
		}

		[Category("Parent/Child")]
		public int ChildCount
		{
			get {return this._slChildrenLocZSorted.Count;}
		}

        public Sprite this[string name]
        {
            get { return this.GetChildByName(name); }
        }
        public Sprite this[int index]
        {
            get { return this.GetChildByIndex(index); }
        }

		public Sprite GetChildByName(string a_sName)
		{
			DataRow[] rows = m_dtChildren.Select("Name = '"+a_sName+"'");
			if (rows.GetLength(0) > 0)
			{
				int hash = (int)rows[0]["Hash"];
				return (Sprite)m_plChildrenByHash[hash];
			}
			return (Sprite)null;
		}
		public Sprite GetChildByIndex(int a_nIndex)
		{
			return (Sprite)this._slChildrenLocZSorted.GetByIndex(a_nIndex);
		}
		public void GetChildHierarchyDebug(ref string sOutput, int nDepth)
		{
			string sIndent = "";
			nDepth++;
			for (int j = 0; j<nDepth; j++)
				sIndent+=" ";

			for (int i = 0; i < ChildCount; i++)
			{
				Sprite sp = GetChildByIndex(i);
				sOutput+=sIndent+sp.Name+"\r\n";
				sp.GetChildHierarchyDebug(ref sOutput, nDepth);
			}
		}
		public Sprite GetChildByHashCode(int a_hashCode)
		{
			Sprite sp = (Sprite)m_plChildrenByHash[a_hashCode];
			return sp;
		}
		#endregion

		public EPointF GetCollisionPoint(Sprite spToCheck)
		{
			//TODO: check per pixel!
			//TODO: now we're assuming they belong to the same coordinate space (faster).
			//There should be an alternative to check both rects in root space.
			if (spToCheck.Rect.IntersectsWith(Rect))
			{
				return this.Loc.Copy();
			}
			return (EPointF)null;
		}

		public ERectangleF GetBoundingRectangle()
		{
			return this.GetBoundingRectangle(true, false, true);
		}
		/// <summary>
		/// Get the rectangle that completely encompasses all child sprites (and optionally their children)
		/// </summary>
		/// <returns></returns>
		public ERectangleF GetBoundingRectangle(bool includeChildrensBounds, bool onlyThoseWithMembers, bool includeThis)
		{
			ERectangleF rct = ERectangleF.FromLTRB(9999999,9999999,-9999999,-9999999);
			if (includeThis)
				if ((onlyThoseWithMembers && this.Member!=null) || !onlyThoseWithMembers)
					rct.Expand(this.Rect);

			for (int i = this.ChildCount-1; i>=0; i--)
			{
				Sprite sp = this.GetChildByIndex(i);

				if ((onlyThoseWithMembers && this.Member!=null) || !onlyThoseWithMembers)
					rct.Expand(sp.Rect);

				if (includeChildrensBounds)
					rct.Expand(sp.GetBoundingRectangle(true, onlyThoseWithMembers, false));
			}
			return rct;
		}
	}
}