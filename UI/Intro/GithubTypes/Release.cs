using System;
using System.Collections.Generic;

namespace Octokit
{
	public class Release
	{
		public string Url { get; set; }

		public string HtmlUrl { get; set; }

		public string AssetsUrl { get; set; }

		public string UploadUrl { get; set; }

		public int Id { get; set; }

		public string NodeId { get; set; }

		public string TagName { get; set; }

		public string TargetCommitish { get; set; }

		public string Name { get; set; }

		public string Body { get; set; }

		public bool Draft { get; set; }

		public bool Prerelease { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset? PublishedAt { get; set; }

		public Author Author { get; set; }

		public string TarballUrl { get; set; }

		public string ZipballUrl { get; set; }

		public List<ReleaseAsset> Assets { get; set; }
	}
}