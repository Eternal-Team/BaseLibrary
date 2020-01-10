using System;

namespace Octokit
{
	public class Committer
	{
		public string NodeId { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public DateTimeOffset Date { get; set; }
	}
}