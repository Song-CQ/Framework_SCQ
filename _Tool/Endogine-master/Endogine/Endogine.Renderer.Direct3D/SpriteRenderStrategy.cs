using System;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Drawing;
using System.Collections.Generic;

namespace Endogine.Renderer.Direct3D
{
	/// <summary>
	/// Summary description for SpriteRender3DStrategy.
	/// </summary>
	public class SpriteRenderStrategy : Endogine.SpriteRenderStrategy
	{
		int _vertexCount = 4;
		VertexBuffer _vertexBuffer = null;
		VertexFormats _customVertexFlags = VertexFormats.Position | VertexFormats.Texture1;// | VertexFormats.Diffuse;
		//private VertexFormats customVertexFlags = VertexFormats.Position | VertexFormats.Texture1 | VertexFormats.Diffuse;
		private Device _device = null;

        //http://www.gamedev.net/community/forums/topic.asp?topic_id=391584

		protected Material _mtrl;
        RasterOps.ROPs _ffpOp = RasterOps.ROPs.Copy;
        Matrix _matrix = Matrix.Identity;
        int _blend = 255;
        TextureFilter _magFilter = TextureFilter.Linear; //TODO: Check default of device?
        TextureFilter _minFilter = TextureFilter.Linear;
        Texture _tx;

		//private static VertexBuffer _largeVertexBuffer;
		//private static Hashtable _spriteToVertexBufferIndex;

        //Shader _shader;
        Effect _effect;

		public SpriteRenderStrategy()
		{
        }

		public override void Dispose()
		{
			_vertexBuffer.Dispose();
		}

        public override Endogine.ResourceManagement.Shader  Shader
        {
            set { base.Shader = value; this._effect = ((Shader)value).Effect; }
            get { return base.Shader; }
        }

		public Device Device
		{
			get {return this._device;}
			set {this._device = value;}
		}
		public override void Init()
		{
			_mtrl = new Material();
            if (_sp!=null)
      			this.SetColor(_sp.Color);
            else
                this.SetColor(System.Drawing.Color.FromArgb(255,255,255));

			//TODO:! One single buffer for whole system!!!
            //TODO: OnCreateVertexBuffer also creates one... why?
			_vertexBuffer = new VertexBuffer(typeof(CustomVertex), 
				_vertexCount, _device, Usage.WriteOnly, _customVertexFlags, Pool.Default);
			_vertexBuffer.Created += new System.EventHandler(this.OnCreateVertexBuffer);
		}

		private struct CustomVertex 
		{
			public float X;
			public float Y;
			public float Z;
			public float Tu;
			public float Tv;
			//public int Color;
			public CustomVertex(float x, float y, float z, float tu, float tv)//, int color) 
			{
				X = x;
				Y = y;
				Z = z;
				Tu = tu;
				Tv = tv;
				//Color = color;
			}
		}

		public override void SetColor(Color a_clr)
		{
			//m_mtrl.Emissive = m_sp.Color;
            _mtrl.Diffuse = _mtrl.Ambient = a_clr; //Color.FromArgb(50,255,255,255); //_sp.Color;
		}

		public override void SetMember(Endogine.ResourceManagement.MemberBase a_mb)
		{
			this.OnCreateVertexBuffer(_vertexBuffer, null);
            this._tx = ((MemberSpriteBitmapRenderStrategy)((MemberSpriteBitmap)a_mb).RenderStrategy).Texture;
            //this._tx = ((MemberSpriteBitmapRenderStrategyA)_sp.Member.RenderStrategy).Texture;
		}

        public override void SetPixelDataProvider(Endogine.BitmapHelpers.PixelDataProvider pdp)
        {
            this.OnCreateVertexBuffer(_vertexBuffer, null);
            this._tx = ((PixelDataProvider)pdp).Texture;
        }

		public override void SetMemberAnimationFrame(int a_n)
		{
			AdjustVertices(null);
		}
//		public override void SetSourceRect(ERectangle rct)
//		{
//			AdjustVertices();
//		}
		public override void RecalcedParentOutput()
		{
			AdjustVertices(null);
		}

        protected override void SetSourceClipRect(ERectangleF rct)
        {
            this.AdjustVertices(rct);
        }

        protected float[,] GenerateUVs(ERectangleF rctfCropped)
		{
			float tXOffset = 0f; //.5f/sizeReal.Width;
			float tYOffset = 0f; //.5f/sizeReal.Height;

			float[,] tUVs = new float[,]{
{rctfCropped.X+tXOffset,			rctfCropped.Y+tYOffset},
{rctfCropped.OppositeX-tXOffset,		rctfCropped.Y+tYOffset},
{rctfCropped.OppositeX-tXOffset,		rctfCropped.OppositeY-tYOffset},
{rctfCropped.X+tXOffset,			rctfCropped.OppositeY-tYOffset}};

			return tUVs;
		}

		protected void OnCreateVertexBuffer(object sender, EventArgs e) 
		{
			_vertexBuffer = new VertexBuffer(typeof(CustomVertex),
                _vertexCount, _device, Usage.WriteOnly, _customVertexFlags, Pool.Default); //Usage.Dynamic
			AdjustVertices(null);
		}

		protected void AdjustVertices(ERectangleF rctfCropped)
		{
            if (_sp != null)
            {
                if (_sp.Member == null)
                    return;
                rctfCropped = _sp.GetPortionOfMemberToDisplay();
            }
            else
            {
                if (rctfCropped == null)
                    rctfCropped = new ERectangleF(0, 0, 1, 1);
            }

			if (_vertexBuffer == null)
				return;

			float[,] aUVs = GenerateUVs(rctfCropped);

			
			CustomVertex[] vertices = null;
			try
			{
				vertices = _vertexBuffer.Lock(0, 0) as CustomVertex[];
			}
			catch (Exception e)
			{
				//TODO: why does this happen?? 
                Console.WriteLine(e.Message);
				return;
				//throw new Exception("Failed to lock buffer...");
			}
            //ERectangleF rct = new ERectangleF(0, 0, 1, 1);
            //int nColor = System.Drawing.Color.FromArgb(255,255,0,120).ToArgb(); //255,255,255,255
            //vertices[0] = new CustomVertex(rct.Left,		rct.Top, 0.0f, aUVs[0,0], aUVs[0,1]);//,      nColor);
            //vertices[1] = new CustomVertex(rct.Right,	rct.Top, 0.0f, aUVs[1,0], aUVs[1,1]);//,      nColor);
            //vertices[2] = new CustomVertex(rct.Left,		-rct.Bottom, 0.0f, aUVs[3,0], aUVs[3,1]);//,nColor);
            //vertices[3] = new CustomVertex(rct.Right,	-rct.Bottom, 0.0f, aUVs[2,0], aUVs[2,1]);//,nColor);
            vertices[0] = new CustomVertex(0f, 0f, 0.0f, aUVs[0, 0], aUVs[0, 1]);
            vertices[1] = new CustomVertex(1f, 0f, 0.0f, aUVs[1, 0], aUVs[1, 1]);
            vertices[2] = new CustomVertex(0f, -1f, 0.0f, aUVs[3, 0], aUVs[3, 1]);
            vertices[3] = new CustomVertex(1f, -1f, 0.0f, aUVs[2, 0], aUVs[2, 1]);
			_vertexBuffer.Unlock();
		}
		public override void EnterFrame()
		{
			//AdjustVertices();
		}

        public Matrix CreateMatrix(ERectangleF rctDrawTarget, float rotation, EPoint regPoint, EPoint sourceRectSize)
        {
            System.Drawing.Rectangle rctView = this._device.ScissorRectangle; //this._device.Viewport

            Matrix m = Matrix.Scaling(rctDrawTarget.Width, rctDrawTarget.Height, 1);
            EPointF pntRegOff = regPoint.ToEPointF() / new EPointF(sourceRectSize.X, sourceRectSize.Y) * new EPointF(rctDrawTarget.Width, rctDrawTarget.Height);
            m.Multiply(Matrix.Translation(-pntRegOff.X, pntRegOff.Y, 0));
            m.Multiply(Matrix.RotationZ(-rotation));
            m.Multiply(Matrix.Translation(pntRegOff.X, -pntRegOff.Y, 0));

            EPointF pntLoc = new EPointF(rctDrawTarget.X - rctView.Width / 2, rctDrawTarget.Y - rctView.Height / 2);
            m.Multiply(Matrix.Translation(pntLoc.X, -pntLoc.Y, 0f));
            m.M43 = 9000f;
            
            return m;
        }
        public void SetMatrix(Matrix m)
        {
            this._matrix.M11 = m.M11;
            this._matrix.M12 = m.M12;
            this._matrix.M13 = m.M13;
            this._matrix.M14 = m.M14;

            this._matrix.M21 = m.M21;
            this._matrix.M22 = m.M22;
            this._matrix.M23 = m.M23;
            this._matrix.M24 = m.M24;
         
            this._matrix.M31 = m.M31;
            this._matrix.M32 = m.M32;
            this._matrix.M33 = m.M33;
            this._matrix.M34 = m.M34;

            this._matrix.M41 = m.M41;
            this._matrix.M42 = m.M42;
            this._matrix.M43 = m.M43;
            this._matrix.M44 = m.M44;
        }

        public override void CalcRenderRegion(ERectangleF rctDrawTarget, float rotation, EPoint regPoint, EPoint sourceRectSize)
        {
            if (this._effect == null)
                this._matrix = this.CreateMatrix(rctDrawTarget, rotation, regPoint, sourceRectSize);
            else
            {
                //Have to calculate differently when using shaders (haven't looked into why exactly yet)
                System.Drawing.Rectangle rctView = this._device.ScissorRectangle;
                this._matrix = Matrix.Scaling(rctDrawTarget.Width / rctView.Width * 2, rctDrawTarget.Height / rctView.Height * 2, 1); //Matrix.Identity;
                ////EPointF pntRelativeRegPoint = this._sp.RegPoint.ToEPointF() / this._sp.SourceRect.Size.ToEPointF();
                ////EPointF pntRegOff = regPoint.ToEPointF() / new EPointF(sourceRectSize.X, sourceRectSize.Y) * new EPointF(rctDrawTarget.Width, rctDrawTarget.Height);
                ////m2.Multiply(Matrix.Translation(-pntRegOff.X, pntRegOff.Y, 0));
                ////m2.Multiply(Matrix.RotationZ(-rotation));
                ////m2.Multiply(Matrix.Translation(pntRegOff.X, -pntRegOff.Y, 0));
                this._matrix.Multiply(Matrix.Translation(rctDrawTarget.X / rctView.Width * 2 - 1f, 1f - rctDrawTarget.Y / rctView.Height * 2, 0));
            }
        }

        private void GetInfoFromSprite()
        {
            //TODO: recalculate only if these values have changed (or any parent's values)
            this._matrix = this.CreateMatrix(this._sp.CalcRectInDrawTarget(), this._sp.Rotation, this._sp.RegPoint, this._sp.SourceRect.Size);

            Stage stage = ((Stage)EH.Instance.Stage);
            stage.ZCurrent += stage.ZStep;
            this._matrix.M43 = 10000f - stage.ZCurrent - 1;
            //this._matrix.M43 = 10000f-m_sp.LocZ-1;

            this._ffpOp = this._sp.Ink;
            this._blend = this._sp.Blend;

            if (this._sp.TextureMagFilter == Sprite.TextureFilters.High)
                this._magFilter = TextureFilter.GaussianQuad;
            else if (this._sp.TextureMagFilter == Sprite.TextureFilters.Low)
                this._magFilter = TextureFilter.Linear;

            if (this._sp.TextureMinFilter == Sprite.TextureFilters.High)
                this._minFilter = TextureFilter.GaussianQuad;
            else if (this._sp.TextureMinFilter == Sprite.TextureFilters.Low)
                this._minFilter = TextureFilter.Linear;

            this._tx = ((MemberSpriteBitmapRenderStrategy)_sp.Member.RenderStrategy).Texture; //m_sp.Member.Texture
        }

        public Endogine.BitmapHelpers.PixelDataProvider PixelDataProvider
        {
            set
            {
                this._tx = ((PixelDataProvider)value).Texture;
            }
        }

		public override void SubDraw() 
		{
            if (this._sp!=null)
                this.GetInfoFromSprite();

			this._device.Material = this._mtrl;

            if (this._effect == null)
            {
                //TODO: (in controlling mechanism:) render order by RenderStates somehow? I hear that state changes are expensive
                switch (this._ffpOp)
                {
                    case RasterOps.ROPs.Copy:
                        if (true) // this.m_sp.Member.GotAlpha || true  //this.m_mtrl.Diffuse.A != 255)
                        {
                            _device.RenderState.AlphaBlendEnable = true;
                            _device.RenderState.SourceBlend = Blend.SourceAlpha;
                            _device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

                            if (this._mtrl.Diffuse.A != 255)
                            {
                                _device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
                                _device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;
                                _device.TextureState[0].AlphaOperation = TextureOperation.Modulate;

                                _device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                                _device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
                                _device.TextureState[0].ColorOperation = TextureOperation.Modulate;

                                _device.TextureState[1].AlphaOperation = TextureOperation.Disable;
                                _device.TextureState[1].ColorOperation = TextureOperation.Disable;
                            }
                        }
                        else
                        {
                            _device.RenderState.AlphaBlendEnable = false;
                        }
                        break;
                    case RasterOps.ROPs.AddPin:
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.DestinationBlend = Blend.InvSourceColor;
                        _device.RenderState.SourceBlend = Blend.One;
                        break;
                    case RasterOps.ROPs.Multiply:
                        //multiply?
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.BlendOperation = BlendOperation.Min; //device.RenderState.SourceBlend = Blend.DestinationColor same?
                        break;
                    case RasterOps.ROPs.BgTransparent:
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.SourceBlend = Blend.SourceAlpha;
                        _device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

                        //					if (this._blend!=255)
                        //					{
                        //						device.RenderState.BlendFactor = Color.FromArgb(0, this._blend, this._blend,this._blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
                        //						device.RenderState.AlphaSourceBlend = Blend.BlendFactor;
                        //					}
                        break;
                    case RasterOps.ROPs.Lightest:
                        //lightest?
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.BlendOperation = BlendOperation.Max;
                        break;
                    case RasterOps.ROPs.SubtractPin: //OK
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.SourceBlend = Blend.Zero;
                        _device.RenderState.DestinationBlend = Blend.InvSourceColor;
                        break;
                    case RasterOps.ROPs.Difference: //Hmm..
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.SourceBlend = Blend.InvDestinationColor;
                        _device.RenderState.DestinationBlend = Blend.InvSourceColor;
                        break;
                    case RasterOps.ROPs.D3DTest1:
                        //Director blend ink (by sprite's Blend value)
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.BlendFactor = Color.FromArgb(0, this._blend, this._blend, this._blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
                        _device.RenderState.SourceBlend = Blend.BlendFactor;
                        _device.RenderState.DestinationBlend = Blend.InvBlendFactor;
                        break;
                    case RasterOps.ROPs.D3DTest2:
                        //Alpha channel blending
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.BlendFactor = Color.FromArgb(0, this._blend, this._blend, this._blend);
                        _device.RenderState.SourceBlend = Blend.SourceAlpha;
                        _device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                        break;
                    case RasterOps.ROPs.D3DTest3:
                        //don't understand this one...
                        _device.RenderState.AlphaBlendEnable = true;
                        _device.RenderState.SourceBlend = Blend.InvSourceColor;
                        break;
                    case RasterOps.ROPs.D3DTest4:
                        //device.RenderState.AlphaTestEnable = false; 
                        //device.RenderState.ReferenceAlpha = 0x01; 
                        //device.RenderState.AlphaFunction = Compare.GreaterEqual; 
                        _device.RenderState.AlphaBlendEnable = true;
                        //device.RenderState.BlendFactor = Color.FromArgb(this._blend, 255,255,255);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
                        _device.RenderState.BlendFactor = Color.FromArgb(0, this._blend, this._blend, this._blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
                        //device.RenderState.AlphaSourceBlend = Blend.BlendFactor;
                        //device.RenderState.AlphaDestinationBlend = Blend.BlendFactor;
                        _device.RenderState.SourceBlend = Blend.BlendFactor;
                        _device.RenderState.DestinationBlend = Blend.InvBlendFactor;
                        //device.RenderState.Ambient = System.Drawing.Color.FromArgb(0, 255, 255, 255);
                        //m_mtrl.Ambient = m_mtrl.Diffuse = Color.FromArgb(10, m_mtrl.Diffuse.R, m_mtrl.Diffuse.G, m_mtrl.Diffuse.B);
                        //device.RenderState.BlendFactor = Color.FromArgb(10, 127, 127, 127);
                        //device.RenderState.DiffuseMaterialSource = ColorSource.Material;
                        //device.RenderState.BlendOperation = BlendOperation.Add;
                        //device.SetTextureStageState(0, TextureStageStates.AlphaArgument0, 100f);
                        //device.Material = m_mtrl;

                        //device.SetRenderState(RenderStates.DestinationBlend, 4);
                        //device.RenderState.SourceBlend = Blend.BothSourceAlpha; //let's though key only?
                        //device.RenderState.DestinationBlend = Blend.SourceColor; //some transparency in grayish areas?
                        //device.RenderState.BlendOperation = BlendOperation.Subtract; ??
                        //device.RenderState.BlendOperation = BlendOperation.Add;
                        //device.SetRenderState(RenderStates.BlendFactor, 0.5f);
                        //device.RenderState.SourceBlend = Blend.SourceAlpha;
                        //device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                        //device.RenderState.BlendFactor = Color.FromArgb(127, 127, 127, 127);
                        //device.RenderState.BlendFactor = Color.FromArgb(127, 255, 255, 255);
                        //m_mtrl.Diffuse = Color.FromArgb(127, m_mtrl.Diffuse.R, m_mtrl.Diffuse.G, m_mtrl.Diffuse.B);
                        //device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, 100f);
                        break;

                    //http://www.two-kings.de/tutorials/d3d.html //disappeared??
                    // http://www.toymaker.info/Games/html/render_states.html
                }
                _device.SetTransform(TransformType.View, this._matrix);
            }

            _device.SetTexture(0,this._tx);

            //TODO: GaussianQuad doesn't work sometimes...
            try
            {
                _device.SamplerState[0].MagFilter = this._magFilter;
                _device.SamplerState[0].MinFilter = this._minFilter;
            }
            catch { }
			_device.SetStreamSource(0, _vertexBuffer, 0);
			_device.VertexFormat = _customVertexFlags;
            
            //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/directx9_c/dx9_graphics_reference_hlsl_intrinsic_functions.asp

            if (this._effect == null)
            {
                this._device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, this._vertexCount - 2);
            }
            else
            {
                this._effect.SetValue("matViewProjection", this._matrix); // matViewProjection worldViewProj
                //foreach (KeyValuePair<string, object> kv in this.Shader.Parameters)
                //    this._effect.SetValue(kv.Key, kv.Value);

                //this._effect.SetValue("time", (float)(DateTime.Now.Ticks % 10000) / 10000); 
                //this._effect.SetValue("colorize", new Vector4(this._mtrl.Diffuse.A, this._mtrl.Diffuse.R, this._mtrl.Diffuse.G, this._mtrl.Diffuse.B));
                //this._effect.SetValue("texture", this._tx);
                this._effect.Technique = this.Shader.Technique; // "simple";

                this._effect.Begin(0);
                for (int passNum = 0; passNum < this.Shader.NumPassesToExecute; passNum++)
                {
                    this._effect.BeginPass(passNum);
                    this._device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, this._vertexCount - 2);
                    this._effect.EndPass();
                }
                this._effect.End();
            }

		}
	}
}