//using BaseLibrary.UI.Elements;
//using BaseLibrary.Utility;
//using TheOneLibrary.Base.UI;

//namespace BaseLibrary.ModBook
//{
//	public class ModBookUI : BaseUI
//	{
//		//public static Texture2D BookBackground { get; set; }

//		private UIGrid<UICategory> gridCategories;
//		private UIScrollbar scrollbarCategories;

//		//private UIGrid<UITexture> gridHTML = new UIGrid<UITexture>();
//		//private UIScrollbar scrollbarHTML = new UIScrollbar();

//		public override void Initialize()
//		{
//			panelMain = new UIDraggablePanel
//			{
//				Width = (1084, 0),
//				Height = (610, 0),
//				Padding = (8, 8, 8, 8)
//			};
//			panelMain.Center();

//			gridCategories = new UIGrid<UICategory>
//			{
//				Width = (-44f, 1f),
//				Height = (-16f, 1f),
//				Left = (8f, 0f),
//				Top = (8f, 0f),
//				ListPadding = 4,
//				OverflowHidden = true
//			};
//			panelMain.Append(gridCategories);

//			scrollbarCategories = new UIScrollbar
//			{
//				Left = (-28f, 1f),
//				Height = (-16f, 1f),
//				Top = (8f, 0f),
//				View = (100f, 1000f)
//			};
//			panelMain.Append(scrollbarCategories);
//			gridCategories.SetScrollbar(scrollbarCategories);

//			#region Old
//			//gridHTML.Width.Set(-16f, 0.5f);
//			//gridHTML.Height.Set(-16f, 1f);
//			//gridHTML.Left.Set(8f, 0.5f);
//			//gridHTML.Top.Set(8f, 0f);
//			//gridHTML.ListPadding = 4;
//			//gridHTML.OverflowHidden = true;
//			//panelMain.Append(gridHTML);

//			//scrollbarHTML.Height.Set(-16f, 1f);
//			//scrollbarHTML.Left.Set(-28f, 1f);
//			//scrollbarHTML.Top.Set(8f, 0f);
//			//scrollbarHTML.SetView(100f, 1000f);
//			//panelMain.Append(scrollbarHTML);
//			//gridHTML.SetScrollbar(scrollbarHTML); 
//			#endregion

//			//foreach (ModBook modBook in ModBookLoader.modBooks.Values)
//			//{
//			//}

//			//string html = File.ReadAllText(@"C:\Development\Web\HTML\index.html");
//			//CssData css = HtmlRender.ParseStyleSheet(File.ReadAllText(@"C:\Development\Web\CSS\index.css"));
//			//Image image = HtmlRender.RenderToImageGdiPlus(html /*, (int)gridHTML.GetDimensions().Width*/, cssData: css);

//			//Texture2D texture2D;
//			//using (MemoryStream stream = new MemoryStream())
//			//{
//			//	image.Save(stream, ImageFormat.Png);
//			//	texture2D = Texture2D.FromStream(Main.instance.GraphicsDevice, stream);
//			//}

//			//UITexture texture = new UITexture(texture2D)
//			//{
//			//	Width = (0, 1),
//			//	Height = (texture2D.Height, 0)
//			//};
//			//texture.MaxHeight.Set(float.MaxValue, 0);
//			//panelMain.Append(texture);
//		}

//		//public void Load(ModBook modBook)
//		//{
//		//	//foreach (Category category in modBook.Categories)
//		//	//{
//		//	//    UICategory uiCategory = new UICategory(category)
//		//	//    {
//		//	//        Width = (0, 1),
//		//	//        Height = (64, 0)
//		//	//    };
//		//	//    gridCategories.Add(uiCategory);
//		//	//    uiCategory.Initialize();
//		//	//}

//		//	//foreach (LinkElementData<RectangleF> data in panel.GetValue<HtmlContainer>("_htmlContainer").GetLinks())
//		//	//{
//		//	//    Debug.WriteLine(data.Href + "\t" + data.Rectangle);
//		//	//}

//		//	//UITexture texture = new UITexture(texture2D);
//		//	//texture.Width.Precent = 1f;
//		//	//texture.Height.Pixels = image.Height;
//		//	//texture.MaxHeight.Set(float.MaxValue, 0f);
//		//	//gridHTML.Add(texture);
//		//}
//	}
//}