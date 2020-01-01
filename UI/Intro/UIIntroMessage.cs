using BaseLibrary.UI.Elements;
using BaseLibrary.UI.New;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BaseLibrary.UI.Intro
{
	internal class UIIntroMessage : BaseState
	{
		private HttpClient client;

		private List<GithubRepository> GithubRepositories = new List<GithubRepository>();

		private List<PatreonPatron> PatreonPatrons = new List<PatreonPatron>();
		private List<PatreonTier> PatreonTiers = new List<PatreonTier>();
		private List<PatreonGoal> PatreonGoals = new List<PatreonGoal>();

		private int goalIndex;
		private int repositoryIndex;

		private UILoadingWheel loadingWheelChangelogs;
		private Elements.UIText textCurrentRepository;
		private UIMultilineText textCommits;
		private UITextButton buttonMain, buttonChangelogs, buttonCredits;
		private Elements.UIPanel panelMain;
		private UITextButton buttonReturnToMenu;

		private Elements.UIGrid<Elements.BaseElement> gridPatrons;

		private Elements.UIText textProgress;
		private UIMultilineText textGoal;
		private UIRoundedBar sliderProgress;

		private List<Elements.BaseElement> tabMain;
		private List<Elements.BaseElement> tabChangelogs;
		private List<Elements.BaseElement> tabCredits;

		public override async void OnInitialize()
		{
			Texture2D dividerTexture = ModContent.GetTexture("Terraria/UI/Divider");

			client = new HttpClient
			{
				BaseAddress = new Uri("http://localhost:59035/")
			};
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

			HttpResponseMessage response = await client.GetAsync("patreon/getgoals");
			if (response.IsSuccessStatusCode) PatreonGoals = await response.Content.ReadAsAsync<List<PatreonGoal>>();

			panelMain = new Elements.UIPanel
			{
				Width = (0, 0.4f),
				Height = (0, 0.6f),
				Padding = (12, 12, 12, 12),
				HAlign = 0.5f,
				VAlign = 0.6f
			};
			Append(panelMain);

			float width = panelMain.Dimensions.Width * 0.31f;
			buttonMain = new UITextButton(Language.GetText("Mods.BaseLibrary.UI.Main"))
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

			buttonChangelogs = new UITextButton(Language.GetText("Mods.BaseLibrary.UI.Changelogs"))
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

				response = await client.GetAsync("github/getrepositories");
				if (response.IsSuccessStatusCode) GithubRepositories = (await response.Content.ReadAsAsync<List<GithubRepository>>()).Where(repository => ModLoader.GetMod(repository.Name) != null).ToList();

				if (GithubRepositories.Count == 0) return;

				await SetRepository(GithubRepositories[0]);
			};
			Insert(1, buttonChangelogs);

			buttonCredits = new UITextButton(Language.GetText("Mods.BaseLibrary.UI.Credits"))
			{
				Width = (width, 0),
				Height = (40, 0),
				Left = (panelMain.Position.X + panelMain.Dimensions.Width - width - 8f, 0),
				Top = (panelMain.Position.Y - 30, 0),
				Padding = (12, 8, 8, 4),
				Toggleable = true
			};
			buttonCredits.OnClick += async (evt, element) =>
			{
				if (PatreonTiers.Count == 0 && PatreonPatrons.Count == 0)
				{
					response = await client.GetAsync("patreon/gettiers");
					if (response.IsSuccessStatusCode) PatreonTiers = await response.Content.ReadAsAsync<List<PatreonTier>>();

					response = await client.GetAsync("patreon/getpatrons");
					if (response.IsSuccessStatusCode) PatreonPatrons = await response.Content.ReadAsAsync<List<PatreonPatron>>();

					foreach (PatreonPatron patron in PatreonPatrons.Where(patron => patron.Status != null)) patron.Tier = PatreonTiers.OrderByDescending(x => x.AmountCents).First(x => x.AmountCents <= patron.CurrentCents);

					foreach (PatreonTier tier in PatreonTiers)
					{
						UITextButton buttonTier = new UITextButton(tier.Title)
						{
							Width = (0, 1),
							Height = (40, 0),
							Toggleable = true,
							MarginTop = 8
						};
						buttonTier.SetPadding(6);
						buttonTier.OnClick += (a, b) =>
						{
							int index = gridPatrons.Items.IndexOf(buttonTier) + 1;
							if (index < gridPatrons.Items.Count && gridPatrons.Items[index] is UIMultilineText text) gridPatrons.Remove(text);
							else
							{
								string t = "No patrons";
								float height = 40f;
								List<PatreonPatron> valid = PatreonPatrons.Where(patron => patron.Tier == tier).ToList();
								if (valid.Count > 0)
								{
									t = valid.Select(patron => $"{patron.Name} donated ${patron.LifetimeCents / 100}").Aggregate((x, y) => x + "\n" + y);
									height = valid.Count * 24f + 16f;
								}

								UIMultilineText textPatrons = new UIMultilineText(t)
								{
									Width = (0, 1),
									Height = (height, 0),
									DrawBackground = true,
									Padding = (8, 8, 8, 8),
									MarginTop = -2
								};
								gridPatrons.Insert(index, textPatrons);
							}
						};
						gridPatrons.Add(buttonTier);
					}
				}

				buttonChangelogs.Selected = buttonMain.Selected = false;

				panelMain.RemoveAllChildren();
				panelMain.Append(tabCredits);
			};
			Insert(2, buttonCredits);

			buttonReturnToMenu = new UITextButton(Language.GetText("Mods.BaseLibrary.UI.ReturnToMenu"))
			{
				Width = (0, 0.2f),
				Height = (40, 0),
				Top = (panelMain.Dimensions.Y + panelMain.Dimensions.Height + 8, 0),
				HAlign = 0.5f
			};
			buttonReturnToMenu.OnClick += (evt, element) => Main.menuMode = 0;
			Append(buttonReturnToMenu);

			tabMain = new List<Elements.BaseElement>();
			{
				UIMultilineText textIntro = new UIMultilineText(client.GetStringAsync("misc/gettext").Result)
				{
					Width = (0, 1),
					Height = (0, 0.5f),
					Padding = (8, 8, 8, 8),
					DrawBackground = true
				};
				tabMain.Add(textIntro);

				// Goals
				{
					Elements.UIPanel panelGoals = new Elements.UIPanel
					{
						Width = (0, 0.5f),
						Height = (0, 0.5f),
						Top = (0, 0.5f),
						Padding = (8, 8, 8, 8),
						BorderColor = Color.Transparent,
						BackgroundColor = Utility.ColorPanel_Selected * 0.75f
					};
					tabMain.Add(panelGoals);

					Elements.UIText textGoals = new Elements.UIText("Goals", 1.2f)
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Width = (-76, 1),
						Height = (30, 0)
					};
					textGoals.OnPostDraw += spritebatch => spritebatch.Draw(dividerTexture, new Rectangle((int)textGoals.Position.X - 6, (int)textGoals.Position.Y + 32, (int)panelGoals.Size.X - 4, 4));
					panelGoals.Append(textGoals);

					UITextButton buttonNext = new UITextButton(">")
					{
						Size = new Vector2(30),
						HAlign = 1,
						Padding = (0, 0, 0, 0)
					};
					buttonNext.OnClick += (evt, element) =>
					{
						if (goalIndex < PatreonGoals.Count - 1)
						{
							goalIndex++;

							PatreonGoal goal = PatreonGoals[goalIndex];
							textProgress.Text = $"${goal.CompletedPercentage * goal.AmountCents * 0.0001f:N0} of ${goal.AmountCents * 0.01f} per month";
							textGoal.Text = goal.Description;
							sliderProgress.Progress = goal.CompletedPercentage;
						}
					};
					panelGoals.Append(buttonNext);

					UITextButton buttonPrev = new UITextButton("<")
					{
						Size = new Vector2(30),
						Left = (-64, 1),
						Padding = (0, 0, 0, 0)
					};
					buttonPrev.OnClick += (evt, element) =>
					{
						if (goalIndex > 0)
						{
							goalIndex--;

							PatreonGoal goal = PatreonGoals[goalIndex];
							textProgress.Text = $"${goal.CompletedPercentage * goal.AmountCents * 0.0001f:N0} of ${goal.AmountCents * 0.01f} per month";
							textGoal.Text = goal.Description;
							sliderProgress.Progress = goal.CompletedPercentage;
						}
					};
					panelGoals.Append(buttonPrev);

					textProgress = new Elements.UIText($"${PatreonGoals[goalIndex].CompletedPercentage * PatreonGoals[goalIndex].AmountCents * 0.0001f:N0} of ${PatreonGoals[goalIndex].AmountCents * 0.01f} per month")
					{
						Width = (0, 1),
						Height = (20, 0),
						Top = (40, 0)
					};
					panelGoals.Append(textProgress);

					textGoal = new UIMultilineText(PatreonGoals[goalIndex].Description)
					{
						Width = (0, 1),
						Height = (-84, 1),
						Top = (84, 0)
					};
					panelGoals.Append(textGoal);

					sliderProgress = new UIRoundedBar
					{
						Width = (0, 1),
						Height = (8, 0),
						Top = (68, 0),
						Progress = PatreonGoals[goalIndex].CompletedPercentage
					};
					panelGoals.Append(sliderProgress);
				}

				// Links
				{
					Elements.UIPanel panelLinks = new Elements.UIPanel
					{
						Width = (0, 0.5f),
						Height = (0, 0.5f),
						Top = (0, 0.5f),
						HAlign = 1f,
						Padding = (8, 8, 8, 8),
						BorderColor = Color.Transparent,
						BackgroundColor = Utility.ColorPanel_Selected * 0.75f
					};
					tabMain.Add(panelLinks);

					// Patreon
					{
						Elements.BaseElement containerPatreon = new Elements.BaseElement
						{
							Width = (0, 1),
							Height = (32, 0)
						};
						containerPatreon.OnClick += (evt, element) => Process.Start("https://www.patreon.com/Itorius");
						panelLinks.Append(containerPatreon);

						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"), ScaleMode.Stretch)
						{
							Width = (32, 0),
							Height = (32, 0)
						};
						containerPatreon.Append(texture);

						Elements.UIText text = new Elements.UIText("Donate on Patreon!")
						{
							Width = (-40, 1),
							Left = (40, 0),
							Height = (28, 0),
							VerticalAlignment = VerticalAlignment.Center
						};
						containerPatreon.Append(text);
					}

					// TCF
					{
						Elements.BaseElement containerTCF = new Elements.BaseElement
						{
							Width = (0, 1),
							Height = (32, 0),
							Top = (40, 0)
						};
						containerTCF.OnClick += (evt, element) => Process.Start("https://forums.terraria.org/index.php?threads/itorius-mods.42289/");
						panelLinks.Append(containerTCF);

						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/TCFLogo"), ScaleMode.Stretch)
						{
							Width = (32, 0),
							Height = (32, 0)
						};
						containerTCF.Append(texture);

						Elements.UIText text = new Elements.UIText("Visit my TCF page!")
						{
							Width = (-40, 1),
							Left = (40, 0),
							Height = (28, 0),
							VerticalAlignment = VerticalAlignment.Center
						};
						containerTCF.Append(text);
					}

					// Github
					{
						Elements.BaseElement containerGithub = new Elements.BaseElement
						{
							Width = (0, 1),
							Height = (32, 0),
							Top = (80, 0)
						};
						containerGithub.OnClick += (evt, element) => Process.Start("https://github.com/Eternal-Team");
						panelLinks.Append(containerGithub);

						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/GithubLogo"), ScaleMode.Stretch)
						{
							Width = (32, 0),
							Height = (32, 0)
						};
						containerGithub.Append(texture);

						Elements.UIText text = new Elements.UIText("Check out my code!")
						{
							Width = (-40, 1),
							Left = (40, 0),
							Height = (28, 0),
							VerticalAlignment = VerticalAlignment.Center
						};
						containerGithub.Append(text);
					}
				}
			}

			tabChangelogs = new List<Elements.BaseElement>();
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
					if (GithubRepositories.Count == 0) return;

					if (--repositoryIndex < 0) repositoryIndex = GithubRepositories.Count - 1;

					await SetRepository(GithubRepositories[repositoryIndex]);
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
					if (GithubRepositories.Count == 0) return;

					if (++repositoryIndex >= GithubRepositories.Count) repositoryIndex = 0;

					await SetRepository(GithubRepositories[repositoryIndex]);
				};
				tabChangelogs.Add(buttonNext);

				textCurrentRepository = new Elements.UIText("Loading...", 1.5f)
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

			tabCredits = new List<Elements.BaseElement>();
			{
				gridPatrons = new Elements.UIGrid<Elements.BaseElement>
				{
					Width = (0, 1),
					Height = (0, 1),
					ListPadding = 0
				};
				tabCredits.Add(gridPatrons);
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

		private async Task SetRepository(GithubRepository repository)
		{
			loadingWheelChangelogs.Enabled = true;
			textCommits.Text = "";

			textCurrentRepository.Text = repository.Name;

			HttpResponseMessage response = await client.GetAsync("github/getreleases/" + repository.Id);
			if (response.IsSuccessStatusCode)
			{
				var releases = await response.Content.ReadAsAsync<List<GithubRelease>>();
				if (releases.Count == 0)
				{
					textCommits.Text = $"No releases found for mod {repository.Name}";
					loadingWheelChangelogs.Enabled = false;
					return;
				}

				GithubRelease currentRelease = releases[0];
				GithubRelease previousRelease = releases.Skip(1).Take(1).FirstOrDefault();

				if (previousRelease == null) response = await client.GetAsync($"github/getcommits/{repository.Id}");
				else response = await client.GetAsync($"github/getcommits/{repository.Id}/{previousRelease.Id}/{currentRelease.Id}");

				if (response.IsSuccessStatusCode)
				{
					var commits = await response.Content.ReadAsAsync<List<GithubCommit>>();
					if (commits.Count == 0)
					{
						textCommits.Text = $"No commits found for release {currentRelease.TagName}";
						loadingWheelChangelogs.Enabled = false;
						return;
					}

					textCommits.Text = $"Changelogs for version {currentRelease.TagName}\n\n{commits.Select(commit => commit.Commit.Message).Concat("\n")}";
				}
			}

			loadingWheelChangelogs.Enabled = false;
		}
	}
}