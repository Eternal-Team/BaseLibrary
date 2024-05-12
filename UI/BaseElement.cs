using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace BaseLibrary.UI;

public class BaseElement : IComparable<BaseElement>, IEnumerable<BaseElement>
{
	public virtual int CompareTo(BaseElement? other) => 0;

	public bool test;

	public List<BaseElement> Children = [];

	public void Add(BaseElement element)
	{
		Children.Add(element);
	}

	internal void InternalDraw(SpriteBatch spriteBatch)
	{
		// spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight), 30, 30), test ? Color.Red : Color.Green);

		foreach (BaseElement element in Children)
		{
			element.InternalDraw(spriteBatch);
		}
	}

	public void Update(GameTime gameTime)
	{
	}

	public IEnumerator<BaseElement> GetEnumerator() => Children.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Recalculate()
	{
	}
}