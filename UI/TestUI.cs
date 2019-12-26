using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BaseLibrary.UI
{
	public class TestUI : BaseUI
	{
		public override void Initialize()
		{
			panelMain = new UIDraggablePanel
			{
				Width = (0, 0.75f),
				Height = (0, 0.5f)
			};
			panelMain.Center();

			UITextButton button = new UITextButton("Reinitialize")
			{
				Width = (0, 1),
				Height = (28, 0)
			};
			button.SetPadding(4);
			button.OnClick += (evt, element) =>
			{
				RemoveAllChildren();
				OnInitialize();
			};
			panelMain.Append(button);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Vector2 textPosition = panelMain.Position + new Vector2(8, 36);
			const string text = "A a B b C c D d E e F f G g H h I i J j K k L l M m N n O o P p Q q R r S s T t U u V v W w X x Y y Z z !\" # $ % & ' ( ) * + , - . / : ; < = > ? @ [ \\ ] ^ _ ` { | } ~";
			float scale = 1.2f;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;

			// ^^^ these ones look bad
			// VVV these ones look good

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, null);

			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, textPosition.X, textPosition.Y, Color.White, Color.Black, Vector2.Zero, scale);
			textPosition.Y += 28f * scale;
		}
	}
}