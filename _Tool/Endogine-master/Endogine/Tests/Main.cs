using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Endogine;

namespace Tests
{
    public class Main : Endogine.MainBase
    {
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem miParticle;
        private System.Windows.Forms.MenuItem miPuzzle;
        private System.Windows.Forms.MenuItem miFont;
        private System.Windows.Forms.MenuItem miParallax;
        private System.Windows.Forms.MenuItem miGifAnim;
        private System.Windows.Forms.MenuItem miDonut;
        private System.ComponentModel.IContainer components = null;

        private PuzzleBobble.PlayArea m_playArea = null;

        private Endogine.Forms.Label m_lbl;

        private SideScroller.GameMain m_scrollerGame = null;

        private ParticleControl m_particleControl = null;
        private System.Windows.Forms.MenuItem miGDIPerlin;
        private FunParticleSystem m_particleSystem = null;

        private Sprite m_spProcedural = null;

        private Sprite m_spDragGif = null;
        private System.Windows.Forms.MenuItem miCaveHunter;
        private System.Windows.Forms.MenuItem menuItem3;
        private Sprite m_spDonut = null;
        private System.Windows.Forms.MenuItem miSnooker;

        private CaveHunter.GameMain m_caveHunter = null;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem miSoundDrums;
        private System.Windows.Forms.MenuItem miSpaceInvaders;

        private Snooker.GameMain snooker = null;
        private SpaceInvaders.GameMain spaceInvaders = null;
        private MenuItem menuItem2;
        private MenuItem miProcessingSmoke;
        private MenuItem miProcessingCellular3;
        private Processing.Smoke smoke;
        private MenuItem miColor;
        private Processing.CellularAutomata3 cellular3;

        public Main()
        {
            InitializeComponent();
            //			this.Text = "Main!";
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miParticle = new System.Windows.Forms.MenuItem();
            this.miFont = new System.Windows.Forms.MenuItem();
            this.miGDIPerlin = new System.Windows.Forms.MenuItem();
            this.miGifAnim = new System.Windows.Forms.MenuItem();
            this.miDonut = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.miParallax = new System.Windows.Forms.MenuItem();
            this.miPuzzle = new System.Windows.Forms.MenuItem();
            this.miCaveHunter = new System.Windows.Forms.MenuItem();
            this.miSnooker = new System.Windows.Forms.MenuItem();
            this.miSpaceInvaders = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.miSoundDrums = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miProcessingSmoke = new System.Windows.Forms.MenuItem();
            this.miProcessingCellular3 = new System.Windows.Forms.MenuItem();
            this.miColor = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miParticle,
            this.miFont,
            this.miGDIPerlin,
            this.miGifAnim,
            this.miDonut,
            this.menuItem3,
            this.menuItem4,
            this.menuItem2,
            this.miColor});
            this.menuItem1.Text = "Engine tests";
            // 
            // miParticle
            // 
            this.miParticle.Index = 0;
            this.miParticle.Text = "Particle system";
            this.miParticle.Click += new System.EventHandler(this.miParticle_Click);
            // 
            // miFont
            // 
            this.miFont.Index = 1;
            this.miFont.Text = "Font";
            this.miFont.Click += new System.EventHandler(this.miFont_Click);
            // 
            // miGDIPerlin
            // 
            this.miGDIPerlin.Index = 2;
            this.miGDIPerlin.Text = "GDI+random procedural";
            this.miGDIPerlin.Click += new System.EventHandler(this.miGDIPerlin_Click);
            // 
            // miGifAnim
            // 
            this.miGifAnim.Index = 3;
            this.miGifAnim.Text = "Draggable animated gif";
            this.miGifAnim.Click += new System.EventHandler(this.miGifAnim_Click);
            // 
            // miDonut
            // 
            this.miDonut.Index = 4;
            this.miDonut.Text = "Bouncing donut";
            this.miDonut.Click += new System.EventHandler(this.miDonut_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 5;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miParallax,
            this.miPuzzle,
            this.miCaveHunter,
            this.miSnooker,
            this.miSpaceInvaders});
            this.menuItem3.Text = "Games";
            // 
            // miParallax
            // 
            this.miParallax.Index = 0;
            this.miParallax.Text = "Parallax Scroll";
            this.miParallax.Click += new System.EventHandler(this.miParallax_Click);
            // 
            // miPuzzle
            // 
            this.miPuzzle.Index = 1;
            this.miPuzzle.Text = "Puzzle Bobble";
            this.miPuzzle.Click += new System.EventHandler(this.miPuzzle_Click);
            // 
            // miCaveHunter
            // 
            this.miCaveHunter.Index = 2;
            this.miCaveHunter.Text = "Cave Hunter";
            this.miCaveHunter.Click += new System.EventHandler(this.miCaveHunter_Click);
            // 
            // miSnooker
            // 
            this.miSnooker.Index = 3;
            this.miSnooker.Text = "Snooker";
            this.miSnooker.Click += new System.EventHandler(this.miSnooker_Click);
            // 
            // miSpaceInvaders
            // 
            this.miSpaceInvaders.Index = 4;
            this.miSpaceInvaders.Text = "Space Invaders";
            this.miSpaceInvaders.Click += new System.EventHandler(this.miSpaceInvaders_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 6;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSoundDrums});
            this.menuItem4.Text = "Sound";
            // 
            // miSoundDrums
            // 
            this.miSoundDrums.Index = 0;
            this.miSoundDrums.Text = "Drum machine";
            this.miSoundDrums.Click += new System.EventHandler(this.miSoundDrums_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 7;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miProcessingSmoke,
            this.miProcessingCellular3});
            this.menuItem2.Text = "Processing-wannabe";
            // 
            // miProcessingSmoke
            // 
            this.miProcessingSmoke.Index = 0;
            this.miProcessingSmoke.Text = "Smoke";
            this.miProcessingSmoke.Click += new System.EventHandler(this.miProcessingSmoke_Click);
            // 
            // miProcessingCellular3
            // 
            this.miProcessingCellular3.Index = 1;
            this.miProcessingCellular3.Text = "Cellular Automata 3";
            this.miProcessingCellular3.Click += new System.EventHandler(this.miProcessingCellular3_Click);
            // 
            // miColor
            // 
            this.miColor.Index = 8;
            this.miColor.Text = "Color editors";
            this.miColor.Click += new System.EventHandler(this.miColor_Click);
            // 
            // Main
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(640, 481);
            this.Menu = this.mainMenu1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stage";
            this.ResumeLayout(false);

        }
        #endregion

        private void miParticle_Click(object sender, System.EventArgs e)
        {
            if (m_particleControl == null)
            {
                //MemberSpriteBitmap mbParticle = (MemberSpriteBitmap)EndogineHub.Instance.CastLib.GetOrCreate("Particle");
                //mbParticle.CenterRegPoint();

                m_particleSystem = new FunParticleSystem();
                m_particleSystem.ParticlePicRef = PicRef.GetOrCreate("Particle");
                m_particleSystem.Ink = RasterOps.ROPs.AddPin; //Difference looks nice (only works properly in GDI mode);
                m_particleSystem.SourceRect = new ERectangle(0, 0, 50, 50); //how big is the emitter
                m_particleSystem.LocZ = 100;

                m_particleControl = new ParticleControl(m_particleSystem);
            }
            else
            {
                m_particleControl.Dispose();
                m_particleControl = null;

                m_particleSystem.Dispose();
            }
        }

        private void miPuzzle_Click(object sender, System.EventArgs e)
        {
            if (m_playArea == null)
            {
                //This starts puzzle bobble:
                m_playArea = new PuzzleBobble.PlayArea();
                m_playArea.Loc = new EPointF(100, 50);
            }
            else
            {
                m_playArea.Dispose();
                m_playArea = null;
            }
        }

        private void miFont_Click(object sender, System.EventArgs e)
        {
            if (m_lbl == null)
            {
                Endogine.Text.FontGenerator fg = new Endogine.Text.FontGenerator();
                fg.Font = new Font("Verdana", 24);
                fg.PensAndBrushes.Add(new LinearGradientBrush(new Rectangle(0, 0, 20, 20), Color.FromArgb(255, 0, 0), Color.FromArgb(0, 0, 255), 45, false));
                fg.PensAndBrushes.Add(new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 255, 255), 1.8f));
                fg.DefinedCharacters = Endogine.Text.FontGenerator.GetCharSet(Endogine.Text.FontGenerator.CharSet.Default, false, false);
                //fg.UseStyleTemplate("Test1");
                //fg.FontSize = 36;
                //fg.DefinedCharacters = Endogine.Text.FontGenerator.GetCharSet(Endogine.Text.FontGenerator.CharSet.Default, false, false);

                m_lbl = new Endogine.Forms.Label();
                m_lbl.FontGenerator = fg;
                m_lbl.CharacterSpacing = 2;
                m_lbl.Text = "Kerning: VA";
                m_lbl.Loc = new EPointF(40, 40);

                ////Add a swing behavior to each sprite. Behaviors can be added to any sprite.
                for (int i = 0; i < m_lbl.Sprites.Count; i++)
                {
                    Sprite sp = m_lbl.Sprites[i];
                    sp.TextureFilter = Sprite.TextureFilters.High;
                    sp.AddBehavior(new BhSwing(i * 5));
                }
            }
            else
            {
                m_lbl.Dispose();
                m_lbl = null;
            }
        }

        private void miParallax_Click(object sender, System.EventArgs e)
        {
            if (m_scrollerGame == null)
            {
                m_scrollerGame = new SideScroller.GameMain();
            }
            else
            {
                m_scrollerGame.Dispose();
                m_scrollerGame = null;
            }
        }

        private void miGDIPerlin_Click(object sender, System.EventArgs e)
        {
            if (m_spProcedural == null)
            {
                #region Procedural noise bitmap

                Random rnd = new Random();
                Endogine.Procedural.Noise procedural = null;
                switch (rnd.Next(4))
                {
                    case 0:
                        procedural = new Endogine.Procedural.Plasma();
                        procedural.Decay = 0.5f;
                        procedural.Frequency = 0.1f;
                        procedural.Octaves = 3;
                        break;
                    case 1:
                        procedural = new Endogine.Procedural.Wood();
                        procedural.Decay = 0.5f;
                        procedural.Frequency = 0.1f;
                        procedural.Octaves = 3;
                        ((Endogine.Procedural.Wood)procedural).NumCircles = 5;
                        ((Endogine.Procedural.Wood)procedural).Turbulence = 0.3f;
                        break;
                    case 2:
                        procedural = new Endogine.Procedural.Marble();
                        procedural.Decay = 0.5f;
                        procedural.Frequency = 0.1f;
                        procedural.Octaves = 3;
                        ((Endogine.Procedural.Marble)procedural).Periods = new EPointF(15, 30);
                        ((Endogine.Procedural.Marble)procedural).Turbulence = 3.3f;
                        break;
                    case 3:
                        procedural = new Endogine.Procedural.Noise();
                        procedural.Decay = 0.5f;
                        procedural.Frequency = 1f;
                        procedural.Octaves = 3;
                        break;
                }


                Bitmap bmp;
                Graphics g;
                bmp = new Bitmap(200, 200);
                Endogine.BitmapHelpers.Canvas canvas = Endogine.BitmapHelpers.Canvas.Create(bmp);

                //create a color table that makes it look like lakes and mountains:
                System.Collections.SortedList aColors = new System.Collections.SortedList();
                aColors.Add(0.0, Color.FromArgb(0, 0, 190));
                aColors.Add(0.1, Color.FromArgb(0, 0, 255));
                aColors.Add(0.11, Color.FromArgb(0, 200, 0));
                aColors.Add(0.5, Color.FromArgb(150, 100, 0));
                aColors.Add(1.0, Color.FromArgb(255, 255, 255));
                procedural.SetColors(aColors);
                //write pixels to bitmap:
                canvas.Locked = true;
                procedural.WriteToBitmap(canvas);
                canvas.Locked = false;
                #endregion

                #region Create gradient bitmap
                //Create two gradients using GDI+, and merge them with my CopyPixels for special effects
                Bitmap bmpGradient = new Bitmap(200, 200);
                g = Graphics.FromImage(bmpGradient);
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(4, 0, bmpGradient.Width, bmpGradient.Height), Color.FromArgb(255, 0, 0), Color.FromArgb(0, 255, 0), 0f);
                g.FillRectangle(brush, brush.Rectangle);

                Bitmap bmp2 = new Bitmap(bmpGradient.Width, bmpGradient.Height);
                brush = new LinearGradientBrush(new Rectangle(0, 0, bmp2.Width, bmp2.Height), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 0, 0), (float)90);
                Graphics g2 = Graphics.FromImage(bmp2);
                g2.FillRectangle(brush, brush.Rectangle);

                RasterOps.CopyPixels(bmpGradient, bmp2, (int)RasterOps.ROPs.AddPin, 255);
                RasterOps.CopyPixels(bmp, bmpGradient, (int)RasterOps.ROPs.Lightest, 255);
                g.Dispose();
                #endregion

                MemberSpriteBitmap mb = new MemberSpriteBitmap(bmp);

                m_spProcedural = new Sprite();
                m_spProcedural.Name = "Procedural";
                m_spProcedural.Member = mb;
                m_spProcedural.Scaling = new EPointF(2, 2);
                m_spProcedural.Ink = 0;
            }
            else
            {
                //TODO: the bitmap will still remain in memory, should have an "autodispose" option,
                //so that resources gets disposed when no sprites are using them
                m_spProcedural.Dispose();
                m_spProcedural = null;
            }
        }

        private void miGifAnim_Click(object sender, System.EventArgs e)
        {
            if (m_spDragGif == null)
            {
                m_spDragGif = new DragSprite();
                m_spDragGif.SetGraphics("416");
                m_spDragGif.Ink = RasterOps.ROPs.D3DTest1;
                m_spDragGif.LocZ = 5;
                m_spDragGif.Loc = new EPointF(300, 300);
            }
            else
            {
                m_spDragGif.Dispose();
                m_spDragGif = null;
            }
        }

        private void miDonut_Click(object sender, System.EventArgs e)
        {
            if (m_spDonut == null)
            {
                m_spDonut = new Bouncer();
                m_spDonut.SetGraphics("donut");
                m_spDonut.TextureFilter = Sprite.TextureFilters.High;
                m_spDonut.Scaling = new EPointF(4, 4);
                m_spDonut.Ink = RasterOps.ROPs.D3DTest2;
                m_spDonut.LocZ = 1;
            }
            else
            {
                m_spDonut.Dispose();
                m_spDonut = null;
            }
        }

        private void miCaveHunter_Click(object sender, System.EventArgs e)
        {
            if (m_caveHunter == null)
                m_caveHunter = new CaveHunter.GameMain();
            else
            {
                m_caveHunter.Dispose();
                m_caveHunter = null;
            }
        }

        private void miSnooker_Click(object sender, System.EventArgs e)
        {
            if (this.snooker == null)
                this.snooker = new Snooker.GameMain();
            else
            {
                this.snooker.Dispose();
                this.snooker = null;
            }
        }

        private void miSpaceInvaders_Click(object sender, System.EventArgs e)
        {
            //			Test test = new Test();
            ////			test.MdiParent = this.MdiParent;
            //			test.Show();
            //			EH.Instance.Stage.RenderControl = test;

            if (this.spaceInvaders == null)
                this.spaceInvaders = new SpaceInvaders.GameMain();
            else
            {
                this.spaceInvaders.Dispose();
                this.spaceInvaders = null;
            }
        }


        public override void EndogineInitDone()
        {
            base.EndogineInitDone();
            //			Endogine.Forms.Frame frame2 = new Endogine.Forms.Frame();
            //			frame2.CuttingRect = frame.CuttingRect;
            //			frame2.MemberName = "MeterBar";
            //			frame2.Rect = new ERectangleF(0,0,32,50);

            //this.miSoundScripted_Click(null, null);

            //this.miSoundDrums_Click(null, null);

            
            //string sName = "DVDMenu.psd"; //Eagle Eagle2 Test2 Test DVDMenu
            //string sFile = @"C:\Documents and Settings\Jonas\Mina dokument\Visual Studio 2005\Projects\Jonas\Endogine\_Docs\PSD\" + sName;
            //Endogine.Serialization.Photoshop.Document psd =
            //    new Endogine.Serialization.Photoshop.Document(sFile);
        }

        private void miSoundScripted_Click(object sender, System.EventArgs e)
        {
            Endogine.Scripting.FlowScript.ListPlayer pl = new Endogine.Scripting.FlowScript.ListPlayer();
            System.IO.StreamReader rd = new System.IO.StreamReader(EH.Instance.CastLib.DirectoryPath + "FlowScript.txt");
            if (Endogine.Scripting.EScript.Functions.Instance == null)
            {
                Endogine.Scripting.EScript.Functions funcs;
                funcs = new Endogine.Scripting.EScript.Functions();
            }
            //Endogine.Scripting.EScript.Functions.SetUserValue("o", new Endogine.Sound());
            pl.Run(rd.ReadToEnd());
        }

        private void miSoundDrums_Click(object sender, System.EventArgs e)
        {
            //Tests.DrumMachine.PlayKeyboard pkb = new Tests.DrumMachine.PlayKeyboard();
            DrumMachine.DrumForm form = new Tests.DrumMachine.DrumForm();
        }

        private void miProcessingSmoke_Click(object sender, EventArgs e)
        {
            if (this.smoke == null)
            {
                //MessageBox.Show("This example currently requires BASS dll:s. Download from Endogine at CodeProject and unpack in the same folder as the executable.");
                this.smoke = new Tests.Processing.Smoke();
            }
            else
            {
                this.smoke.Dispose();
                this.smoke = null;
            }
        }

        private void miProcessingCellular3_Click(object sender, EventArgs e)
        {
            if (this.cellular3 == null)
                this.cellular3 = new Tests.Processing.CellularAutomata3();
            else
            {
                this.cellular3.Dispose();
                this.cellular3 = null;
            }
            //Tests.Processing.Hairy h = new Tests.Processing.Hairy();
        }

        private void miFlash_Click(object sender, EventArgs e)
        {
            string sFile = EH.Instance.CastLib.DirectoryPath + "\\Tiger.swf";//Rect Tiger two Circle 3Shapes Test Bitmap box
            Endogine.Serialization.Flash.Flash flash = new Endogine.Serialization.Flash.Flash(sFile);
        }


        private void miColor_Click(object sender, EventArgs e)
        {
            if (!Endogine.Editors.EditorFactory.HasDll)
            {
                throw new Exception("No GUI Editor .dll loaded. Put Endogine.Editors.dll or other next to the .exe and restart!");
            }
            //Endogine.Editors.EditorFactory.LoadDll(@"C:\Documents and Settings\Jonas\My Documents\Visual Studio 2005\Projects\Endogine\Endogine.Editors\bin\Debug\Endogine.Editors.dll");

            //Endogine.Editors.ISwatchesForm swatches = (Endogine.Editors.ISwatchesForm)EH.Instance.OpenEditor("ColorEditors.SwatchesForm");
            Endogine.Editors.IColorPickerForm picker = (Endogine.Editors.IColorPickerForm)EH.Instance.OpenEditor("ColorEditors.ColorPickerForm");
            picker.ColorObject = new Endogine.ColorEx.ColorHsb(90, 1, 1);
        }
    }
}
