using System.Collections.Generic;

namespace Octokit
{
	public class Commit : GitReference
	{
		public string Message { get; set; }

		public Committer Author { get; set; }

		public Committer Committer { get; set; }

		public GitReference Tree { get; set; }

		public List<GitReference> Parents { get; set; }

		public int CommentCount { get; set; }
	}
}