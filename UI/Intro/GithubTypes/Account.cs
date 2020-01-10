using System;

namespace Octokit
{
	public abstract class Account
	{
		public string AvatarUrl { get; set; }

		public string Bio { get; set; }

		public string Blog { get; set; }

		public int? Collaborators { get; set; }

		public string Company { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public int? DiskUsage { get; set; }

		public string Email { get; set; }

		public int Followers { get; set; }

		public int Following { get; set; }

		public bool? Hireable { get; set; }

		public string HtmlUrl { get; set; }

		public int Id { get; set; }

		public string NodeId { get; set; }

		public string Location { get; set; }

		public string Login { get; set; }

		public string Name { get; set; }

		public AccountType? Type { get; set; }

		public int OwnedPrivateRepos { get; set; }

		public int? PrivateGists { get; set; }

		public int PublicGists { get; set; }

		public int PublicRepos { get; set; }

		public int TotalPrivateRepos { get; set; }

		public string Url { get; set; }
	}
}