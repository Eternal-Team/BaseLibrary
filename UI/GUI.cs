﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public interface IGUI
	{
		BaseUI Interface_UI { get; set; }
		UserInterface Interface { get; set; }
		LegacyGameInterfaceLayer InterfaceLayer { get; set; }
		bool Draw();
	}

	public class GUI<T> : IGUI where T : BaseUI
	{
		public BaseUI Interface_UI { get; set; }

		public T UI
		{
			get => (T)Interface_UI;
			set => Interface_UI = value;
		}

		public UserInterface Interface { get; set; }
		public LegacyGameInterfaceLayer InterfaceLayer { get; set; }
		public bool Visible;

		public GUI(T ui, UserInterface userInterface, InterfaceScaleType scaleType)
		{
			UI = ui;
			Interface = userInterface;
			Type type = ui.GetType();
			InterfaceLayer = new LegacyGameInterfaceLayer($"{type.Assembly.GetName().Name}:{type.Name}", Draw, scaleType);
		}

		public bool Draw()
		{
			if (Visible) UI.Draw(Main.spriteBatch);

			return true;
		}

		public void Update(GameTime gameTime)
		{
			if (Visible) Interface.Update(gameTime);
		}
	}
}