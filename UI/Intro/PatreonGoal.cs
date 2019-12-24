using Newtonsoft.Json;

namespace BaseLibrary.UI.Intro
{
	[JsonConverter(typeof(JsonPathConverter))]
	public class PatreonGoal : PatreonEntity
	{
		[JsonProperty("attributes.title")]
		public string Title { get; set; }

		[JsonProperty("attributes.description")]
		public string Description { get; set; }

		[JsonProperty("attributes.amountCents")]
		public int AmountCents { get; set; }

		[JsonProperty("attributes.completedPercentage")]
		public int CompletedPercentage { get; set; }
	}
}