using System;
using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BaseLibrary
{
	public static partial class Utility
	{
		private static string mouseText;
		private static Color? colorMouseText;

		private static Texture2D TexturePanelBackground = ModContent.GetTexture("Terraria/UI/PanelBackground");
		private static Texture2D TexturePanelBorder = ModContent.GetTexture("Terraria/UI/PanelBorder");

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

		public static void DrawPanel(this SpriteBatch spriteBatch, Rectangle rectangle, Color? bgColor = null, Color? borderColor = null)
		{
			spriteBatch.DrawPanel(rectangle, TexturePanelBackground, bgColor ?? ColorPanel);
			spriteBatch.DrawPanel(rectangle, TexturePanelBorder, borderColor ?? Color.Black);
		}

		public static void DrawPanel(this SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? bgColor = null, Color? borderColor = null) => spriteBatch.DrawPanel(dimensions.ToRectangle(), bgColor, borderColor);

		public static void DrawSlot(this SpriteBatch spriteBatch, Rectangle dimensions, Color? color = null, Texture2D texture = null)
		{
			if (texture == null) texture = Main.inventoryBack13Texture;

			Point point = new Point(dimensions.X, dimensions.Y);
			Point point2 = new Point(point.X + dimensions.Width - 8, point.Y + dimensions.Height - 8);
			int width = point2.X - point.X - 8;
			int height = point2.Y - point.Y - 8;

			Color value = color ?? ColorSlot;
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 8, 8), new Rectangle(0, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 8, 8), new Rectangle(44, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 8, 8), new Rectangle(0, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 8, 8), new Rectangle(44, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y, width, 8), new Rectangle(8, 0, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point2.Y, width, 8), new Rectangle(8, 44, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 8, 8, height), new Rectangle(0, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 8, 8, height), new Rectangle(44, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y + 8, width, height), new Rectangle(8, 8, 36, 36), value);
		}

		public static void DrawSlot(this SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? color = null, Texture2D texture = null) => spriteBatch.DrawSlot(dimensions.ToRectangle(), color, texture);

		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			float num = Vector2.Distance(start, end);
			Vector2 vector = (end - start) / num;
			Vector2 value = start;
			float rotation = vector.ToRotation();
			for (float num2 = 0f; num2 <= num; num2 += 1f)
			{
				spriteBatch.Draw(Main.blackTileTexture, value, null, color, rotation, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
				value = start + num2 * vector;
			}
		}

		public static void DrawOutline(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int lineSize = 2)
		{
			int width = (int)Math.Abs(start.X - end.X);
			int height = (int)Math.Abs(start.Y - end.Y);

			Point topleft = Vector2.Min(start, end).ToPoint();

			spriteBatch.Draw(Main.magicPixel, new Rectangle(topleft.X, topleft.Y, width, lineSize), color);
			spriteBatch.Draw(Main.magicPixel, new Rectangle(topleft.X, topleft.Y, lineSize, height), color);

			spriteBatch.Draw(Main.magicPixel, new Rectangle(topleft.X, topleft.Y + height - lineSize, width, lineSize), color);
			spriteBatch.Draw(Main.magicPixel, new Rectangle(topleft.X + width - lineSize, topleft.Y, lineSize, height), color);
		}

		public static void DrawMouseText(object text, Color? color = null)
		{
			mouseText = text.ToString();
			colorMouseText = color;
		}

		internal static bool DrawMouseText()
		{
			if (mouseText == null) return true;

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

			Vector2 vector = ExtractText(mouseText).Measure();
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

			TextSnippet[] snippets = ChatManager.ParseMessage(mouseText, colorMouseText ?? Color.White).ToArray();
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

			mouseText = null;
			colorMouseText = null;

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
						c = HSL2RGB(step / (float)steps, 1.0f, 0.5f);
						break;
				}

				for (int x = nextX; x < nextX + i; x++) data[x] = c;

				nextX += i;
				step++;
			}

			texture.SetData(data);
			return texture;
		}

		#region SpriteBatch
		public static readonly RasterizerState OverflowHiddenState = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true
		};

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color? color = null) => spriteBatch.Draw(texture, position, color ?? Color.White);

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color? color = null) => spriteBatch.Draw(texture, rectangle, color ?? Color.White);

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, CalculatedStyle dimensions, Color? color = null) => spriteBatch.Draw(texture, dimensions.ToRectangle(), color);

		public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state) => spriteBatch.Begin(state.spriteSortMode, state.blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.customEffect, state.transformMatrix);

		public struct SpriteBatchState
		{
			public SpriteSortMode spriteSortMode;
			public BlendState blendState;
			public SamplerState samplerState;
			public DepthStencilState depthStencilState;
			public RasterizerState rasterizerState;
			public Effect customEffect;
			public Matrix transformMatrix;
		}

		private static SpriteBatchState GetState(this SpriteBatch spriteBatch) => new SpriteBatchState
		{
			spriteSortMode = typeof(SpriteBatch).GetValue<SpriteSortMode>("spriteSortMode", spriteBatch),
			blendState = typeof(SpriteBatch).GetValue<BlendState>("blendState", spriteBatch),
			samplerState = typeof(SpriteBatch).GetValue<SamplerState>("samplerState", spriteBatch),
			depthStencilState = typeof(SpriteBatch).GetValue<DepthStencilState>("depthStencilState", spriteBatch),
			rasterizerState = typeof(SpriteBatch).GetValue<RasterizerState>("rasterizerState", spriteBatch),
			customEffect = typeof(SpriteBatch).GetValue<Effect>("customEffect", spriteBatch),
			transformMatrix = typeof(SpriteBatch).GetValue<Matrix>("transformMatrix", spriteBatch)
		};

		public static void DrawWithState(this SpriteBatch spriteBatch, SpriteBatchState state, Action<SpriteBatch> drawAction)
		{
			SpriteBatchState oldState = spriteBatch.GetState();

			spriteBatch.End();
			spriteBatch.Begin(state);
			drawAction(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(oldState);
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

		public static void DrawItem(this SpriteBatch spriteBatch, Item item, Vector2 position, Vector2 size)
		{
			if (!item.IsAir)
			{
				Texture2D itemTexture = Main.itemTexture[item.type];
				Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);
				float scale = Math.Min(size.X / rect.Width, size.Y / rect.Height);

				Vector2 origin = rect.Size() * 0.5f * pulseScale;

				if (ItemLoader.PreDrawInInventory(item, spriteBatch, position, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, scale * pulseScale))
				{
					spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), 0f, origin, scale * pulseScale, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), 0f, origin, scale * pulseScale, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInInventory(item, spriteBatch, position, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, scale * pulseScale);
				if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f), new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				if (item.stack > 1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
			}
		}

		public static void DrawNPC(this SpriteBatch spriteBatch, NPC npc, Vector2 position, Vector2 size)
		{
			Main.instance.LoadNPC(npc.type);

			Texture2D npcTexture = Main.npcTexture[npc.type];

			Rectangle rectangle = new Rectangle(0, 0, Main.npcTexture[npc.type].Width, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]);

			Color color = npc.color != Color.Transparent ? new Color(npc.color.R, npc.color.G, npc.color.B, 255f) : new Color(1f, 1f, 1f);

			Main.spriteBatch.Draw(npcTexture, position, rectangle, color, 0, rectangle.Size() * 0.5f, Math.Min(size.X / rectangle.Width, size.Y / rectangle.Height), SpriteEffects.None, 0);
		}

		public static void DrawProjectile(this SpriteBatch spriteBatch, Projectile proj, Vector2 position, Vector2 size)
		{
			Main.instance.LoadProjectile(proj.type);

			Texture2D projTexture = Main.projectileTexture[proj.type];

			Main.spriteBatch.Draw(projTexture, position, null, Color.White, 0, projTexture.Size() * 0.5f, Math.Min(size.X / projTexture.Width, size.Y / projTexture.Height), SpriteEffects.None, 0);
		}
		#endregion
	}
}