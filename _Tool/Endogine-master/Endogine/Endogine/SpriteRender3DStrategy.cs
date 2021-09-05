using System;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Drawing;
using Endogine.ResourceManagement;
using System.Collections;

namespace Endogine
{
	/// <summary>
	/// Summary description for SpriteRender3DStrategy.
	/// </summary>
	public class SpriteRender3DStrategy : SpriteRenderStrategy
	{
		private int numVerts = 4;
		private VertexBuffer vertexBuffer = null;
		private VertexFormats customVertexFlags = VertexFormats.Position | VertexFormats.Texture1;// | VertexFormats.Diffuse;
		//private VertexFormats customVertexFlags = VertexFormats.Position | VertexFormats.Texture1 | VertexFormats.Diffuse;
		private Device device = null;
		protected Material m_mtrl;

		private static VertexBuffer _largeVertexBuffer;
		private static Hashtable _spriteToVertextBufferIndex;

		public SpriteRender3DStrategy()
		{}

		public override void Dispose()
		{
			vertexBuffer.Dispose();
		}
		public override void Init()
		{
			device = m_endogine.Stage.D3DDevice;

			m_mtrl = new Material();
			this.SetColor(m_sp.Color);

			//TODO:! One single buffer for whole system!!!
			vertexBuffer = new VertexBuffer(typeof(CustomVertex), 
				numVerts, device, Usage.WriteOnly, customVertexFlags, Pool.Default);
			vertexBuffer.Created += new System.EventHandler(this.OnCreateVertexBuffer);
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
			m_mtrl.Diffuse = m_mtrl.Ambient = m_sp.Color; //Color.FromArgb(50,255,255,255); //m_sp.Color;
		}

		public override void SetMember(MemberBase a_mb)
		{
			this.OnCreateVertexBuffer(vertexBuffer, null);
		}

		public override void SetMemberAnimationFrame(int a_n)
		{
			AdjustVertices();
		}
//		public override void SetSourceRect(ERectangle rct)
//		{
//			AdjustVertices();
//		}
		public override void RecalcedParentOutput()
		{
			AdjustVertices();
		}



		protected float[,] GenerateUVs()
		{
			ERectangleF rctfCropped = m_sp.GetPortionOfMemberToDisplay();
  
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
			vertexBuffer = new VertexBuffer(typeof(CustomVertex), 
				numVerts, device, Usage.Dynamic, customVertexFlags, Pool.Default);
			AdjustVertices();
		}

		protected void AdjustVertices()
		{
			if (m_sp.Member == null)
				return;
			if (vertexBuffer == null)
				return;

			float[,] aUVs = GenerateUVs();

			ERectangleF rct = new ERectangleF(0,0,1,1);

			CustomVertex[] vertices = null;
			try
			{
				vertices = vertexBuffer.Lock(0, 0) as CustomVertex[];
			}
			catch (Exception e)
			{
				//TODO: why does this happen?? 
				return;
				//throw new Exception("Failed to lock buffer...");
			}
			int nColor = System.Drawing.Color.FromArgb(255,255,0,120).ToArgb(); //255,255,255,255
			vertices[0] = new CustomVertex(rct.Left,		rct.Top, 0.0f, aUVs[0,0], aUVs[0,1]);//,      nColor);
			vertices[1] = new CustomVertex(rct.Right,	rct.Top, 0.0f, aUVs[1,0], aUVs[1,1]);//,      nColor);
			vertices[2] = new CustomVertex(rct.Left,		-rct.Bottom, 0.0f, aUVs[3,0], aUVs[3,1]);//,nColor);
			vertices[3] = new CustomVertex(rct.Right,	-rct.Bottom, 0.0f, aUVs[2,0], aUVs[2,1]);//,nColor);
			vertexBuffer.Unlock();
		}
		public override void EnterFrame()
		{
			//AdjustVertices();
		}

		public override void SubDraw() 
		{
			ERectangleF rctDraw = m_sp.CalcRectInDrawTarget();
			//TODO: if same rect as last time, use a cached matrix instead of re-calculating all this.

//			if (m_sp.Picture != null)
//			{
//				ERectangleF x = rctDraw.Copy();
//				x.Intersect(new ERectangleF(0,0,600,600));
//				if (!x.IsEmpty && !x.IsNegative)
//				{
//					x.Width+=1;
//				}
//			}


			Matrix QuadMatrix = Matrix.Scaling(rctDraw.Width, rctDraw.Height, 1);

			EPointF pntRegOff = m_sp.RegPoint.ToEPointF()/new EPointF(m_sp.SourceRect.Width, m_sp.SourceRect.Height) * new EPointF(rctDraw.Width, rctDraw.Height);

			QuadMatrix.Multiply(Matrix.Translation(-pntRegOff.X, pntRegOff.Y,0));
			QuadMatrix.Multiply(Matrix.RotationZ(-m_sp.Rotation));
			QuadMatrix.Multiply(Matrix.Translation(pntRegOff.X, -pntRegOff.Y,0));

			EPointF pntLoc = new EPointF(rctDraw.X-device.Viewport.Width/2, rctDraw.Y-device.Viewport.Height/2);

			QuadMatrix.Multiply(Matrix.Translation(pntLoc.X, -pntLoc.Y, 0f));

			Stage3D stage = ((Stage3D)EH.Instance.Stage);
			stage.ZCurrent+=stage.ZStep;
			QuadMatrix.M43 = 10000f - stage.ZCurrent-1;
			//QuadMatrix.M43 = 10000f-m_sp.LocZ-1;
//			if (this.m_sp.Parent.Name == "Bar" && (this.m_sp.Name == "2;1" || this.m_sp.Name == "2;2"))
//			{
//				this.m_sp.CalcInParent();
//				m_sp.CalcRectInDrawTarget();
//				float x = rctDraw.Bottom;
//				x++;
//			}

			device.Material = m_mtrl;

			//TODO: (in controlling mechanism:) render order by RenderStates somehow? I hear that state changes are expensive
			switch (m_sp.Ink)
			{
				case RasterOps.ROPs.Copy:
					if (this.m_sp.Member.GotAlpha || true) //this.m_mtrl.Diffuse.A != 255)
					{
						device.RenderState.AlphaBlendEnable = true;
						device.RenderState.SourceBlend = Blend.SourceAlpha;
						device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

						if (this.m_mtrl.Diffuse.A != 255)
						{
							device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
							device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;
							device.TextureState[0].AlphaOperation = TextureOperation.Modulate;

							device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
							device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
							device.TextureState[0].ColorOperation = TextureOperation.Modulate;

							device.TextureState[1].AlphaOperation = TextureOperation.Disable;
							device.TextureState[1].ColorOperation = TextureOperation.Disable;
						}
					}
					else
					{
						device.RenderState.AlphaBlendEnable = false;
					}
					break;
				case RasterOps.ROPs.AddPin:
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.DestinationBlend = Blend.InvSourceColor;
					device.RenderState.SourceBlend = Blend.One;
					break;
				case RasterOps.ROPs.Multiply:
					//multiply?
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.BlendOperation = BlendOperation.Min; //device.RenderState.SourceBlend = Blend.DestinationColor same?
					break;
				case RasterOps.ROPs.BgTransparent:
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.SourceBlend = Blend.SourceAlpha;
					device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

//					if (m_sp.Blend!=255)
//					{
//						device.RenderState.BlendFactor = Color.FromArgb(0, m_sp.Blend, m_sp.Blend,m_sp.Blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
//						device.RenderState.AlphaSourceBlend = Blend.BlendFactor;
//					}
					break;
				case RasterOps.ROPs.Lightest:
					//lightest?
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.BlendOperation = BlendOperation.Max; 
					break;
				case RasterOps.ROPs.SubtractPin: //OK
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.SourceBlend = Blend.Zero;
					device.RenderState.DestinationBlend = Blend.InvSourceColor;
					break;
				case RasterOps.ROPs.Difference: //Hmm..
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.SourceBlend = Blend.InvDestinationColor;
					device.RenderState.DestinationBlend = Blend.InvSourceColor;
					break;
				case RasterOps.ROPs.D3DTest1:
					//Director blend ink (by sprite's Blend value)
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.BlendFactor = Color.FromArgb(0, m_sp.Blend, m_sp.Blend,m_sp.Blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
					device.RenderState.SourceBlend = Blend.BlendFactor;
					device.RenderState.DestinationBlend = Blend.InvBlendFactor;
					break;
				case RasterOps.ROPs.D3DTest2:
					//Alpha channel blending
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.BlendFactor = Color.FromArgb(0, m_sp.Blend, m_sp.Blend,m_sp.Blend);
					device.RenderState.SourceBlend = Blend.SourceAlpha;
					device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
					break;
				case RasterOps.ROPs.D3DTest3:
					//don't understand this one...
					device.RenderState.AlphaBlendEnable = true;
					device.RenderState.SourceBlend = Blend.InvSourceColor;
					break;
				case RasterOps.ROPs.D3DTest4:
					//device.RenderState.AlphaTestEnable = false; 
					//device.RenderState.ReferenceAlpha = 0x01; 
					//device.RenderState.AlphaFunction = Compare.GreaterEqual; 
					device.RenderState.AlphaBlendEnable = true;
					//device.RenderState.BlendFactor = Color.FromArgb(m_sp.Blend, 255,255,255);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
					device.RenderState.BlendFactor = Color.FromArgb(0, m_sp.Blend, m_sp.Blend,m_sp.Blend);//m_sp.Blend, m_sp.Blend, m_sp.Blend);
					//device.RenderState.AlphaSourceBlend = Blend.BlendFactor;
					//device.RenderState.AlphaDestinationBlend = Blend.BlendFactor;
					device.RenderState.SourceBlend = Blend.BlendFactor;
					device.RenderState.DestinationBlend = Blend.InvBlendFactor;
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

			device.SetTexture(0,m_sp.Member.Texture);
			
			device.SamplerState[0].MagFilter = this.m_sp.TextureMagFilter;
			device.SamplerState[0].MinFilter = this.m_sp.TextureMinFilter;

			device.SetStreamSource(0, vertexBuffer, 0);
			device.VertexFormat = customVertexFlags;
			device.SetTransform(TransformType.View, QuadMatrix);
			device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, numVerts-2);
		}
	}
}

