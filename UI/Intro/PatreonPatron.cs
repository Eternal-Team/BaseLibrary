using Newtonsoft.Json;
using Terraria;

namespace BaseLibrary.UI.Intro
{
	[JsonConverter(typeof(JsonPathConverter))]
	public class PatreonPatron : PatreonEntity
	{
		[JsonProperty("attributes.name")]
		public string Name { get; set; }

		[JsonProperty("attributes.status")]
		public string Status { get; set; }

		[JsonProperty("attributes.lifetimeCents")]
		public int LifetimeCents { get; set; }

		[JsonProperty("attributes.currentCents")]
		public int CurrentCents { get; set; }

		public PatreonTier Tier;
	}
}