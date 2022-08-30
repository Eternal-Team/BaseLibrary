using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BaseLibrary.UI;

public class UIGradient : BaseElement
{
	private Asset<Effect> shader;
	private Asset<Texture2D> border;

	public UIGradient()
	{
		shader ??= ModContent.Request<Effect>(BaseLibrary.AssetPath + "Effects/GradientShader");
		border ??= ModContent.Request<Texture2D>(BaseLibrary.TexturePath + "UI/SlotSquare");
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		Effect? effect = shader.Value;
		if (effect is null) return;

		DrawingUtility.DrawSlot(spriteBatch, Dimensions, border.Value);

		RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, rasterizerState, effect, Main.UIScaleMatrix);

		effect.Parameters["uImageSize0"].SetValue(Dimensions.Size());
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Dimensions.X + 2, Dimensions.Y + 2, Dimensions.Width - 4, Dimensions.Height - 4));

		spriteBatch.End();

		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, rasterizerState, null, Main.UIScaleMatrix);
	}
}