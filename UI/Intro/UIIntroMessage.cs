// using BaseLibrary.UI.Elements;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Octokit;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Threading.Tasks;
// using Terraria;
// using Terraria.ModLoader;
//
// namespace BaseLibrary.UI.Intro
// {
// 	internal class UIIntroMessage : BaseState
// 	{
// 		public override bool Enabled => Main.gameMenu && Main.menuMode == 888;
//
// 		private HttpClient client;
//
// 		private List<Repository> GithubRepositories = new List<Repository>();
//
// 		private List<PatreonPatron> PatreonPatrons = new List<PatreonPatron>();
// 		private List<PatreonTier> PatreonTiers = new List<PatreonTier>();
// 		private List<PatreonGoal> PatreonGoals = new List<PatreonGoal>();
//
// 		private int goalIndex;
// 		private int repositoryIndex;
//
// 		private UILoadingWheel loadingWheelChangelogs;
// 		private UIText textCurrentRepository;
// 		private UIMultilineText textCommits;
// 		private UITextButton buttonMain, buttonChangelogs, buttonCredits;
// 		private UIPanel panelMain;
// 		private UITextButton buttonReturnToMenu;
//
// 		private UIGrid<BaseElement> gridPatrons;
//
// 		private UIText textProgress;
// 		private UIMultilineText textGoal;
// 		private UIRoundedBar sliderProgress;
//
// 		private List<BaseElement> tabMain;
// 		private List<BaseElement> tabChangelogs;
// 		private List<BaseElement> tabCredits;
//
// 		public UIIntroMessage()
// 		{
// 			Width.Percent = 100;
// 			Height.Percent = 100;
//
// 			Initialize();
// 		}
//
// 		public async void Initialize()
// 		{
// 			Texture2D dividerTexture = ModContent.GetTexture("Terraria/UI/Divider");
//
// 			client = new HttpClient
// 			{
// 				BaseAddress = new Uri("http://localhost:59035/")
// 			};
// 			client.DefaultRequestHeaders.Accept.Clear();
// 			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
// 			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
//
// 			HttpResponseMessage response = await client.GetAsync("patreon/getgoals");
// 			if (response.IsSuccessStatusCode) PatreonGoals = await response.Content.ReadAsAsync<List<PatreonGoal>>();
//
// 			panelMain = new UIPanel
// 			{
// 				Width = { Percent = 40 },
// 				Height = { Percent = 60 },
// 				Padding = new Padding(12),
// 				X = { Percent = 50 },
// 				Y = { Percent = 60 }
// 			};
// 			Add(panelMain);
//
// 			int width = (int)(panelMain.Dimensions.Width * 0.31f);
// 			buttonMain = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Main"))
// 			{
// 				Width = { Pixels = width },
// 				Height = { Pixels = 40 },
// 				X = { Pixels = panelMain.Dimensions.X + 8 },
// 				Y = { Pixels = panelMain.Dimensions.Y - 30 },
// 				Padding = new Padding(12, 8, 8, 4),
// 				Selected = true,
// 				Toggleable = true
// 			};
// 			buttonMain.OnClick += args =>
// 			{
// 				buttonChangelogs.Selected = buttonCredits.Selected = false;
//
// 				panelMain.Clear();
// 				panelMain.AddRange(tabMain);
// 			};
// 			Insert(0, buttonMain);
//
// 			buttonChangelogs = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Changelogs"))
// 			{
// 				Width = { Pixels = width },
// 				Height = { Pixels = 40 },
// 				X = { Percent = 50 },
// 				Y = { Pixels = panelMain.Dimensions.Y - 30 },
// 				Padding = new Padding(12, 8, 8, 4),
// 				Toggleable = true
// 			};
// 			buttonChangelogs.OnClick += async args =>
// 			{
// 				buttonMain.Selected = buttonCredits.Selected = false;
//
// 				panelMain.Clear();
// 				panelMain.AddRange(tabChangelogs);
//
// 				response = await client.GetAsync("github/getrepositories");
// 				if (response.IsSuccessStatusCode) GithubRepositories = (await response.Content.ReadAsAsync<List<Repository>>()).Where(repository => ModLoader.GetMod(repository.Name) != null).ToList();
//
// 				if (GithubRepositories.Count == 0) return;
//
// 				await SetRepository(GithubRepositories[0]);
// 			};
// 			Insert(1, buttonChangelogs);
//
// 			buttonCredits = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.Credits"))
// 			{
// 				Width = { Pixels = width },
// 				Height = { Pixels = 40 },
// 				X = { Pixels = panelMain.Dimensions.X + panelMain.Dimensions.Width - width - 8 },
// 				Y = { Pixels = panelMain.Dimensions.Y - 30 },
// 				Padding = new Padding(12, 8, 8, 4),
// 				Toggleable = true
// 			};
// 			buttonCredits.OnClick += async args =>
// 			{
// 				if (PatreonTiers.Count == 0 && PatreonPatrons.Count == 0)
// 				{
// 					response = await client.GetAsync("patreon/gettiers");
// 					if (response.IsSuccessStatusCode) PatreonTiers = await response.Content.ReadAsAsync<List<PatreonTier>>();
//
// 					response = await client.GetAsync("patreon/getpatrons");
// 					if (response.IsSuccessStatusCode) PatreonPatrons = await response.Content.ReadAsAsync<List<PatreonPatron>>();
//
// 					foreach (PatreonPatron patron in PatreonPatrons.Where(patron => patron.Status != null)) patron.Tier = PatreonTiers.OrderByDescending(x => x.AmountCents).First(x => x.AmountCents <= patron.CurrentCents);
//
// 					int i = 0;
// 					foreach (PatreonTier tier in PatreonTiers)
// 					{
// 						UITextButton buttonTier = new UITextButton(tier.Title)
// 						{
// 							Width = { Percent = 100 },
// 							Height = { Pixels = 40 },
// 							Toggleable = true,
// 							Margin = i++ == 0 ? Margin.Zero : new Margin(0, 8, 0, 0),
// 							Padding = new Padding(6)
// 						};
// 						buttonTier.OnClick += _ =>
// 						{
// 							int index = gridPatrons.Children.IndexOf(buttonTier) + 1;
// 							if (index < gridPatrons.Children.Count && gridPatrons.Children[index] is UIMultilineText text) gridPatrons.Remove(text);
// 							else
// 							{
// 								string t = "No patrons";
// 								int height = 40;
// 								List<PatreonPatron> valid = PatreonPatrons.Where(patron => patron.Tier == tier).ToList();
// 								if (valid.Count > 0)
// 								{
// 									t = valid.Select(patron => $"{patron.Name} donated ${patron.LifetimeCents / 100}").Aggregate((x, y) => x + "\n" + y);
// 									height = valid.Count * 24 + 16;
// 								}
//
// 								UIMultilineText textPatrons = new UIMultilineText(t)
// 								{
// 									Width = { Percent = 100 },
// 									Height = { Pixels = height },
// 									DrawBackground = true,
// 									Padding = new Padding(8, 8, 8, 8),
// 									Margin = Margin.Zero
// 								};
// 								gridPatrons.Insert(index, textPatrons);
// 							}
// 						};
// 						gridPatrons.Add(buttonTier);
// 					}
// 				}
//
// 				buttonChangelogs.Selected = buttonMain.Selected = false;
//
// 				panelMain.Clear();
// 				panelMain.AddRange(tabCredits);
// 			};
// 			Insert(2, buttonCredits);
//
// 			buttonReturnToMenu = new UITextButton(Terraria.Localization.Language.GetText("Mods.BaseLibrary.UI.ReturnToMenu"))
// 			{
// 				Width = { Percent = 20 },
// 				Height = { Pixels = 40 },
// 				Y = { Pixels = panelMain.Dimensions.Y + panelMain.Dimensions.Height + 8 },
// 				X = { Percent = 50 }
// 			};
// 			buttonReturnToMenu.OnClick += args => Main.menuMode = 0;
// 			Add(buttonReturnToMenu);
//
// 			tabMain = new List<BaseElement>();
// 			{
// 				UIMultilineText textIntro = new UIMultilineText(client.GetStringAsync("misc/gettext").Result)
// 				{
// 					Width = { Percent = 100 },
// 					Height = { Percent = 50 },
// 					Padding = new Padding(8, 8, 8, 8),
// 					DrawBackground = true
// 				};
// 				tabMain.Add(textIntro);
//
// 				// Goals
// 				{
// 					UIPanel panelGoals = new UIPanel
// 					{
// 						Width = { Percent = 50 },
// 						Height = { Percent = 50 },
// 						Y = { Percent = 100 },
// 						Padding = new Padding(8, 8, 8, 8),
// 						BorderColor = Color.Transparent,
// 						BackgroundColor = Utility.ColorPanel_Selected * 0.75f
// 					};
// 					tabMain.Add(panelGoals);
//
// 					UIText textGoals = new UIText("Goals", 1.2f)
// 					{
// 						HorizontalAlignment = HorizontalAlignment.Center,
// 						Width = { Pixels = -76, Percent = 100 },
// 						Height = { Pixels = 30 }
// 					};
// 					//textGoals.OnPostDraw += spritebatch => spritebatch.Draw(dividerTexture, new Rectangle((int)textGoals.Position.X - 6, (int)textGoals.Position.Y + 32, (int)panelGoals.Size.X - 4, 4));
// 					panelGoals.Add(textGoals);
//
// 					UITextButton buttonNext = new UITextButton(">")
// 					{
// 						Size = new Vector2(30),
// 						X = { Percent = 100 },
// 						Padding = Padding.Zero
// 					};
// 					buttonNext.OnClick += args =>
// 					{
// 						if (goalIndex < PatreonGoals.Count - 1)
// 						{
// 							goalIndex++;
//
// 							PatreonGoal goal = PatreonGoals[goalIndex];
// 							textProgress.Text = $"${goal.CompletedPercentage * goal.AmountCents * 0.0001f:N0} of ${goal.AmountCents * 0.01f} per month";
// 							textGoal.Text = goal.Description;
// 							sliderProgress.Progress = goal.CompletedPercentage;
// 						}
// 					};
// 					panelGoals.Add(buttonNext);
//
// 					UITextButton buttonPrev = new UITextButton("<")
// 					{
// 						Size = new Vector2(30),
// 						X = { Pixels = -64, Percent = 100 },
// 						Padding = Padding.Zero
// 					};
// 					buttonPrev.OnClick += args =>
// 					{
// 						if (goalIndex > 0)
// 						{
// 							goalIndex--;
//
// 							PatreonGoal goal = PatreonGoals[goalIndex];
// 							textProgress.Text = $"${goal.CompletedPercentage * goal.AmountCents * 0.0001f:N0} of ${goal.AmountCents * 0.01f} per month";
// 							textGoal.Text = goal.Description;
// 							sliderProgress.Progress = goal.CompletedPercentage;
// 						}
// 					};
// 					panelGoals.Add(buttonPrev);
//
// 					textProgress = new UIText($"${PatreonGoals[goalIndex].CompletedPercentage * PatreonGoals[goalIndex].AmountCents * 0.0001f:N0} of ${PatreonGoals[goalIndex].AmountCents * 0.01f} per month")
// 					{
// 						Width = { Percent = 100 },
// 						Height = { Pixels = 20 },
// 						Y = { Pixels = 40 }
// 					};
// 					panelGoals.Add(textProgress);
//
// 					textGoal = new UIMultilineText(PatreonGoals[goalIndex].Description)
// 					{
// 						Width = { Percent = 100 },
// 						Height = { Pixels = -84, Percent = 100 },
// 						Y = { Pixels = 84 }
// 					};
// 					panelGoals.Add(textGoal);
//
// 					sliderProgress = new UIRoundedBar
// 					{
// 						Width = { Percent = 100 },
// 						Height = { Pixels = 8 },
// 						Y = { Pixels = 68 },
// 						Progress = PatreonGoals[goalIndex].CompletedPercentage
// 					};
// 					panelGoals.Add(sliderProgress);
// 				}
//
// 				// Links
// 				{
// 					UIPanel panelLinks = new UIPanel
// 					{
// 						Width = { Percent = 50 },
// 						Height = { Percent = 50 },
// 						Y = { Percent = 100 },
// 						X = { Percent = 100 },
// 						Padding = new Padding(8, 8, 8, 8),
// 						BorderColor = Color.Transparent,
// 						BackgroundColor = Utility.ColorPanel_Selected * 0.75f
// 					};
// 					tabMain.Add(panelLinks);
//
// 					// Patreon
// 					{
// 						BaseElement containerPatreon = new BaseElement
// 						{
// 							Width = { Percent = 100 },
// 							Height = { Pixels = 32 }
// 						};
// 						containerPatreon.OnClick += args => Process.Start("https://www.patreon.com/Itorius");
// 						panelLinks.Add(containerPatreon);
//
// 						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/PatreonLogo"), ScaleMode.Stretch)
// 						{
// 							Width = { Pixels = 32 },
// 							Height = { Pixels = 32 }
// 						};
// 						containerPatreon.Add(texture);
//
// 						UIText text = new UIText("Donate on Patreon!")
// 						{
// 							Width = { Pixels = -40, Percent = 100 },
// 							X = { Pixels = 40 },
// 							Height = { Pixels = 28 },
// 							VerticalAlignment = VerticalAlignment.Center
// 						};
// 						containerPatreon.Add(text);
// 					}
//
// 					// TCF
// 					{
// 						BaseElement containerTCF = new BaseElement
// 						{
// 							Width = { Percent = 100 },
// 							Height = { Pixels = 32 },
// 							Y = { Pixels = 40 }
// 						};
// 						containerTCF.OnClick += args => Process.Start("https://forums.terraria.org/index.php?threads/itorius-mods.42289/");
// 						panelLinks.Add(containerTCF);
//
// 						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/TCFLogo"), ScaleMode.Stretch)
// 						{
// 							Width = { Pixels = 32 },
// 							Height = { Pixels = 32 }
// 						};
// 						containerTCF.Add(texture);
//
// 						UIText text = new UIText("Visit my TCF page!")
// 						{
// 							Width = { Pixels = -40, Percent = 100 },
// 							X = { Pixels = 40 },
// 							Height = { Pixels = 28 },
// 							VerticalAlignment = VerticalAlignment.Center
// 						};
// 						containerTCF.Add(text);
// 					}
//
// 					// Github
// 					{
// 						BaseElement containerGithub = new BaseElement
// 						{
// 							Width = { Percent = 100 },
// 							Height = { Pixels = 32 },
// 							Y = { Pixels = 80 }
// 						};
// 						containerGithub.OnClick += args => Process.Start("https://github.com/Eternal-Team");
// 						panelLinks.Add(containerGithub);
//
// 						UITexture texture = new UITexture(ModContent.GetTexture("BaseLibrary/Textures/UI/GithubLogo"), ScaleMode.Stretch)
// 						{
// 							Width = { Pixels = 32 },
// 							Height = { Pixels = 32 }
// 						};
// 						containerGithub.Add(texture);
//
// 						UIText text = new UIText("Check out my code!")
// 						{
// 							Width = { Pixels = -40, Percent = 100 },
// 							X = { Pixels = 40 },
// 							Height = { Pixels = 28 },
// 							VerticalAlignment = VerticalAlignment.Center
// 						};
// 						containerGithub.Add(text);
// 					}
// 				}
// 			}
//
// 			tabChangelogs = new List<BaseElement>();
// 			{
// 				loadingWheelChangelogs = new UILoadingWheel(0.75f)
// 				{
// 					X = { Percent = 50 },
// 					Y = { Percent = 50 }
// 				};
// 				tabChangelogs.Add(loadingWheelChangelogs);
//
// 				UITextButton buttonPrevious = new UITextButton("<")
// 				{
// 					Width = { Pixels = 50 },
// 					Height = { Pixels = 40 },
// 					Padding = new Padding(0, 4, 0, 4)
// 				};
// 				buttonPrevious.OnClick += async args =>
// 				{
// 					if (GithubRepositories.Count == 0) return;
//
// 					if (--repositoryIndex < 0) repositoryIndex = GithubRepositories.Count - 1;
//
// 					await SetRepository(GithubRepositories[repositoryIndex]);
// 				};
// 				tabChangelogs.Add(buttonPrevious);
//
// 				UITextButton buttonNext = new UITextButton(">")
// 				{
// 					Width = { Pixels = 50 },
// 					Height = { Pixels = 40 },
// 					X = { Percent = 100 },
// 					Padding = new Padding(0, 4, 0, 4)
// 				};
// 				buttonNext.OnClick += async args =>
// 				{
// 					if (GithubRepositories.Count == 0) return;
//
// 					if (++repositoryIndex >= GithubRepositories.Count) repositoryIndex = 0;
//
// 					await SetRepository(GithubRepositories[repositoryIndex]);
// 				};
// 				tabChangelogs.Add(buttonNext);
//
// 				textCurrentRepository = new UIText("Loading...", 1.5f)
// 				{
// 					Width = { Pixels = -116, Percent = 100 },
// 					Height = { Pixels = 40 },
// 					X = { Percent = 50 },
// 					HorizontalAlignment = HorizontalAlignment.Center,
// 					VerticalAlignment = VerticalAlignment.Center,
// 					ScaleToFit = true
// 				};
// 				tabChangelogs.Add(textCurrentRepository);
//
// 				textCommits = new UIMultilineText("")
// 				{
// 					Width = { Pixels = -28, Percent = 100 },
// 					Height = { Pixels = -48, Percent = 100 },
// 					Y = { Pixels = 48 },
// 					DrawBackground = true,
// 					Padding = new Padding(8)
// 				};
// 				tabChangelogs.Add(textCommits);
//
// 				textCommits.scrollbar.Height = new StyleDimension { Pixels = -64, Percent = 100 };
// 				textCommits.scrollbar.Y = new StyleDimension { Pixels = 56 };
// 				textCommits.scrollbar.X.Percent = 100;
// 				tabChangelogs.Add(textCommits.scrollbar);
// 			}
//
// 			tabCredits = new List<BaseElement>();
// 			{
// 				gridPatrons = new UIGrid<BaseElement>
// 				{
// 					Width = { Percent = 100 },
// 					Height = { Percent = 100 },
// 					ItemMargin = 0
// 				};
// 				tabCredits.Add(gridPatrons);
// 			}
//
// 			panelMain.AddRange(tabMain);
// 		}
//
// 		public override void Recalculate()
// 		{
// 			base.Recalculate();
//
// 			if (panelMain == null) return;
//
// 			Rectangle dimensions = panelMain.Dimensions;
// 			Point position = new Point((int)panelMain.Position.X, (int)panelMain.Position.Y);
//
// 			int width = (int)(dimensions.Width * 0.31f);
// 			buttonMain.X = new StyleDimension { Pixels = position.X + 8 };
// 			buttonMain.Y = new StyleDimension { Pixels = position.Y - 30 };
// 			buttonMain.Width = new StyleDimension { Pixels = width };
// 			buttonMain.Recalculate();
//
// 			buttonChangelogs.Y = new StyleDimension { Pixels = position.Y - 30 };
// 			buttonChangelogs.Width = new StyleDimension { Pixels = width };
// 			buttonChangelogs.Recalculate();
//
// 			buttonCredits.X = new StyleDimension { Pixels = position.X + dimensions.Width - width - 8 };
// 			buttonCredits.Y = new StyleDimension { Pixels = position.Y - 30 };
// 			buttonCredits.Width = new StyleDimension { Pixels = width };
// 			buttonCredits.Recalculate();
//
// 			buttonReturnToMenu.Y = new StyleDimension { Pixels = dimensions.Y + dimensions.Height + 8 };
// 			buttonReturnToMenu.Recalculate();
// 		}
//
// 		private async Task SetRepository(Repository repository)
// 		{
// 			loadingWheelChangelogs.Enabled = true;
// 			textCommits.Text = "";
//
// 			textCurrentRepository.Text = repository.Name;
//
// 			HttpResponseMessage response = await client.GetAsync("github/getreleases/" + repository.Id);
// 			if (response.IsSuccessStatusCode)
// 			{
// 				var releases = await response.Content.ReadAsAsync<List<Release>>();
// 				if (releases.Count == 0)
// 				{
// 					textCommits.Text = $"No releases found for mod {repository.Name}";
// 					loadingWheelChangelogs.Enabled = false;
// 					return;
// 				}
//
// 				Release currentRelease = releases[0];
// 				Release previousRelease = releases.Skip(1).Take(1).FirstOrDefault();
//
// 				if (previousRelease == null) response = await client.GetAsync($"github/getcommits/{repository.Id}");
// 				else response = await client.GetAsync($"github/getcommits/{repository.Id}/{previousRelease.Id}/{currentRelease.Id}");
//
// 				if (response.IsSuccessStatusCode)
// 				{
// 					var commits = await response.Content.ReadAsAsync<List<GitHubCommit>>();
// 					if (commits.Count == 0)
// 					{
// 						textCommits.Text = $"No commits found for release {currentRelease.TagName}";
// 						loadingWheelChangelogs.Enabled = false;
// 						return;
// 					}
//
// 					textCommits.Text = $"Changelogs for version {currentRelease.TagName}\n\n{commits.Select(commit => commit.Commit.Message).Concat("\n")}";
// 				}
// 			}
//
// 			loadingWheelChangelogs.Enabled = false;
// 		}
// 	}
// }