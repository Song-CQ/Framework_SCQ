using System;
using System.Collections;
using System.Windows.Forms;

namespace Endogine
{
	/// <summary>
	/// Summary description for StageBase.
	/// </summary>
	public abstract class StageBase
	{
        public delegate void RenderDelegate();
        public event RenderDelegate OnEndUpdate;

		protected Sprite _spRoot;
		protected Camera _cam = null;

		//protected EndogineHub m_endogine;
		protected Control _renderControl;
        protected System.Drawing.Color m_clr = System.Drawing.Color.Black;

		protected Sprite _defaultParent;
		protected bool m_bFullscreen = false;

		private static string _rendererPrefix = "Endogine.Renderer.";
		private static Hashtable _loadedRenderers;
        string _renderer;
        protected Endogine.ResourceManagement.Shaders _shaders;
        protected bool _clearTarget = true;

        public StageBase(Control a_renderControl) //, EndogineHub a_endogine
		{
			this._renderControl = a_renderControl;
			//m_endogine = a_endogine;
		}

		public virtual void Dispose()
		{}

		/// <summary>
		/// Size of the control that is being rendered to
		/// </summary>
		public EPoint ControlSize
		{
			get {return new EPoint(this._renderControl.ClientRectangle.Width, this._renderControl.ClientRectangle.Height);}
		}
		public EPoint Size
		{
			get {return _spRoot.SourceRect.Size;}
		}

		public virtual Control RenderControl
		{
			get {return this._renderControl;}
			set {this._renderControl = value;}
		}

        protected virtual void PreInit()
        {
        }

		public virtual void Init()
		{
            this.CreateRootSprite(new ERectangle(0, 0, this.ControlSize.X, this.ControlSize.Y));
		}

        public virtual void PreUpdate()
        { }
		public virtual void Update()
		{ }
        public virtual void EndUpdate()
        {
            if (this.OnEndUpdate!=null)
                this.OnEndUpdate();
        }
        public virtual void PostUpdate()
        { }
        public virtual Endogine.BitmapHelpers.Canvas Capture() //Endogine.BitmapHelpers.PixelDataProvider
        { return null; }


		public System.Drawing.Color Color
		{
			get {return m_clr;}
			set {m_clr = value;}
		}
		public bool Fullscreen
		{
			set {m_bFullscreen = value;}
			get {return this.m_bFullscreen;}
		}
        public bool ClearRenderTarget
        {
            get { return this._clearTarget; }
            set { this._clearTarget = value; }
        }

		protected void CreateRootSprite(ERectangle a_rct)
		{
			_spRoot = new Sprite();
			_spRoot.SourceRect = a_rct;
			_spRoot.Name = "root";

			_cam = new Camera();
			_cam.Name = "Camera";
			_cam.Parent = _spRoot;

			ParallaxLayer layer = new ParallaxLayer();
			layer.Name = "DefaultLayer";
			layer.Parent = (Sprite)_cam;
            
			this._defaultParent = layer;
		}

		public Sprite RootSprite
		{
			get {return _spRoot;}
		}

		/// <summary>
		/// (Cam only set the camera to a new object if it has no children)
		/// </summary>
		public Camera Camera
		{
			get {return _cam;}
			set
			{
				if (this._cam.ChildCount > 0)
					throw new Exception("Can only switch camera if it has no children!");
				this._cam = value;
			}
		}

		public Sprite DefaultParent
		{
			set
			{
				_defaultParent = value;
			}
			get
			{
				if (_defaultParent==null)
				{
					if (this.Camera != null)
					{
						ParallaxLayer layer = (ParallaxLayer)this.Camera.GetChildByName("DefaultLayer");
                        if (layer != null)
                            _defaultParent = (Sprite)layer;
                        else
                            throw new Exception("No default parent sprite set!");
					}
					else if (this.RootSprite != null)
						_defaultParent = this.RootSprite;
				}
				return _defaultParent;
			}
		}

        public static string[] GetAvailableRenderers(string directory)
		{
            if (directory == null)
                directory = System.IO.Directory.GetCurrentDirectory() +"\\"; // EH.Instance.ApplicationDirectory

            string search = directory + _rendererPrefix+"*.dll";
			System.IO.FileInfo[] files = Endogine.Files.FileFinder.GetFiles(search);
			string[] names = new string[files.Length];
			for (int i=0; i<files.Length; i++)
			{
				string name = files[i].Name;
				name = name.Remove(0,name.IndexOf(_rendererPrefix)+_rendererPrefix.Length);
				name = name.Remove(name.Length-4,4); //remove .dll
				names[i] = name;
			}
			return names;
		}


		public static System.Reflection.Assembly GetRendererAssembly(string name, string directory)
		{
			if (_loadedRenderers == null)
				_loadedRenderers = new Hashtable();

            if (directory == null)
                directory = EH.Instance.ApplicationDirectory;

			if (name==null)
			{
                string[] renderers = GetAvailableRenderers(directory);
				if (renderers.Length > 0)
					name = (string)renderers[0];
			}

            if (!_loadedRenderers.Contains(name))
         
            {
                string file = directory + _rendererPrefix + name + ".dll";
                if (!System.IO.File.Exists(file))
                    throw new Exception("Renderer not found: " + file);
                _loadedRenderers[name] = System.Reflection.Assembly.LoadFile(file);
            }

			return (System.Reflection.Assembly)_loadedRenderers[name];
		}

        public static StageBase CreateRenderer(string name, Control renderControl, string directory) //EndogineHub eh
		{
            System.Reflection.Assembly ass = GetRendererAssembly(name, directory);

			string sFullname = ass.FullName.Substring(0,ass.FullName.IndexOf(","));
			Type scriptClass = ass.GetType(sFullname+".Stage");
			if (scriptClass==null)
				throw new Exception("Stage class not found in renderer "+name);

			System.Reflection.ConstructorInfo cons = scriptClass.GetConstructor(new Type[]{typeof(Control)}); //, typeof(EndogineHub)
			object o = cons.Invoke(new object[]{renderControl}); //eh
            StageBase stage = (StageBase)o;
            stage._renderer = name;
            stage.PreInit();
            return stage;
		}

        public string Renderer
        {
            get { return this._renderer; }
        }

        public abstract Endogine.BitmapHelpers.PixelDataProvider CreateRenderTarget(int width, int height, int numChannels);

        /// <summary>
        /// Extremely ugly - needed because a RenderTarget obviously can't be readable. Ugly! An abomination! Only implemented for Direct3D renderer, others return the rendertarget.
        /// </summary>
        /// <returns></returns>
        public virtual Endogine.BitmapHelpers.PixelDataProvider UglyConvertRenderTargetToReadable(Endogine.BitmapHelpers.PixelDataProvider pdpTarget)
        {
            return this.RenderTarget;
        }

        public abstract Endogine.BitmapHelpers.PixelDataProvider RenderTarget
        { set; get; }

        public abstract SpriteRenderStrategy CreateRenderStrategy();

        public abstract Endogine.ResourceManagement.MemberSpriteBitmapRenderStrategy CreateMemberStrategy();

        /// <summary>
        /// Create a pixelDataProvider for the current rendering strategy (essentially - create a Texture, or a Bitmap) from the supplied Bitmap
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public abstract Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(System.Drawing.Bitmap bmp);
        public abstract Endogine.BitmapHelpers.PixelDataProvider CreatePixelDataProvider(int width, int height, int numChannels);

        public virtual Endogine.BitmapHelpers.PixelDataProvider TransformIntoRenderTarget(Endogine.BitmapHelpers.PixelDataProvider pdp)
        {
            return pdp;
        }

        public Endogine.ResourceManagement.Shaders Shaders
        {
            get { return this._shaders; }
        }
    }
}
