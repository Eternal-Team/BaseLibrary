using System.IO;
using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using TheOneLibrary.Base.UI;

namespace BaseLibrary.ModBook
{
	public class ModBookUI : BaseUI
	{
		public static Texture2D BookBackground { get; set; }

		private UIGrid<UICategory> gridCategories = new UIGrid<UICategory>();
		private UIScrollbar scrollbarCategories = new UIScrollbar();

		private UIGrid<UITexture> gridHTML = new UIGrid<UITexture>();
		private UIScrollbar scrollbarHTML = new UIScrollbar();

		public override void Initialize()
		{
			panelMain.Width.Pixels = 1084;
			panelMain.Height.Pixels = 720;
			panelMain.customTexture = BookBackground;
			panelMain.Center();

			//gridCategories.Width.Set(-44f, 1f);
			//gridCategories.Height.Set(-16f, 1f);
			//gridCategories.Left.Set(8f, 0f);
			//gridCategories.Top.Set(8f, 0f);
			//gridCategories.ListPadding = 4;
			//gridCategories.OverflowHidden = true;
			//panelMain.Append(gridCategories);

			//scrollbarCategories.Left.Set(-28f, 1f);
			//scrollbarCategories.Height.Set(-16f, 1f);
			//scrollbarCategories.Top.Set(8f, 0f);
			//scrollbarCategories.SetView(100f, 1000f);
			//panelMain.Append(scrollbarCategories);
			//gridCategories.SetScrollbar(scrollbarCategories);

			//gridHTML.Width.Set(-16f, 0.5f);
			//gridHTML.Height.Set(-16f, 1f);
			//gridHTML.Left.Set(8f, 0.5f);
			//gridHTML.Top.Set(8f, 0f);
			//gridHTML.ListPadding = 4;
			//gridHTML.OverflowHidden = true;
			//panelMain.Append(gridHTML);

			//scrollbarHTML.Height.Set(-16f, 1f);
			//scrollbarHTML.Left.Set(-28f, 1f);
			//scrollbarHTML.Top.Set(8f, 0f);
			//scrollbarHTML.SetView(100f, 1000f);
			//panelMain.Append(scrollbarHTML);
			//gridHTML.SetScrollbar(scrollbarHTML);
		}

		public void Load(ModBook modBook)
		{
			foreach (Category category in modBook.Categories)
			{
				UICategory uiCategory = new UICategory(category)
				{
					Width = new StyleDimension(0f, 1f),
					Height = new StyleDimension(64f, 0f)
				};
				gridCategories.Add(uiCategory);
				uiCategory.Initialize();
			}

			//string html = File.ReadAllText(@"C:\Development\Web\HTML\index.html");
			//CssData css = HtmlRender.ParseStyleSheet(File.ReadAllText(@"C:\Development\Web\CSS\index.css"));
			//Image image = HtmlRender.RenderToImageGdiPlus(html/*, (int)gridHTML.GetDimensions().Width*/, cssData: css);

			//Texture2D texture2D;
			//using (MemoryStream stream = new MemoryStream())
			//{
			//	image.Save(stream, ImageFormat.Png);
			//	texture2D = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
			//}

			////foreach (LinkElementData<RectangleF> data in panel.GetValue<HtmlContainer>("_htmlContainer").GetLinks())
			////{
			////    Debug.WriteLine(data.Href + "\t" + data.Rectangle);
			////}

			//UITexture texture = new UITexture(texture2D);
			//texture.Width.Precent = 1f;
			//texture.Height.Pixels = image.Height;
			//texture.MaxHeight.Set(float.MaxValue, 0f);
			//gridHTML.Add(texture);
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