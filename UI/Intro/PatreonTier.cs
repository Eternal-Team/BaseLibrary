using Newtonsoft.Json;
using Terraria;

namespace BaseLibrary.UI.Intro
{
	[JsonConverter(typeof(JsonPathConverter))]
	public class PatreonTier : PatreonEntity
	{
		[JsonProperty("attributes.title")]
		public string Title { get; set; }

		[JsonProperty("attributes.description")]
		public string Description { get; set; }

		[JsonProperty("attributes.amountCents")]
		public int AmountCents { get; set; }

		[JsonProperty("attributes.published")]
		public bool Published { get; set; }

		[JsonProperty("attributes.userLimit")]
		public int? UserLimit { get; set; }

		[JsonProperty("attributes.remainingLimit")]
		public int? RemainingLimit { get; set; }

		[JsonProperty("attributes.DiscordRoles")]
		public long[] DiscordRoles { get; set; }
	}
}