using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using Terraria;

namespace BaseLibrary.UI;

public struct UIVideoSettings
{
	public static readonly UIVideoSettings Default = new() {
		ScaleMode = ScaleMode.None,
		LoopVideo = true
	};

	public ScaleMode ScaleMode;
	public bool LoopVideo;
}

public class UIVideo : BaseElement
{
	public UIVideoSettings Settings = UIVideoSettings.Default;

	private readonly VideoPlayer videoPlayer;
	private Asset<Video>? video;
	private bool pendingResize;

	public UIVideo(Asset<Video> video)
	{
		videoPlayer ??= new VideoPlayer();
		SetVideo(video);

		videoPlayer.Stop();
		if (Settings.LoopVideo)
		{
			videoPlayer.IsLooped = true;
			videoPlayer.Play(video.Value);
		}
	}

	public override void Recalculate()
	{
		if (video is not null)
		{
			Size.PercentY = 0;
			Size.PixelsY = video.Value.Height;
			Size.PercentX = 0;
			Size.PixelsX = video.Value.Width;
		}

		base.Recalculate();
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		if (video is null) return;

		Texture2D? frameTexture = videoPlayer.GetTexture();

		if (Settings.ScaleMode == ScaleMode.Stretch)
		{
			spriteBatch.Draw(frameTexture, InnerDimensions, Color.White);
		}
		else if (Settings.ScaleMode == ScaleMode.None)
		{
			Vector2 size = new Vector2(video.Value.Width, video.Value.Height);
			Vector2 position = Dimensions.TopLeft();
			
			spriteBatch.Draw(frameTexture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			DrawingUtility.DrawAchievementBorder(spriteBatch, position, size);
		}
	}

	public void SetVideo(Asset<Video> video)
	{
		this.video = video;
	}
}