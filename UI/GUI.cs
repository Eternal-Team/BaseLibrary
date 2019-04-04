using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class GUI<T> where T : BaseUI
	{
		public BaseUI Interface_UI { get; set; }

		public T UI
		{
			get => (T)Interface_UI;
			set => Interface_UI = value;
		}

		public UserInterface Interface { get; set; }
		public LegacyGameInterfaceLayer InterfaceLayer { get; set; }
		public Func<bool> Visible;

		public GUI(T ui, UserInterface userInterface, InterfaceScaleType scaleType)
		{
			UI = ui;
			Interface = userInterface;
			Type type = ui.GetType();
			InterfaceLayer = new LegacyGameInterfaceLayer($"{type.Assembly.GetName().Name}:{type.Name}", Draw, scaleType);
		}

		public bool Draw()
		{
			if (Visible()) Interface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);

			return true;
		}

		public void Update(GameTime gameTime)
		{
			if (Visible()) Interface.Update(gameTime);
		}
	}
}