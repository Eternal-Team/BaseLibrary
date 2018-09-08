using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using static BaseLibrary.Utility.Utility;

namespace BaseLibrary.UI
{
	// todo: introduce Vector2 for position and size
	// todo: make left,right,top,left floats
	// todo: functions for size and position?

	public class BaseElement : UIElement
	{
		public event Action<SpriteBatch> OnPreDraw;
		public event Action<SpriteBatch> OnPostDraw;
		public event Func<string> GetHoverText;

		public override void Draw(SpriteBatch spriteBatch)
		{
			//if (Visible && (VisibleFunc?.Invoke() ?? true))
			//{
			PreDraw(spriteBatch);

			if (_useImmediateMode) spriteBatch.DrawImmediate(DrawSelf);
			else DrawSelf(spriteBatch);

			if (OverflowHidden) spriteBatch.DrawOverflowHidden(this, DrawChildren);
			else DrawChildren(spriteBatch);

			PostDraw(spriteBatch);

			if (IsMouseHovering && GetHoverText != null) DrawMouseText(GetHoverText.Invoke());
			//}
		}

		public virtual void PreDraw(SpriteBatch spriteBatch)
		{
			OnPreDraw?.Invoke(spriteBatch);
		}

		public virtual void PostDraw(SpriteBatch spriteBatch)
		{
			OnPostDraw?.Invoke(spriteBatch);
		}
	}
}