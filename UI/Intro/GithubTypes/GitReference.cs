namespace Octokit
{
	public class GitReference
	{
		public string NodeId { get; set; }

		public string Url { get; set; }

		public string Label { get; set; }

		public string Ref { get; set; }

		public string Sha { get; set; }

		public User User { get; set; }

		public Repository Repository { get; set; }
	}
}