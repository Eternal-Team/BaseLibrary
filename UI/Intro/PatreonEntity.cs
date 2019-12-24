using Newtonsoft.Json;

namespace BaseLibrary.UI.Intro
{
	public class PatreonEntity
	{
		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonProperty("type")]
		public PatreonType Type { get; set; }
	}
}