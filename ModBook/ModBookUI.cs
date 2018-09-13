using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.UI;
using TheArtOfDev.HtmlRenderer.WinForms;
using TheOneLibrary.Base.UI;
using Color = Microsoft.Xna.Framework.Color;

namespace BaseLibrary.ModBook
{
	public class ModBookUI : BaseUI
	{
		private static readonly Texture2D dividerTexture;
		private UIGrid<UITexture> gridHTML = new UIGrid<UITexture>();
		private UIScrollbar scrollbarHTML = new UIScrollbar();

		static ModBookUI()
		{
			dividerTexture = TextureManager.Load("Images/UI/Divider");
		}

		public override void Initialize()
		{
			panelMain.Width.Precent = 0.5f;
			panelMain.Height.Precent = 0.5f;
			panelMain.Center();
			panelMain.OnPostDraw += spriteBatch =>
			{
				CalculatedStyle dimensions = panelMain.GetDimensions();
				spriteBatch.Draw(dividerTexture, dimensions.Position() + new Vector2(dimensions.Width / 2f - 2f, 0), null, Color.White, 90f.ToRadians(), Vector2.Zero, new Vector2(dimensions.Height / 8f, 1f), SpriteEffects.None, 0f);
			};

			gridHTML.Width.Set(-16f, 0.5f);
			gridHTML.Height.Set(-16f, 1f);
			gridHTML.Left.Set(8f, 0.5f);
			gridHTML.Top.Set(8f, 0f);
			gridHTML.ListPadding = 4;
			gridHTML.OverflowHidden = true;
			panelMain.Append(gridHTML);

			scrollbarHTML.Height.Set(-16f, 1f);
			scrollbarHTML.Left.Set(-28f, 1f);
			scrollbarHTML.Top.Set(8f, 0f);
			scrollbarHTML.SetView(100f, 1000f);
			panelMain.Append(scrollbarHTML);
			gridHTML.SetScrollbar(scrollbarHTML);
		}

		public void Load(ModBook modBook)
		{
			int index = 0;
			foreach (Category category in modBook.Categories)
			{
				UIButton categoryButton = new UIButton(category.GetTexture(), ScaleMode.Zoom);
				categoryButton.Width.Pixels = 40;
				categoryButton.Height.Pixels = 40;
				categoryButton.Left.Pixels = 8;
				categoryButton.Top.Pixels = 8 + 48 * index++;
				categoryButton.GetHoverText += () => category.Name;
				panelMain.Append(categoryButton);
			}

			string html = $@"<h1 style='font-family:Comic Sans MS;color:white;'>This is a heading</h1><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><h1>This is the end</h1>";
			Image image = HtmlRender.RenderToImageGdiPlus(html, (int)gridHTML.GetDimensions().Width);
			image.Save(@"C:\Users\Itorius\Desktop\test.png", ImageFormat.Png);
			//HtmlPanel panel = new HtmlPanel
			//{
			//    Text = html,
			//    Size = new Size(500, 500)
			//};
			//
			//foreach (LinkElementData<RectangleF> data in panel.GetValue<HtmlContainer>("_htmlContainer").GetLinks())
			//{
			//    Debug.WriteLine(data.Href + "\t" + data.Rectangle);
			//}

			UITexture texture = new UITexture(LoadPicture(@"C:\Users\Itorius\Desktop\test.png"));
			texture.Width.Precent = 1f;
			texture.Height.Pixels = image.Height;
			texture.MaxHeight.Set(float.MaxValue, 0f);
			gridHTML.Add(texture);
		}

		public static Texture2D LoadPicture(string filename)
		{
			FileStream setStream = File.Open(filename, FileMode.Open);
			Texture2D NewTexture = Texture2D.FromStream(Main.instance.GraphicsDevice, setStream);
			setStream.Dispose();
			return NewTexture;
		}
	}
}