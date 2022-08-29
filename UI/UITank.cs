using BaseLibrary.Utility;
using FluidLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BaseLibrary.UI;

[ExtendsFromMod("FluidLibrary")]
public class UITank : BaseElement
{
	private FluidStorage storage;
	private int slot;

	public FluidStack FluidStack => storage[slot];

	public UITank(FluidStorage itemStorage, int slot)
	{
		Width.Pixels = 44;
		Height.Pixels = 44;

		this.slot = slot;
		storage = itemStorage;
	}

	public override int CompareTo(BaseElement other) => other is UITank uiSlot ? slot.CompareTo(uiSlot.slot) : 0;

	protected override void Draw(SpriteBatch spriteBatch)
	{
		DepthStencilState stencilMask = new DepthStencilState
		{
			StencilEnable = true,
			StencilFunction = CompareFunction.Always,
			StencilPass = StencilOperation.Replace,
			ReferenceStencil = 1,
			DepthBufferEnable = false
		};

		DepthStencilState stencilLiquid = new DepthStencilState
		{
			StencilEnable = true,
			StencilFunction = CompareFunction.LessEqual,
			StencilPass = StencilOperation.Keep,
			ReferenceStencil = 1,
			DepthBufferEnable = false
		};

		RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		var projection = Matrix.CreateOrthographicOffCenter(0,
			Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth / Main.UIScale,
			Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight / Main.UIScale,
			0, 0, 1
		);

		var shader = new AlphaTestEffect(Main.graphics.GraphicsDevice)
		{
			Projection = projection
		};

		var texture = TextureAssets.InventoryBack.Value;

		// Draw slot to color and stencil buffer
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, stencilMask, rasterizerState, shader, Main.UIScaleMatrix);
		DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);
		spriteBatch.End();

		if (FluidStack.Fluid == null)
			return;

		Texture2D fluidTexture = ModContent.Request<Texture2D>(FluidStack.Fluid.Texture).Value;

		float fill = FluidStack.Volume / (float)storage.MaxVolumeFor(slot);

		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, stencilLiquid, rasterizerState, null, Main.UIScaleMatrix);

		// Draw fluid (use stencil)
		int height = (int)(Dimensions.Height * fill);
		spriteBatch.Draw(fluidTexture, new Rectangle((int)Dimensions.BottomLeft().X, (int)Dimensions.BottomLeft().Y - height, Dimensions.Width, height), Color.White);

		spriteBatch.End();

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizerState, shader, Main.UIScaleMatrix);

		// Draw border of slot
		DrawingUtility.DrawSlot(spriteBatch, Dimensions, ModContent.Request<Texture2D>(BaseLibrary.AssetPath + "Textures/UI/SlotBorder").Value, Color.White);
	}
}