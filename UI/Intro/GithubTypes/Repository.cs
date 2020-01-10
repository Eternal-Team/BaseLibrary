using System;

namespace Octokit
{
	public class Repository
	{
		public string Url { get; set; }

		public string HtmlUrl { get; set; }

		public string CloneUrl { get; set; }

		public string GitUrl { get; set; }

		public string SshUrl { get; set; }

		public string SvnUrl { get; set; }

		public string MirrorUrl { get; set; }

		public long Id { get; set; }

		public string NodeId { get; set; }

		public User Owner { get; set; }

		public string Name { get; set; }

		public string FullName { get; set; }

		public string Description { get; set; }

		public string Homepage { get; set; }

		public string Language { get; set; }

		public bool Private { get; set; }

		public bool Fork { get; set; }

		public int ForksCount { get; set; }

		public int StargazersCount { get; set; }

		public string DefaultBranch { get; set; }

		public int OpenIssuesCount { get; set; }

		public DateTimeOffset? PushedAt { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public RepositoryPermissions Permissions { get; set; }

		public Repository Parent { get; set; }

		public Repository Source { get; set; }

		public bool HasIssues { get; set; }

		public bool HasWiki { get; set; }

		public bool HasDownloads { get; set; }

		public bool? AllowRebaseMerge { get; set; }

		public bool? AllowSquashMerge { get; set; }

		public bool? AllowMergeCommit { get; set; }

		public bool HasPages { get; set; }

		public int SubscribersCount { get; set; }

		public long Size { get; set; }

		public bool Archived { get; set; }
	}
}