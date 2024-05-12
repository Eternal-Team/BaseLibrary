using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BaseLibrary;

public static class DrawingUtility
{
	public static class Colors
	{
		public static readonly Color Panel = new(73, 94, 171);
		public static readonly Color PanelSelected = new(51, 65, 119);
		public static readonly Color PanelDisabled = new(71, 78, 102);
		public static readonly Color PanelHovered = new(94, 120, 221);
		public static readonly Color Slot = new(63, 65, 151);
		public static readonly Color Outline = new(18, 18, 38);
	}

	// public static Asset<Texture2D> Pixel = ModContent.Request<Texture2D>(BaseLibrary.TexturePath + "UI/Pixel");

	public static Texture2D GetTexturePremultiplied(string path)
	{
		Texture2D texture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
		Color[] buffer = new Color[texture.Width * texture.Height];
		texture.GetData(buffer);
		for (int i = 0; i < buffer.Length; i++) buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
		texture.SetData(buffer);
		return texture;
	}

	// public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions)
	// {
	// 	spriteBatch.Draw(texture, dimensions, Color.White);
	// }
	//
	// public static void DrawSlot(SpriteBatch spriteBatch, Rectangle dimensions, Texture2D texture, Color? color = null)
	// {
	// 	Point point = new Point(dimensions.X, dimensions.Y);
	// 	Point point2 = new Point(point.X + dimensions.Width - 8, point.Y + dimensions.Height - 8);
	// 	int width = point2.X - point.X - 8;
	// 	int height = point2.Y - point.Y - 8;
	//
	// 	Color value = color ?? UICommon.DefaultUIBlue;
	// 	spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 8, 8), new Rectangle(0, 0, 8, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 8, 8), new Rectangle(44, 0, 8, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 8, 8), new Rectangle(0, 44, 8, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 8, 8), new Rectangle(44, 44, 8, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y, width, 8), new Rectangle(8, 0, 36, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point.X + 8, point2.Y, width, 8), new Rectangle(8, 44, 36, 8), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 8, 8, height), new Rectangle(0, 8, 8, 36), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 8, 8, height), new Rectangle(44, 8, 8, 36), value);
	// 	spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y + 8, width, height), new Rectangle(8, 8, 36, 36), value);
	// }
	//
	// public static void DrawSlot(SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? color = null, Texture2D texture = null)
	// {
	// 	DrawSlot(spriteBatch, dimensions.ToRectangle(), texture, color);
	// }

	public static void DrawPanel(SpriteBatch spriteBatch, Rectangle dimensions, Texture2D texture, Color color)
	{
		Point point = new Point(dimensions.X, dimensions.Y);
		Point point2 = new Point(point.X + dimensions.Width - 12, point.Y + dimensions.Height - 12);
		int width = point2.X - point.X - 12;
		int height = point2.Y - point.Y - 12;
		spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 12, 12), new Rectangle(0, 0, 12, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 12, 12), new Rectangle(16, 0, 12, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 12, 12), new Rectangle(0, 16, 12, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 12, 12), new Rectangle(16, 16, 12, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y, width, 12), new Rectangle(12, 0, 4, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point.X + 12, point2.Y, width, 12), new Rectangle(12, 16, 4, 12), color);
		spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 12, 12, height), new Rectangle(0, 12, 12, 4), color);
		spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 12, 12, height), new Rectangle(16, 12, 12, 4), color);
		spriteBatch.Draw(texture, new Rectangle(point.X + 12, point.Y + 12, width, height), new Rectangle(12, 12, 4, 4), color);
	}

	public static void DrawPanel(SpriteBatch spriteBatch, Rectangle rectangle, Color? bgColor = null, Color? borderColor = null)
	{
		DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBackground").Value, bgColor ?? Colors.Panel);
		DrawPanel(spriteBatch, rectangle, Main.Assets.Request<Texture2D>("Images/UI/PanelBorder").Value, borderColor ?? Color.Black);
	}

	// public static void DrawItemInInventory(SpriteBatch spriteBatch, Item item, Vector2 position, Vector2 size, float scale, bool drawStackSize)
	// {
	// 	Main.instance.LoadItem(item.type);
	// 	Texture2D itemTexture = TextureAssets.Item[item.type].Value;
	//
	// 	Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
	// 	Color newColor = Color.White;
	// 	float pulseScale = 1f;
	// 	ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);
	// 	int height = rect.Height;
	// 	int width = rect.Width;
	// 	float drawScale = 1f;
	//
	// 	float availableWidth = size.X;
	// 	if (width > availableWidth || height > availableWidth)
	// 	{
	// 		if (width > height) drawScale = availableWidth / width;
	// 		else drawScale = availableWidth / height;
	// 	}
	//
	// 	drawScale *= scale;
	// 	// Vector2 position = position ;
	// 	Vector2 origin = rect.Size() * 0.5f;
	//
	// 	if (ItemLoader.PreDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale))
	// 	{
	// 		spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
	// 		if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
	// 	}
	//
	// 	ItemLoader.PostDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale);
	// 	if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
	// 	if (drawStackSize && item.stack > 1)
	// 	{
	// 		string text = /*!Settings.ShortStackSize ||*/ item.stack < 1000 ? item.stack.ToString() : TextUtility.ToSI(item.stack, "N1");
	// 		float texscale = 0.75f;
	// 		// note: i dont think this will scale well with larger slot sizes
	// 		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position + new Vector2(8, size.Y - FontAssets.MouseText.Value.MeasureString(text).Y * texscale), Color.White, 0f, Vector2.Zero, new Vector2(texscale));
	// 	}
	// }
	//
	// public static void DrawItemInWorld(SpriteBatch spriteBatch, Item item, Vector2 position, Vector2 size, float rotation = 0f)
	// {
	// 	Main.instance.LoadItem(item.type);
	// 	Texture2D itemTexture = TextureAssets.Item[item.type].Value;
	//
	// 	Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
	// 	Color newColor = Color.White;
	// 	float pulseScale = 1f;
	// 	ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);
	//
	// 	float availableWidth = size.X;
	// 	int width = rect.Width;
	// 	int height = rect.Height;
	// 	float drawScale = 1f;
	// 	if (width > availableWidth || height > availableWidth)
	// 	{
	// 		if (width > height) drawScale = availableWidth / width;
	// 		else drawScale = availableWidth / height;
	// 	}
	//
	// 	Vector2 origin = rect.Size() * 0.5f;
	//
	// 	float totalScale = pulseScale * drawScale;
	//
	// 	if (ItemLoader.PreDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), ref rotation, ref totalScale, item.whoAmI))
	// 	{
	// 		spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), rotation, origin, totalScale, SpriteEffects.None, 0f);
	// 		if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), rotation, origin, totalScale, SpriteEffects.None, 0f);
	// 	}
	//
	// 	ItemLoader.PostDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), rotation, totalScale, item.whoAmI);
	// 	if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f), new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
	// }
	//
	// public static void DrawOutline(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int lineSize = 2)
	// {
	// 	int width = (int)Math.Abs(start.X - end.X);
	// 	int height = (int)Math.Abs(start.Y - end.Y);
	//
	// 	Point topleft = Vector2.Min(start, end).ToPoint();
	//
	// 	// top
	// 	spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(topleft.X, topleft.Y, width, lineSize), color);
	// 	// left
	// 	spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(topleft.X, topleft.Y, lineSize, height), color);
	// 	// bottom
	// 	spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(topleft.X, topleft.Y + height - lineSize, width, lineSize), color);
	// 	// right
	// 	spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(topleft.X + width - lineSize, topleft.Y, lineSize, height), color);
	// }
	//
	// public static void DrawTextWithBorder(SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 position, Color textColor, Color borderColor, Vector2 origin, float rotation, float scale = 1f)
	// {
	// 	// shadows
	// 	sb.DrawString(font, text, position + new Vector2(-2f, 0f), borderColor, rotation, origin, scale, SpriteEffects.None, 0.0f);
	// 	sb.DrawString(font, text, position + new Vector2(2f, 0f), borderColor, rotation, origin, scale, SpriteEffects.None, 0.0f);
	// 	sb.DrawString(font, text, position + new Vector2(0f, -2f), borderColor, rotation, origin, scale, SpriteEffects.None, 0.0f);
	// 	sb.DrawString(font, text, position + new Vector2(0f, 2f), borderColor, rotation, origin, scale, SpriteEffects.None, 0.0f);
	//
	// 	sb.DrawString(font, text, position, textColor, rotation, origin, scale, SpriteEffects.None, 0.0f);
	// }
}