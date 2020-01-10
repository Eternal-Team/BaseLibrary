using System.Collections.Generic;

namespace Octokit
{
	public class GitHubCommit : GitReference
	{
		public Author Author { get; set; }

		public string CommentsUrl { get; set; }

		public Commit Commit { get; set; }

		public Author Committer { get; set; }

		public string HtmlUrl { get; set; }

		public GitHubCommitStats Stats { get; set; }

		public List<GitReference> Parents { get; set; }

		public List<GitHubCommitFile> Files { get; set; }
	}
}