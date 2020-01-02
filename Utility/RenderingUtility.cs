using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using System;
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

		public static SamplerState DefaultSamplerState = SamplerState.LinearClamp;

		public static readonly RasterizerState OverflowHiddenState = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true
		};

		private static SpriteBatchState _immediateState;

		public static SpriteBatchState ImmediateState
		{
			get => _immediateState ?? (_immediateState = new SpriteBatchState
			{
				SpriteSortMode = SpriteSortMode.Immediate,
				BlendState = BlendState.AlphaBlend,
				SamplerState = SamplerState.LinearClamp,
				DepthStencilState = DepthStencilState.None,
				RasterizerState = OverflowHiddenState,
				CustomEffect = null,
				TransformMatrix = Main.UIScaleMatrix,
				ScissorRectangle = Main.instance.GraphicsDevice.ScissorRectangle
			});
			set => _immediateState = value;
		}

		private static SpriteBatchState _pointClampState;

		public static SpriteBatchState PointClampState
		{
			get => _pointClampState ?? (_pointClampState = new SpriteBatchState
			{
				SpriteSortMode = SpriteSortMode.Immediate,
				BlendState = BlendState.AlphaBlend,
				SamplerState = SamplerState.PointClamp,
				DepthStencilState = DepthStencilState.None,
				RasterizerState = OverflowHiddenState,
				CustomEffect = null,
				TransformMatrix = Main.UIScaleMatrix,
				ScissorRectangle = Main.instance.GraphicsDevice.ScissorRectangle
			});
			set => _pointClampState = value;
		}

		public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
		{
			spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(state.ScissorRectangle.X, state.ScissorRectangle.Y, Clamp(state.ScissorRectangle.Width, 0, Main.screenWidth), Clamp(state.ScissorRectangle.Height, 0, Main.screenHeight));
			spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.CustomEffect, state.TransformMatrix);
		}

		public static SpriteBatchState End(this SpriteBatch spriteBatch)
		{
			SpriteBatchState state = spriteBatch.GetState();
			spriteBatch.End();
			return state;
		}

		#region Draw
		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color? color = null)
		{
			spriteBatch.Draw(texture, position, color ?? Color.White);
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color? color = null)
		{
			spriteBatch.Draw(texture, rectangle, color ?? Color.White);
		}

		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, CalculatedStyle dimensions, Color? color = null)
		{
			spriteBatch.Draw(texture, dimensions.ToRectangle(), color);
		}

		public static void Draw(this SpriteBatch spriteBatch, SpriteBatchState state, Action action)
		{
			SpriteBatchState oldState = End(spriteBatch);

			Rectangle oldScissor = state.ScissorRectangle;
			state.ScissorRectangle = oldState.ScissorRectangle;

			spriteBatch.Begin(state);
			action();
			spriteBatch.End();
			state.ScissorRectangle = oldScissor;
			spriteBatch.Begin(oldState);
		}

		public static void Draw(this SpriteBatch spriteBatch, SpriteBatchState state, Rectangle scissorRectangle, Action action)
		{
			SpriteBatchState oldState = End(spriteBatch);

			Rectangle prevRect = state.ScissorRectangle;
			state.ScissorRectangle = Rectangle.Intersect(scissorRectangle, oldState.ScissorRectangle);

			spriteBatch.Begin(state);
			action();
			spriteBatch.End();
			state.ScissorRectangle = prevRect;
			spriteBatch.Begin(oldState);
		}
		#endregion

		#region DrawPanel
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
			spriteBatch.DrawPanel(rectangle, BaseLibrary.texturePanelBackground, bgColor ?? ColorPanel);
			spriteBatch.DrawPanel(rectangle, BaseLibrary.texturePanelBorder, borderColor ?? Color.Black);
		}

		public static void DrawPanel(this SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? bgColor = null, Color? borderColor = null)
		{
			spriteBatch.DrawPanel(dimensions.ToRectangle(), bgColor, borderColor);
		}
		#endregion

		#region DrawSlot
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

		public static void DrawSlot(this SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? color = null, Texture2D texture = null)
		{
			spriteBatch.DrawSlot(dimensions.ToRectangle(), color, texture);
		}
		#endregion

		#region DrawLine
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
		#endregion

		#region DrawMouseText
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
		#endregion

		#region DrawEntity
		public static void DrawEntity(this SpriteBatch spriteBatch, Entity entity, Vector2 position, Vector2 size)
		{
			switch (entity)
			{
				case Item item:
					spriteBatch.DrawItemInInventory(item, position, size);
					break;
				case NPC npc:
					spriteBatch.DrawNPC(npc, position, size);
					break;
				case Projectile projectile:
					spriteBatch.DrawProjectile(projectile, position, size);
					break;
			}
		}

		public static void DrawItemInInventory(this SpriteBatch spriteBatch, Item item, Vector2 position, Vector2 size, bool drawStackSize = true)
		{
			if (!item.IsAir)
			{
				Texture2D itemTexture = Main.itemTexture[item.type];
				Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);

				float availableWidth = size.X;
				int width = rect.Width;
				int height = rect.Height;
				float scale = 1f;
				if (width > availableWidth || height > availableWidth)
				{
					if (width > height) scale = availableWidth / width;
					else scale = availableWidth / height;
				}

				Vector2 origin = rect.Size() * 0.5f;
				float totalScale = pulseScale * scale;

				if (ItemLoader.PreDrawInInventory(item, spriteBatch, position, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, totalScale))
				{
					spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), 0f, origin, totalScale, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), 0f, origin, totalScale, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInInventory(item, spriteBatch, position, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, totalScale);
				if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f), new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				if (drawStackSize && item.stack > 1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
			}
		}

		public static void DrawItemInWorld(this SpriteBatch spriteBatch, Item item, Vector2 position, Vector2 size, float rotation = 0f)
		{
			if (!item.IsAir)
			{
				Texture2D itemTexture = Main.itemTexture[item.type];
				Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);

				float availableWidth = size.X;
				int width = rect.Width;
				int height = rect.Height;
				float drawScale = 1f;
				if (width > availableWidth || height > availableWidth)
				{
					if (width > height) drawScale = availableWidth / width;
					else drawScale = availableWidth / height;
				}

				Vector2 origin = rect.Size() * 0.5f;

				float totalScale = pulseScale * drawScale;

				if (ItemLoader.PreDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), ref rotation, ref totalScale, item.whoAmI))
				{
					spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), rotation, origin, totalScale, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), rotation, origin, totalScale, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInWorld(item, spriteBatch, item.GetColor(Color.White), item.GetAlpha(newColor), rotation, totalScale, item.whoAmI);
				if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f), new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
			}
		}

		public static void DrawNPC(this SpriteBatch spriteBatch, NPC npc, Vector2 position, Vector2 size)
		{
			spriteBatch.Draw(PointClampState, () =>
			{
				Main.instance.LoadNPC(npc.type);

				Color color = npc.color != Color.Transparent ? npc.color : Color.White;

				if (npc.boss)
				{
					Texture2D npcTexture = Main.npcHeadBossTexture[npc.GetBossHeadTextureIndex()];
					spriteBatch.Draw(npcTexture, position, null, color, 0, npcTexture.Size() * 0.5f, Math.Min(size.X / npcTexture.Width, size.Y / npcTexture.Height), SpriteEffects.None, 0);
				}
				else
				{
					Texture2D npcTexture = Main.npcTexture[npc.type];
					Rectangle rectangle = new Rectangle(0, 0, Main.npcTexture[npc.type].Width, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]);

					spriteBatch.Draw(npcTexture, position, rectangle, color, 0, rectangle.Size() * 0.5f, Math.Min(size.X / rectangle.Width, size.Y / rectangle.Height), SpriteEffects.None, 0);
				}
			});
		}

		public static void DrawProjectile(this SpriteBatch spriteBatch, Projectile proj, Vector2 position, Vector2 size)
		{
			spriteBatch.Draw(PointClampState, () =>
			{
				Main.instance.LoadProjectile(proj.type);

				Texture2D projTexture = Main.projectileTexture[proj.type];

				spriteBatch.Draw(projTexture, position, null, Color.White, 0, projTexture.Size() * 0.5f, Math.Min(size.X / projTexture.Width, size.Y / projTexture.Height), SpriteEffects.None, 0);
			});
		}
		#endregion

		private static SpriteBatchState GetState(this SpriteBatch spriteBatch) => new SpriteBatchState
		{
			SpriteSortMode = Platform.IsWindows ? typeof(SpriteBatch).GetValue<SpriteSortMode>("spriteSortMode", spriteBatch) : typeof(SpriteBatch).GetValue<SpriteSortMode>("sortMode", spriteBatch),
			BlendState = typeof(SpriteBatch).GetValue<BlendState>("blendState", spriteBatch),
			SamplerState = typeof(SpriteBatch).GetValue<SamplerState>("samplerState", spriteBatch),
			DepthStencilState = typeof(SpriteBatch).GetValue<DepthStencilState>("depthStencilState", spriteBatch),
			RasterizerState = typeof(SpriteBatch).GetValue<RasterizerState>("rasterizerState", spriteBatch),
			CustomEffect = typeof(SpriteBatch).GetValue<Effect>("customEffect", spriteBatch),
			TransformMatrix = typeof(SpriteBatch).GetValue<Matrix>("transformMatrix", spriteBatch),
			ScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle
		};

		public static void DrawOverflowHidden(this SpriteBatch spriteBatch, UIElement element, Action action)
		{
			ImmediateState.ScissorRectangle = Rectangle.Intersect(element.GetClippingRectangle(spriteBatch), spriteBatch.GraphicsDevice.ScissorRectangle);
			spriteBatch.Draw(ImmediateState, action);
		}

		public static void DrawWithEffect(this SpriteBatch spriteBatch, Effect effect, Action action)
		{
			ImmediateState.CustomEffect = effect;
			spriteBatch.Draw(ImmediateState, action);
			ImmediateState.CustomEffect = null;
		}

		public static Texture2D GetTexture(this Entity entity)
		{
			Texture2D texture = null;

			switch (entity)
			{
				case Item item:
					Texture2D itemTexture = Main.itemTexture[item.type];
					Rectangle rectangle = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();

					texture = new Texture2D(Main.graphics.GraphicsDevice, rectangle.Width, rectangle.Height);
					Color[] data = new Color[rectangle.Width * rectangle.Height];
					itemTexture.GetData(0, rectangle, data, 0, data.Length);
					texture.SetData(data);

					break;
				case NPC npc:
					break;
				case Projectile projectile:
					break;
			}

			return texture;
		}
	}
}