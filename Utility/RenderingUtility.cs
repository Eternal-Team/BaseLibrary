﻿using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using UIPanel = Terraria.GameContent.UI.Elements.UIPanel;

namespace BaseLibrary.Utility
{
	public static partial class Utility
	{
		public static string MouseText;
		public static Color? colorMouseText;

		internal static Texture2D TexturePanelBackground => typeof(UIPanel).GetValue<Texture2D>("_backgroundTexture");
		internal static Texture2D TexturePanelBorder => typeof(UIPanel).GetValue<Texture2D>("_borderTexture");

		public static void DrawPanel(this SpriteBatch spriteBatch, Rectangle dimensions, Texture2D texture, Color color)
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

		public static void DrawPanel(this SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? bgColor = null, Color? borderColor = null) => spriteBatch.DrawPanel(dimensions.ToRectangle(), bgColor, borderColor);

		public static void DrawPanel(this SpriteBatch spriteBatch, Rectangle rectangle, Color? bgColor = null, Color? borderColor = null)
		{
			spriteBatch.DrawPanel(rectangle, TexturePanelBackground, bgColor ?? ColorPanel);
			spriteBatch.DrawPanel(rectangle, TexturePanelBorder, borderColor ?? Color.Black);
		}

		public static void DrawOutline(this SpriteBatch spriteBatch, Point16 start, Point16 end, Color color, float lineSize, bool addZero = false)
		{
			float width = Math.Abs(start.X - end.X) * 16 + 16;
			float height = Math.Abs(start.Y - end.Y) * 16 + 16;

			Vector2 position = -Main.screenPosition + start.Min(end).ToVector2() * 16;

			if (addZero)
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen) zero = Vector2.Zero;
				position += zero;
			}

			spriteBatch.Draw(Main.magicPixel, position, null, color, 0f, Vector2.Zero, new Vector2(width, lineSize / 1000f), SpriteEffects.None, 0f);
			spriteBatch.Draw(Main.magicPixel, position, null, color, 0f, Vector2.Zero, new Vector2(lineSize, height / 1000f), SpriteEffects.None, 0f);

			spriteBatch.Draw(Main.magicPixel, position + new Vector2(0, height - lineSize), null, color, 0f, Vector2.Zero, new Vector2(width, lineSize / 1000f), SpriteEffects.None, 0f);
			spriteBatch.Draw(Main.magicPixel, position + new Vector2(width - lineSize, 0), null, color, 0f, Vector2.Zero, new Vector2(lineSize, height / 1000f), SpriteEffects.None, 0f);
		}

		public static void DrawMouseText(object text, Color? color = null)
		{
			MouseText = text.ToString();
			colorMouseText = color;
		}

		internal static bool DrawMouseText()
		{
			if (MouseText == null) return true;

			Main.LocalPlayer.showItemIcon = false;
			Main.ItemIconCacheUpdate(0);
			Main.mouseText = true;

			PlayerInput.SetZoom_UI();
			int hackedScreenWidth = Main.screenWidth;
			int hackedScreenHeight = Main.screenHeight;
			int hackedMouseX = Main.mouseX;
			int hackedMouseY = Main.mouseY;
			PlayerInput.SetZoom_UI();
			PlayerInput.SetZoom_Test();

			int posX = Main.mouseX + 10;
			int posY = Main.mouseY + 10;
			if (hackedMouseX != -1 && hackedMouseY != -1)
			{
				posX = hackedMouseX + 10;
				posY = hackedMouseY + 10;
			}

			if (Main.ThickMouse)
			{
				posX += 6;
				posY += 6;
			}

			Vector2 vector = ExtractText(MouseText).Measure();
			if (hackedScreenHeight != -1 && hackedScreenWidth != -1)
			{
				if (posX + vector.X + 4f > hackedScreenWidth) posX = (int)(hackedScreenWidth - vector.X - 4f);
				if (posY + vector.Y + 4f > hackedScreenHeight) posY = (int)(hackedScreenHeight - vector.Y - 4f);
			}
			else
			{
				if (posX + vector.X + 4f > Main.screenWidth) posX = (int)(Main.screenWidth - vector.X - 4f);
				if (posY + vector.Y + 4f > Main.screenHeight) posY = (int)(Main.screenHeight - vector.Y - 4f);
			}

			TextSnippet[] snippets = ChatManager.ParseMessage(MouseText, colorMouseText ?? Color.White).ToArray();
			if (snippets.Length > 1)
			{
				for (int i = 0; i < snippets.Length; i++)
				{
					TextSnippet textSnippet = snippets[i];

					if (textSnippet.Text.EndsWith("\n"))
					{
						if (i + 1 < snippets.Length)
						{
							textSnippet.Text = textSnippet.Text.Replace("\n", "");
							snippets[i + 1].Text = snippets[i + 1].Text.Insert(0, "\n");
						}
					}
				}
			}

			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, snippets, new Vector2(posX, posY), 0f, Vector2.Zero, Vector2.One, out int num);

			MouseText = null;

			return true;
		}

		public static Texture2D CreateGrad(int width, int steps, Channel channel)
		{
			Texture2D texture = new Texture2D(Main.graphics.GraphicsDevice, width, 1);
			Color[] data = new Color[width];

			int nextX = 0;
			int step = 0;
			foreach (int i in DistributeInteger(width, steps))
			{
				Color c = Color.White;
				switch (channel)
				{
					case Channel.R:
						c = new Color(255 * step / steps, 0, 0);
						break;
					case Channel.G:
						c = new Color(0, 255 * step / steps, 0);
						break;
					case Channel.B:
						c = new Color(0, 0, 255 * step / steps);
						break;
					case Channel.A:
						c = new Color(0, 0, 0, 255 * step / steps);
						break;
					default:
						c = HSL2RGB(step / (decimal)steps, 1.0m, 0.5m);
						break;
				}

				for (int x = nextX; x < nextX + i; x++) data[x] = c;

				nextX += i;
				step++;
			}

			texture.SetData(data);
			return texture;
		}

		public static Color HSL2RGB(decimal h, decimal sl, decimal l)
		{
			decimal r = l;
			decimal g = l;
			decimal b = l;

			decimal v = l <= 0.5m ? l * (1.0m + sl) : l + sl - l * sl;

			if (v > 0)
			{
				decimal m = l + l - v;
				decimal sv = (v - m) / v;
				h *= 6.0m;
				int sextant = (int)h;
				decimal fract = h - sextant;
				decimal vsf = v * sv * fract;
				decimal mid1 = m + vsf;
				decimal mid2 = v - vsf;

				if (h < 1m || h == 6m)
				{
					r = v;
					g = mid1;
					b = m;
				}
				else if (h < 2m)
				{
					r = mid2;
					g = v;
					b = m;
				}
				else if (h < 3m)
				{
					r = m;
					g = v;
					b = mid1;
				}
				else if (h < 4m)
				{
					r = m;
					g = mid2;
					b = v;
				}
				else if (h < 5m)
				{
					r = mid1;
					g = m;
					b = v;
				}
				else if (h < 6m)
				{
					r = v;
					g = m;
					b = mid2;
				}
			}

			return new Color(Convert.ToByte(r * 255m), Convert.ToByte(g * 255m), Convert.ToByte(b * 255m));
		}

		public static void LoadTextures(this Mod mod)
		{
			if (Main.dedServ) return;

			foreach (Type type in mod.Code.GetTypes())
			{
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(x => x.PropertyType == typeof(Texture2D) || x.PropertyType.GetElementType() == typeof(Texture2D) || x.PropertyType.GenericTypeArguments.Any() && x.PropertyType.GenericTypeArguments[0] == typeof(Texture2D)))
				{
					PathOverrideAttribute overrideAttribute = propertyInfo.GetCustomAttribute<PathOverrideAttribute>();

					if (propertyInfo.PropertyType.IsArray)
					{
						Texture2D[] array = (Texture2D[])propertyInfo.GetValue(null);
						for (int i = 0; i < array.Length; i++) array[i] = ModLoader.GetTexture(overrideAttribute?.path ?? $"{mod.Name}/Textures/{propertyInfo.Name}_{i}");
					}
					else if (propertyInfo.IsEnumerable())
					{
						List<Texture2D> list = (List<Texture2D>)propertyInfo.GetValue(null);
						int i = 0;
						while (File.Exists(overrideAttribute?.path ?? $"{mod.File.path}/Textures/{propertyInfo.Name}_{i}"))
						{
							list.Add(ModLoader.GetTexture(overrideAttribute?.path ?? $"{mod.Name}/Textures/{propertyInfo.Name}_{i}"));
							i++;
						}
					}
					else propertyInfo.SetValue(null, ModLoader.GetTexture(overrideAttribute?.path ?? $"{mod.Name}/Textures/{propertyInfo.Name}"));
				}
			}
		}

		#region SpriteBatch
		public static RasterizerState OverflowHiddenState = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true
		};

		public static void DrawImmediate(this SpriteBatch spriteBatch, Action<SpriteBatch> drawAction)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, OverflowHiddenState, null, Main.UIScaleMatrix);
			drawAction.Invoke(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, OverflowHiddenState, null, Main.UIScaleMatrix);
		}

		public static void DrawOverflowHidden(this SpriteBatch spriteBatch, UIElement uiElement, Action<SpriteBatch> drawAction)
		{
			Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
			RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
			SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

			spriteBatch.End();
			Rectangle clippingRectangle = uiElement.GetClippingRectangle(spriteBatch);
			Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenState, null, Main.UIScaleMatrix);

			drawAction.Invoke(spriteBatch);

			rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
			spriteBatch.End();
			spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position) => spriteBatch.Draw(texture, position, Color.White);

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle) => spriteBatch.Draw(texture, rectangle, Color.White);

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, CalculatedStyle dimensions, Color? color = null) => spriteBatch.Draw(texture, dimensions.ToRectangle(), color ?? Color.White);

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, RectangleF rectangleF, Color? color = null) => spriteBatch.Draw(texture, rectangleF.rectangle, null, color ?? Color.White);
		#endregion
	}

	public class PathOverrideAttribute : Attribute
	{
		public string path;

		public PathOverrideAttribute(string path) => this.path = path;
	}
}