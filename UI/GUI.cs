using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class GUI<T> where T : BaseUI
	{
		public T UI;

		public UserInterface Interface { get; set; }

		public LegacyGameInterfaceLayer InterfaceLayer { get; set; }

		public Func<bool> Visible;

		private bool _visible => Visible?.Invoke() ?? true;

		public GUI(T ui, UserInterface userInterface, InterfaceScaleType scaleType)
		{
			UI = ui;
			Interface = userInterface;
			Type type = ui.GetType();
			InterfaceLayer = new LegacyGameInterfaceLayer($"{type.Assembly.GetName().Name}:{type.Name}", Draw, scaleType);
		}

		public bool Draw()
		{
			if (_visible) Interface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);

			return true;
		}

		public void Update(GameTime gameTime)
		{
			if (_visible) Interface.Update(gameTime);
		}
	}
}