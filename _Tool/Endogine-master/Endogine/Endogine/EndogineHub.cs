using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

using Endogine.ResourceManagement;
using Endogine.Editors;

namespace Endogine
{
    public delegate void EnterFrame();

    public delegate void MouseButtonEventHandler(object sender, MouseEventArgs e, bool bDown);
    public delegate void MouseScrollEventHandler(object sender, MouseEventArgs e);
    public delegate void KeyEventHandler(System.Windows.Forms.KeyEventArgs e, bool bDown);

    /// <summary>
    /// Abbreviation of EndogineHub (since it's used so much, it's boring to type)
    /// </summary>
    public class EH
    {
        public static EndogineHub Instance
        {
            get { return EndogineHub.Instance; }
        }
        public static void Put(string s)
        {
            EndogineHub.Put(s);
        }
    }
    /// <summary>
    /// Summary description for Endogine.
    /// </summary>
    public class EndogineHub
    {
        public event EnterFrame EnterFrameEvent;
        public event MouseButtonEventHandler MouseButtonEvent;
        public event MouseScrollEventHandler MouseScrollEvent;
        public event KeyEventHandler KeyEvent;

        private string _renderStrategy;

        private StageBase _stage;

        private CastLib m_castlib = null;

        private Audio.SoundManager _soundManager = null;

        private EPoint m_pntMouseLoc;
        private bool m_bMouseDown;
        private bool m_bSendMouseEventsToSprites = true;
        private ArrayList m_aSpritesInContextMenu;

        //private int m_nLastTick;

        private string m_sApplicationPath;

        public static Endogine.Editors.IMessageWindow m_msgWnd; //TODO: why did I make this static??
        public Endogine.Editors.ISceneGraphViewer m_sceneGraphViewer;
        public Endogine.Editors.ILocScaleRotEdit CamControl;

        private RasterOps m_rasterOps;

        private static EndogineHub m_endogine;

        public bool m_bFullscreen = false;
        private Form m_formMdiParent;

        private Sprite[] m_latestCreatedSprites = null;


        private Form _startupForm; //TODO: shouldn't have such a thing, console startup rather...



        public EndogineHub(string a_sApplicationPath)
        {
            m_sApplicationPath = a_sApplicationPath;

            m_pntMouseLoc = new EPoint();
            m_endogine = this;

            AppSettings app = new AppSettings();

            string s = AppSettings.Instance.GetNodeText("DisplayMode.Fullscreen");
            if (s != null)
                m_bFullscreen = Convert.ToBoolean(s);

            s = AppSettings.Instance.GetNodeText("DisplayMode.Renderer");
            if (s != null)
                this._renderStrategy = s;
        }

        public void Init(Control a_mainControl, Form a_formMdiParent, Form startupForm)
        {
            m_formMdiParent = a_formMdiParent;
            this._startupForm = startupForm;

            string[] renderers = StageBase.GetAvailableRenderers(null);

            this._stage = StageBase.CreateRenderer(this._renderStrategy, a_mainControl, null); //renderers[0] //GDI Direct3D OpenGL

            Stage.Fullscreen = m_bFullscreen;
            Stage.Init();

            if (this._renderStrategy == "GDI")
            {
                //Precalculate some raster operations (look-up tables)
                m_rasterOps = new RasterOps();
                m_rasterOps.PreCalcBlendMode(RasterOps.ROPs.AddPin);
                m_rasterOps.PreCalcBlendMode(RasterOps.ROPs.Lightest);
                m_rasterOps.PreCalcBlendMode(RasterOps.ROPs.Difference);
            }

            string sMediaPath = null;
            //TODO: ugly way of getting path - do something about it in AppSettings.Instance
            if (AppSettings.Instance.GetPath("Media") != null)
                sMediaPath = AppSettings.Instance.GetPath("Media");
            if (sMediaPath == null)
                sMediaPath = AppSettings.BaseDirectory + "\\Media";
            m_castlib = new CastLib(this);
            m_castlib.Init(sMediaPath);

            try
            {
                this._soundManager = Audio.SoundManager.CreateSystem(null, a_mainControl); //DirectX Bass
                if (this._soundManager != null)
                {
                    this._soundManager.DefaultSoundPath = null;
                    if (AppSettings.Instance.GetPath("Sound") != null)
                        this._soundManager.DefaultSoundPath = AppSettings.Instance.GetPath("Sound");
                    if (this._soundManager.DefaultSoundPath == null)
                        this._soundManager.DefaultSoundPath = this.CastLib.DirectoryPath; //System.IO.Directory.GetCurrentDirectory;
                }
            }
            catch { }

            if (AppSettings.Instance.GetNodeText("ShowEditors") != "false")
            {
                //TODO: let user decide if/which GUI should be used!
                if (EditorFactory.LoadDll("Endogine.Editors.dll"))
                {
                    if (AppSettings.Instance.GetNodeText("ShowEditors.CamControl") == "true")
                    {
                        this.OpenEditor("CamControl");
                        this.CamControl.Location = new Point(700, 50);
                    }

                    if (AppSettings.Instance.GetNodeText("ShowEditors.SceneGraphViewer") == "true")
                    {
                        m_sceneGraphViewer = (ISceneGraphViewer)this.OpenEditor("SceneGraphViewer");
                        m_sceneGraphViewer.SelectedSprite = Stage.DefaultParent;
                    }

                    if (AppSettings.Instance.GetNodeText("ShowEditors.MessageWindow") == "true")
                    {
                        m_msgWnd = (IMessageWindow)this.OpenEditor("MessageWindow");
                    }

                    if (AppSettings.Instance.GetNodeText("ShowEditors.ResourceBrowser") == "true")
                        this.OpenEditor("ResourceBrowser");

                }
            }

            if (AppSettings.Instance.GetNodeText("OnScreenEdit") == "true")
                this.m_bSendMouseEventsToSprites = false;

            this.Stage.OnEndUpdate += new StageBase.RenderDelegate(Stage_OnEndUpdate);
        }

        void Stage_OnEndUpdate()
        {
            //    if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
            //    {
            //        Endogine.BitmapHelpers.Canvas canvas = this.Stage.Capture();
            //        Bitmap bmp = canvas.ToBitmap();
            //        bmp.Save("Screenshot.png");
            //        Console.WriteLine("Up!");
            //    }
        }

        public void ShutDown()
        {
            //			this._startupForm.Close();
            //			this._startupForm.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public void Dispose()
        {
            if (m_castlib != null)
                m_castlib.Dispose();
            if (Stage != null)
                Stage.Dispose();
        }

        public static EndogineHub Instance
        {
            get { return m_endogine; }
        }

        public static void Put(string s)
        {
            if (m_msgWnd != null)
                m_msgWnd.Put(s);
        }

        public void PreSetFullscreen(bool a_b) //use before engine is initialized
        {
            m_bFullscreen = a_b;
        }

        public void PreSetRenderStrategy(string a_sName) //use before engine is initialized
        {
            this._renderStrategy = a_sName;
        }

        public StageBase Stage
        {
            get { return this._stage; }
        }
        public Audio.SoundManager SoundManager
        {
            get { return this._soundManager; }
        }

        public string ApplicationPath
        {
            get { return m_sApplicationPath; }
        }
        public string ApplicationDirectory
        {
            get
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(m_sApplicationPath);
                return finfo.Directory.FullName + "\\";
            }
        }

        public CastLib CastLib
        {
            get { return m_castlib; }
        }

        /// <summary>
        /// Used when drag'n'dropping sprites - a sprite may have been disposed and replaced
        /// by another type. Use this method to get the object that was actually created.
        /// </summary>
        public Sprite[] LatestCreatedSprites
        {
            get { return this.m_latestCreatedSprites; }
            set { this.m_latestCreatedSprites = value; }
        }

        public bool OnScreenEdit
        {
            set { m_bSendMouseEventsToSprites = !value; }
        }

        public EPoint MouseLoc
        { get { return m_pntMouseLoc; } }
        public bool MouseDown
        { get { return m_bMouseDown; } }

        public bool MainLoop()
        {
            if (this.Stage == null)
                return false;
            this.OnPaint(null);
            return true;
        }

        public void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //TODO: DK Ej OnPaint!!
            //Application.DoEvents();

            System.Drawing.Point pnt = this.Stage.RenderControl.PointToScreen(new Point(0, 0));
            m_pntMouseLoc.X = System.Windows.Forms.Form.MousePosition.X - pnt.X;
            m_pntMouseLoc.Y = System.Windows.Forms.Form.MousePosition.Y - pnt.Y;

            if (Stage == null)
                return;

            if (EnterFrameEvent != null)
                EnterFrameEvent();

            if (m_bSendMouseEventsToSprites || true) //TODO: ....
            {
                Stage.RootSprite.CheckChildrenMouse(new MouseEventArgs(System.Windows.Forms.Control.MouseButtons, 0,
                         m_pntMouseLoc.X, m_pntMouseLoc.Y, 0), m_pntMouseLoc.ToEPointF(), false, m_bMouseDown);
                //TODO: unnecessary to check sprites again. Optimize!
                ArrayList aSprites = Stage.RootSprite.GetSpritesUnderLoc(m_pntMouseLoc.ToEPointF(), 1);
                if (aSprites.Count > 0 && ((Sprite)aSprites[0]).Cursor != null)
                {
                    object cursor = ((Sprite)aSprites[0]).Cursor;
                    if (cursor.GetType() == typeof(System.Windows.Forms.Cursor))
                        this.Stage.RenderControl.Cursor = (System.Windows.Forms.Cursor)cursor;
                }
                else
                    this.Stage.RenderControl.Cursor = System.Windows.Forms.Cursors.Default;
            }



            //TODO: remove, just testing renderToTexture:
            bool renderToTexture = false; // (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift);
            if (renderToTexture)
            {
                //this.Stage.Capture();
                Endogine.BitmapHelpers.PixelDataProvider pdp = this.Stage.CreatePixelDataProvider(256, 256, 4);
                this.Stage.RenderTarget = pdp;
            }

            this._stage.Update();

            if (renderToTexture)
                this.Stage.RenderTarget = null;
        }


        public void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            if (KeyEvent != null)
                KeyEvent(e, false);
        }
        public void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (KeyEvent != null)
                KeyEvent(e, true);
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            EPointF pntMouse = new EPointF(e.X, e.Y);
            //MessageBox.Show(this.m_formMdiParent, "Aloha!");
            //MessageBox.Show(this.m_formMdiParent.ActiveMdiChild, "Aloha!");

            if (m_bSendMouseEventsToSprites || true) //TODO: ....
            {
                Stage.RootSprite.CheckChildrenMouse(e, pntMouse, true, true);
                //else
                //{
                //select a sprite on stage:
                if (e.Button == MouseButtons.Right || (e.Button == MouseButtons.Left && System.Windows.Forms.Control.ModifierKeys == Keys.Control))
                {
                    int nNumSprites = -1;
                    if (e.Button == MouseButtons.Left)
                        nNumSprites = 1;
                    m_aSpritesInContextMenu = Stage.RootSprite.GetSpritesUnderLoc(pntMouse, nNumSprites);

                    if (m_aSpritesInContextMenu.Count > 1
                        || (m_aSpritesInContextMenu.Count == 1 && e.Button == MouseButtons.Right))
                    {
                        System.Windows.Forms.ContextMenu cxmn = new ContextMenu();
                        foreach (Sprite sp in m_aSpritesInContextMenu)
                        {
                            cxmn.MenuItems.Add(sp.GetSceneGraphName());
                            cxmn.MenuItems[cxmn.MenuItems.Count - 1].Click += new EventHandler(ContextMenu_Click);
                        }
                        cxmn.MenuItems.Add("-");
                        cxmn.MenuItems.Add("Exit edit mode");
                        cxmn.Show(this.Stage.RenderControl, new Point(e.X, e.Y));
                    }
                    else if (m_aSpritesInContextMenu.Count == 1)
                    {
                        m_sceneGraphViewer.SelectedSprite = (Sprite)m_aSpritesInContextMenu[0];
                    }
                }
            }

            m_pntMouseLoc.X = e.X;
            m_pntMouseLoc.Y = e.Y;
            m_bMouseDown = true;
            if (MouseButtonEvent != null) MouseButtonEvent(sender, e, true);
        }
        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (m_bSendMouseEventsToSprites || true) //TODO: ....
                Stage.RootSprite.CheckChildrenMouse(e, new EPointF(e.X, e.Y), true, false);

            m_pntMouseLoc.X = e.X;
            m_pntMouseLoc.Y = e.Y;
            m_bMouseDown = false;
            if (MouseButtonEvent != null) MouseButtonEvent(sender, e, false);
        }
        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (MouseScrollEvent != null) MouseScrollEvent(sender, e);
        }
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            //Stage.RootSprite.CheckMouse(e, new PointF(e.X,e.Y));

            //			m_pntMouseLoc.X = e.X;
            //			m_pntMouseLoc.Y = e.Y;
        }

        private void ContextMenu_Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            if (mi.Text == "Exit edit mode")
                return;
            else if (mi.Text == "-")
                return;
            else
            {
                Sprite sp = (Sprite)m_aSpritesInContextMenu[mi.Index];
                m_sceneGraphViewer.SelectedSprite = sp;
                m_sceneGraphViewer.MarkSprite(sp);
            }
        }

        public IEditorForm OpenEditor(string editorType) //System.Type editorType)
        {
            IEditorForm editor = null;

            if (editorType == "CamControl")
            {
                if (this.CamControl == null)
                {
                    this.CamControl = (ILocScaleRotEdit)EditorFactory.CreateEditor("LocScaleRotEdit");
                    this.CamControl.EditSprite = Stage.Camera;
                    this.CamControl.AutoswitchToSprite = false;
                    this.CamControl.Disposed += new EventHandler(editor_Disposed);
                }
                editor = this.CamControl;
            }
            else
            {
                editor = EditorFactory.CreateEditor(editorType);
                editor.Disposed += new EventHandler(editor_Disposed);
            }

            this.ShowEditor(editor);
            return editor;
        }

        void editor_Disposed(object sender, EventArgs e)
        {
            if (sender == this.CamControl)
                this.CamControl = null;
            else if (sender == this.m_sceneGraphViewer)
                this.m_sceneGraphViewer = null;
            else if (sender == m_msgWnd)
                m_msgWnd = null;
        }

        public void ShowEditor(Editors.IEditorForm editor) //Form
        {
            if (m_formMdiParent != null && editor.MdiParent != m_formMdiParent)
                editor.MdiParent = m_formMdiParent;
            editor.Show();
        }

        public Sprite SpriteToAddChildrenTo
        {
            get
            {
                if (this.m_sceneGraphViewer.SelectedSprite != null)
                    return this.m_sceneGraphViewer.SelectedSprite;
                return this.Stage.DefaultParent;
            }
        }

        public Form MdiParent
        {
            get { return this.m_formMdiParent; }
        }
    }
}
