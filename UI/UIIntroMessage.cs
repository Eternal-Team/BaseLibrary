using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary.UI
{
	public class UIIntroMessage : BaseState
	{
		private UILoadingWheel loadingWheelChangelogs;
		private UIText textCurrentRepository;
		private UIMultilineText textCommits;
		private UITextButton buttonMain, buttonChangelogs, buttonCredits;
		private UIPanel panelMain;
		private UITextButton buttonReturnToMenu;

		private List<BaseElement> tabMain;
		private List<BaseElement> tabChangelogs;
		private List<BaseElement> tabCredits;

		private List<Repository> repositories;
		private int index;

		private Lazy<CommitRequest> _commitRequest = new Lazy<CommitRequest>(() => new CommitRequest());
		private CommitRequest Request => _commitRequest.Value;

		private Lazy<GitHubClient> _client = new Lazy<GitHubClient>(() => new GitHubClient(new ProductHeaderValue("EternalTeam-Changelogs"))
		{
			Credentials = new Credentials(Encoding.UTF8.GetString(BaseLibrary.Instance.GetFileBytes("secret.txt")))
		});

		private GitHubClient Client => _client.Value;

		public override void OnInitialize()
		{
			panelMain = new UIPanel
			{
				Width = (0, 0.4f),
				Height = (0, 0.6f),
				Padding = (12, 12, 12, 12),
				HAlign = 0.5f,
				VAlign = 0.6f
			};
			Append(panelMain);

			float width = panelMain.Dimensions.Width * 0.31f;
			buttonMain = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Main"))
			{
				Width = (width, 0),
				Height = (40, 0),
				Left = (panelMain.Position.X + 8f, 0),
				Top = (panelMain.Position.Y - 30, 0),
				Padding = (12, 8, 8, 4),
				Selected = true,
				Toggleable = true
			};
			buttonMain.OnClick += (evt, element) =>
			{
				buttonChangelogs.Selected = buttonCredits.Selected = false;

				panelMain.RemoveAllChildren();
				panelMain.Append(tabMain);
			};
			Insert(0, buttonMain);

			buttonChangelogs = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Changelogs"))
			{
				Width = (width, 0),
				Height = (40, 0),
				HAlign = 0.5f,
				Top = (panelMain.Position.Y - 30, 0),
				Padding = (12, 8, 8, 4),
				Toggleable = true
			};
			buttonChangelogs.OnClick += async (evt, element) =>
			{
				buttonMain.Selected = buttonCredits.Selected = false;

				panelMain.RemoveAllChildren();
				panelMain.Append(tabChangelogs);

				repositories = (await Client.Repository.GetAllForOrg("Eternal-Team")).Where(repository => ModLoader.GetMod(repository.Name) != null).ToList();
				if (repositories.Count == 0) return;

				await SetRepository(repositories[0]);
			};
			Insert(1, buttonChangelogs);

			buttonCredits = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Credits"))
			{
				Width = (width, 0),
				Height = (40, 0),
				Left = (panelMain.Position.X + panelMain.Dimensions.Width - width - 8f, 0),
				Top = (panelMain.Position.Y - 30, 0),
				Padding = (12, 8, 8, 4),
				Toggleable = true
			};
			buttonCredits.OnClick += (evt, element) =>
			{
				buttonChangelogs.Selected = buttonMain.Selected = false;

				panelMain.RemoveAllChildren();
				panelMain.Append(tabCredits);
			};
			Insert(2, buttonCredits);

			buttonReturnToMenu = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.ReturnToMenu"))
			{
				Width = (0, 0.2f),
				Height = (40, 0),
				Top = (panelMain.Dimensions.Y + panelMain.Dimensions.Height + 8, 0),
				HAlign = 0.5f
			};
			buttonReturnToMenu.OnClick += (evt, element) => Main.menuMode = 0;
			Append(buttonReturnToMenu);

			tabMain = new List<BaseElement>();
			{
				UIMultilineText introText = new UIMultilineText(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.IntroMessage"))
				{
					Width = (0, 1),
					Height = (-68, 1),
					DrawBackground = true
				};
				introText.SetPadding(8);
				tabMain.Add(introText);

				UITexture texturePatreon = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"))
				{
					Width = (235, 0),
					Height = (60, 0),
					Top = (-60, 1),
					HAlign = 0.5f
				};
				texturePatreon.OnClick += (evt, element) => Process.Start("https://www.patreon.com/Itorius");
				texturePatreon.OnPostDraw += _ =>
				{
					if (texturePatreon.IsMouseHovering)
					{
						Main.instance.MouseTextHackZoom("https://www.patreon.com/Itorius");
						Main.mouseText = true;
					}
				};
				tabMain.Add(texturePatreon);
			}

			tabChangelogs = new List<BaseElement>();
			{
				loadingWheelChangelogs = new UILoadingWheel(0.75f)
				{
					HAlign = 0.5f,
					VAlign = 0.5f
				};
				tabChangelogs.Add(loadingWheelChangelogs);

				UITextButton buttonPrevious = new UITextButton("<")
				{
					Width = (50, 0),
					Height = (40, 0),
					PaddingBottom = 4f,
					PaddingTop = 4f
				};
				buttonPrevious.OnClick += async (evt, element) =>
				{
					if (repositories.Count == 0) return;

					if (--index < 0) index = repositories.Count - 1;

					await SetRepository(repositories[index]);
				};
				tabChangelogs.Add(buttonPrevious);

				UITextButton buttonNext = new UITextButton(">")
				{
					Width = (50, 0),
					Height = (40, 0),
					HAlign = 1,
					PaddingBottom = 4f,
					PaddingTop = 4f
				};
				buttonNext.OnClick += async (evt, element) =>
				{
					if (repositories.Count == 0) return;

					if (++index >= repositories.Count) index = 0;

					await SetRepository(repositories[index]);
				};
				tabChangelogs.Add(buttonNext);

				textCurrentRepository = new UIText("Loading...", 1.5f)
				{
					Width = (-116, 1),
					Height = (40, 0),
					HAlign = 0.5f,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					ScaleToFit = true
				};
				tabChangelogs.Add(textCurrentRepository);

				textCommits = new UIMultilineText("")
				{
					Width = (-28, 1),
					Height = (-48, 1),
					Top = (48, 0),
					DrawBackground = true
				};
				textCommits.SetPadding(8);
				tabChangelogs.Add(textCommits);

				textCommits.scrollbar.Height = (-64, 1);
				textCommits.scrollbar.Top = (56, 0);
				textCommits.scrollbar.HAlign = 1;
				tabChangelogs.Add(textCommits.scrollbar);
			}

			tabCredits = new List<BaseElement>();
			{
				UIText text = new UIText("Can't get this working until I can afford a VPS")
				{
					HAlign = 0.5f,
					VAlign = 0.5f,
					Width = (0, 1),
					Height = (0, 1),
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				tabCredits.Add(text);
			}

			panelMain.Append(tabMain);
		}

		public override void Recalculate()
		{
			base.Recalculate();

			if (panelMain == null) return;

			CalculatedStyle dimensions = panelMain.Dimensions;
			Vector2 position = panelMain.Position;

			float width = dimensions.Width * 0.31f;
			buttonMain.Left = (position.X + 8f, 0);
			buttonMain.Top = (position.Y - 30, 0);
			buttonMain.Width = (width, 0);
			buttonMain.Recalculate();

			buttonChangelogs.Top = (position.Y - 30, 0);
			buttonChangelogs.Width = (width, 0);
			buttonChangelogs.Recalculate();

			buttonCredits.Left = (position.X + dimensions.Width - width - 8f, 0);
			buttonCredits.Top = (position.Y - 30, 0);
			buttonCredits.Width = (width, 0);
			buttonCredits.Recalculate();

			buttonReturnToMenu.Top = (dimensions.Y + dimensions.Height + 8, 0);
			buttonReturnToMenu.Recalculate();
		}

		public async Task SetRepository(Repository repository)
		{
			loadingWheelChangelogs.Enabled = true;
			textCommits.Text = "";

			textCurrentRepository.Text = repository.Name;

			var releases = await Client.Repository.Release.GetAll(repository.Id);
			if (releases.Count == 0)
			{
				textCommits.Text = $"No releases found for mod {repository.Name}";
				loadingWheelChangelogs.Enabled = false;
				return;
			}

			Release currentRelease = releases[0];
			Release previousRelease = releases.Skip(1).Take(1).FirstOrDefault();

			Request.Until = currentRelease.CreatedAt;
			if (previousRelease != null) Request.Since = previousRelease.CreatedAt.AddSeconds(1);

			var commits = await Client.Repository.Commit.GetAll(repository.Id, Request);
			if (commits.Count == 0)
			{
				textCommits.Text = $"No commits found for release {currentRelease.TagName}";
				loadingWheelChangelogs.Enabled = false;
				return;
			}

			textCommits.Text = $"Changelogs for version {currentRelease.TagName}\n\n{commits.Select(commit => commit.Commit.Message).Concat("\n")}";

			loadingWheelChangelogs.Enabled = false;
		}
	}
}