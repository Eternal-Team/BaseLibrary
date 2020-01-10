using System;

namespace Octokit
{
	public class User : Account
	{
		public RepositoryPermissions Permissions { get; set; }
		public bool SiteAdmin { get; set; }
		public DateTimeOffset? SuspendedAt { get; set; }
		public bool Suspended => SuspendedAt.HasValue;
		public string LdapDistinguishedName { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
	}
}