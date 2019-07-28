using LibGit2Sharp;
using System.Collections.Generic;

namespace MarkdownExtension.GitHistory
{
	public class GitHistory
	{
		public IEnumerable<IField> Fields { get; set; }
	}
	public interface IField
	{
		string Name { get; }
		string GetTagText(LibGit2Sharp.Tag tag);
	}
	public class Sha : IField
	{
		public string Name => "SHA";
		public string GetTagText(LibGit2Sharp.Tag tag)
		{
			Commit commit = tag.Target as Commit;
			return commit.Sha;
		}
	}

	public class Date : IField
	{
		public Date(string format)
		{
			Format = format;
		}
		public string Name => "Date";
		public string Format { get; } //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
		public string GetTagText(LibGit2Sharp.Tag tag)
		{
			Commit commit = tag.Target as Commit;
			var format = Format ?? "d";
			return commit.Author.When.ToString(format);
		}
	}
	public class Tag : IField
	{
		public string Name => "Tag";
		public string GetTagText(LibGit2Sharp.Tag tag) => tag.FriendlyName;
	}
	public class Message : IField
	{
		public string Name => "Message";

		public string GetTagText(LibGit2Sharp.Tag tag)
		{
			Commit commit = tag.Target as Commit;
			return commit.Message;
		}
	}
	public class Author : IField
	{
		public string Name => "Author";

		public string GetTagText(LibGit2Sharp.Tag tag)
		{
			Commit commit = tag.Target as Commit;
			return commit.Author.ToString();
		}
	}
}
