using System;

namespace Octokit
{
	public class CommitStatus
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public string State { get; set; }

		public string TargetUrl { get; set; }

		public string Description { get; set; }

		public string Context { get; set; }

		public long Id { get; set; }

		public string NodeId { get; set; }

		public string Url { get; set; }

		public User Creator { get; set; }
	}

	public enum CommitState
	{
		Pending,
		Success,
		Error,
		Failure
	}
}