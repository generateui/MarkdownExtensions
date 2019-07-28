using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GitGraphExtension.UnitTest")]
namespace MarkdownExtension.GitGraph
{
	public class GitGraph
	{
		public List<Commit> Commits { get; set; } = new List<Commit>();

		public override bool Equals(object obj)
		{
			var other = obj as GitGraph;
			if (other == null)
			{
				return false;
			}
			if (Commits.Count != other.Commits.Count)
			{
				return false;
			}
			for (int i = 0; i < Commits.Count; i++)
			{
				if (!Equals(Commits[i], other.Commits[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return 561792837 + EqualityComparer<List<Commit>>.Default.GetHashCode(Commits);
		}
	}
	public class Commit
	{
		public CommitId CommitId { get; set; }
		public BranchName Branch { get; set; }
		public BranchName Merge { get; set; }
		public BranchName BranchName { get; set; }
		public Tag Tag { get; set; }
		public string Title { get; set; }
		public Author Author { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Commit;
			if (other == null)
			{
				return false;
			}
			return Equals(CommitId, other.CommitId) &&
				Equals(Branch, other.Branch) &&
				Equals(Merge, other.Merge) &&
				Equals(BranchName, other.BranchName) &&
				Equals(Tag, other.Tag) &&
				Equals(Title, other.Title) &&
				Equals(Author, other.Author);
		}

		public override int GetHashCode()
		{
			var hashCode = 356718453;
			hashCode = hashCode * -1521134295 + EqualityComparer<CommitId>.Default.GetHashCode(CommitId);
			hashCode = hashCode * -1521134295 + EqualityComparer<BranchName>.Default.GetHashCode(Branch);
			hashCode = hashCode * -1521134295 + EqualityComparer<BranchName>.Default.GetHashCode(Merge);
			hashCode = hashCode * -1521134295 + EqualityComparer<BranchName>.Default.GetHashCode(BranchName);
			hashCode = hashCode * -1521134295 + EqualityComparer<Tag>.Default.GetHashCode(Tag);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
			hashCode = hashCode * -1521134295 + EqualityComparer<Author>.Default.GetHashCode(Author);
			return hashCode;
		}
	}
	public class BranchName
	{
		public BranchName(string name)
		{
			Name = name;
		}
		public string Name { get; set; }
		public static BranchName Parse(string text, ref int i)
		{
			var name = "";
			i += 1;
			while (i < text.Length)
			{
				var c = text[i];
				if (c == ']')
				{
					return new BranchName(name);
				}
				else
				{
					name += c;
				}
				i += 1;
			}
			throw new Exception();
		}
		public override bool Equals(object obj)
		{
			var other = obj as BranchName;
			if (other == null)
			{
				return false;
			}
			return Equals(Name, other.Name);
		}

		public override int GetHashCode() => Name.GetHashCode();
	}
	public class Tag
	{
		public Tag(string name)
		{
			Name = name;
		}
		public string Name { get; set; }
		internal static Tag Parse(string text, ref int i)
		{
			var name = "";
			i += 1;
			while (i < text.Length)
			{
				var c = text[i];
				if (c == '#')
				{
					return new Tag(name);
				}
				else
				{
					name += c;
				}
				i += 1;
			}
			throw new Exception();
		}
		public override bool Equals(object obj)
		{
			var other = obj as Tag;
			if (other == null)
			{
				return false;
			}
			return Equals(Name, other.Name);
		}
	}
	public class Author
	{
		public Author(string name, string email)
		{
			Name = name;
			Email = email;
		}

		public string Name { get; set; }
		public string Email { get; set; }

		internal static Author Parse(string text, ref int i)
		{
			i += 1; // assume first char is double quote
			var name = "";
			string email = "";
			bool inNameState = true;
			bool firstDoubleQuote = true;
			while (i < text.Length)
			{
				var c = text[i];
				if (c == '"' && firstDoubleQuote)
				{
					firstDoubleQuote = false;
				}
				else if (c == '"' && !firstDoubleQuote && text[i + 1] != '<')
				{
					return new Author(name, null);
				}
				else if (c == '"' && !firstDoubleQuote)
				{
					inNameState = false;
				}
				else if (c == '<')
				{
				}
				else if (c == '>')
				{
					return new Author(name, email);
				}
				else
				{
					if (inNameState)
					{
						name += c;
					}
					else
					{
						email += c;
					}
				}
				i += 1;
			}
			throw new Exception();
		}
		public override bool Equals(object obj)
		{
			var other = obj as Author;
			if (other == null)
			{
				return false;
			}
			return Equals(Name, other.Name) && Equals(Email, other.Email);
		}
	}
	public class CommitId
	{
		public CommitId(string id)
		{
			Id = id;
		}
		public string Id { get; set; }
		internal static CommitId Parse(string text, ref int i)
		{
			var id = "";
			i += 1;
			while (i < text.Length)
			{
				var c = text[i];
				if (c == '}')
				{
					return new CommitId(id);
				}
				else if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))
				{
					id += c;
				}
				else
				{
					throw new Exception();
				}
				i += 1;
			}
			throw new Exception();
		}
		public override bool Equals(object obj)
		{
			var other = obj as CommitId;
			if (other == null)
			{
				return false;
			}
			return Equals(Id, other.Id);
		}
	}
}
