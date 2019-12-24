using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseLibrary
{
	public class SpriteBatchState
	{
		private BlendState blendState;
		private SamplerState samplerState;
		private DepthStencilState depthStencilState;
		private RasterizerState rasterizerState;

		public SpriteSortMode SpriteSortMode;
		public Effect CustomEffect;
		public Matrix TransformMatrix;
		public Rectangle ScissorRectangle;

		public BlendState BlendState
		{
			get => blendState ?? (blendState = BlendState.AlphaBlend);
			set => blendState = value;
		}

		public SamplerState SamplerState
		{
			get => samplerState ?? (samplerState = SamplerState.LinearClamp);
			set => samplerState = value;
		}

		public DepthStencilState DepthStencilState
		{
			get => depthStencilState ?? (depthStencilState = DepthStencilState.Default);
			set => depthStencilState = value;
		}

		public RasterizerState RasterizerState
		{
			get => rasterizerState ?? (rasterizerState = RasterizerState.CullCounterClockwise);
			set => rasterizerState = value;
		}
	}
}