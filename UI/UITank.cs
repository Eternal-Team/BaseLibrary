// using BaseLibrary.Utility;
// using FluidLibrary;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria;
// using Terraria.GameContent;
// using Terraria.ModLoader;
//
// namespace BaseLibrary.UI;
//
// public struct UITankSettings
// {
// 	public static readonly UITankSettings Default = new()
// 	{
// 		AllowMouseInteraction = true
// 	};
//
// 	public bool AllowMouseInteraction;
// }
//
// [ExtendsFromMod("FluidLibrary")]
// public class UITank : BaseElement
// {
// 	private FluidStorage storage;
// 	private int tank;
//
// 	public UITankSettings Settings = UITankSettings.Default;
//
// 	public FluidStack FluidStack => storage[tank];
//
// 	public UITank(FluidStorage itemStorage, int tank)
// 	{
// 		Width.Pixels = 44;
// 		Height.Pixels = 44;
//
// 		this.tank = tank;
// 		storage = itemStorage;
// 	}
//
// 	protected override void MouseDown(MouseButtonEventArgs args)
// 	{
// 		if (!Settings.AllowMouseInteraction && args.Button != MouseButton.Left && args.Button != MouseButton.Right)
// 			return;
//
// 		args.Handled = true;
//
// 		if (Main.mouseItem.IsAir || Main.mouseItem.ModItem is not IFluidStorage mouseStorage)
// 			return;
//
// 		if (args.Button == MouseButton.Left)
// 		{
// 			storage.TransferTo(Main.LocalPlayer, mouseStorage.GetFluidStorage(), tank);
// 		}
// 		else if (args.Button == MouseButton.Right)
// 		{
// 			mouseStorage.GetFluidStorage().TransferTo(Main.LocalPlayer, storage, 0 /* todo: it should try all tanks*/);
// 		}
// 	}
//
// 	public override int CompareTo(BaseElement other) => other is UITank uiSlot ? tank.CompareTo(uiSlot.tank) : 0;
//
// 	protected override void Draw(SpriteBatch spriteBatch)
// 	{
// 		DepthStencilState stencilMask = new DepthStencilState
// 		{
// 			StencilEnable = true,
// 			StencilFunction = CompareFunction.Always,
// 			StencilPass = StencilOperation.Replace,
// 			ReferenceStencil = 1,
// 			DepthBufferEnable = false
// 		};
//
// 		DepthStencilState stencilLiquid = new DepthStencilState
// 		{
// 			StencilEnable = true,
// 			StencilFunction = CompareFunction.LessEqual,
// 			StencilPass = StencilOperation.Keep,
// 			ReferenceStencil = 1,
// 			DepthBufferEnable = false
// 		};
//
// 		RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };
//
// 		var projection = Matrix.CreateOrthographicOffCenter(0,
// 			Main.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth / Main.UIScale,
// 			Main.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight / Main.UIScale,
// 			0, 0, 1
// 		);
//
// 		var shader = new AlphaTestEffect(Main.graphics.GraphicsDevice)
// 		{
// 			Projection = projection
// 		};
//
// 		var texture = TextureAssets.InventoryBack.Value;
//
// 		// Draw slot to color and stencil buffer
// 		spriteBatch.End();
// 		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, stencilMask, rasterizerState, shader, Main.UIScaleMatrix);
// 		DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);
// 		spriteBatch.End();
//
// 		if (FluidStack.Fluid is not null)
// 		{
// 			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, stencilLiquid, rasterizerState, null, Main.UIScaleMatrix);
//
// 			Texture2D fluidTexture = ModContent.Request<Texture2D>(FluidStack.Fluid.Texture).Value;
//
// 			float fill = FluidStack.Volume / (float)storage.MaxVolumeFor(tank);
//
// 			// Draw fluid (use stencil)
// 			int height = (int)(Dimensions.Height * fill);
// 			spriteBatch.Draw(fluidTexture, new Rectangle((int)Dimensions.BottomLeft().X, (int)Dimensions.BottomLeft().Y - height, Dimensions.Width, height), Color.White);
//
// 			spriteBatch.End();
// 		}
//
// 		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizerState, shader, Main.UIScaleMatrix);
//
// 		// Draw border of slot
// 		DrawingUtility.DrawSlot(spriteBatch, Dimensions, ModContent.Request<Texture2D>(BaseLibrary.AssetPath + "Textures/UI/SlotBorder").Value, Color.White);
//
// 		if (IsMouseHovering && HoveredElement == this)
// 		{
// 			// todo: draw BG
// 			
// 			if (FluidStack.Fluid is not null)
// 				Main.instance.MouseText($"Storing {FluidStack.Volume / 255f:0.#}/{storage.MaxVolumeFor(tank) / 255f:0.#} of {FluidStack.Fluid.DisplayName.Get()}\n" +
// 				                        "Left-click with a fluid container to fill it\n" +
// 				                        "Right-click with a fluid container to empty it");
// 			else
// 				Main.instance.MouseText("Empty\n" +
// 				                        "Left-click with a fluid container to fill it\n" +
// 				                        "Right-click with a fluid container to empty it");
// 		}
// 	}
// }