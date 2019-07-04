using MarkdownExtensions;
using LibGit2Sharp;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MarkdownExtension.GitHistory
{
    public class GitHistory : IMarkdownExtension
    {
        public class GitHistoryConfig
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
        public class GitHistorySyntax : ISyntax
        {
            /* branch (optional, default: master)
             * format (optional, sane default)
             * tag
             * commits
             * 
             * ```git-history:
             * show = "tags" #commits
             * fields = "date{d}|tag|message|author{name}"
             * ```
             */
            public IParseResult Parse(string text)
            {
                var toml = Toml.Toml.Parse(text);
                string fieldsText = toml.fields as string;
                var split = fieldsText.Split('|');
                var fields = new List<IField>();
                foreach (var field in split)
                {
                    if (field.StartsWith("date"))
                    {
                        string formatString = null;
                        if (field.Contains("{") && field.Contains("}"))
                        {
                            formatString = field.Substring(5, field.Length - 6 - 1);

                        }
                        fields.Add(new Date(formatString));
                        continue;
                    }
                    if (field.StartsWith("tag"))
                    {
                        fields.Add(new Tag());
                        continue;
                    }
                    if (field.StartsWith("message"))
                    {
                        fields.Add(new Message());
                        continue;
                    }
                    if (field.StartsWith("author"))
                    {
                        fields.Add(new Author());
                        continue;
                    }
                }
                return new ParseSuccess(new GitHistoryConfig { Fields = fields });
            }
        }
        public class GitHistoryFormatter : IFormatter
        {
            public FormatResult Format(object root, IFormatState state)
            {
                var config = root as GitHistoryConfig;
                var repo = new Repository(@"c:\users\ruud\desktop\Enterprise-Architect-Toolpack");
                //var commits = repo.Commits.QueryBy("README.md").ToList();
                var sb = new StringBuilder();
                sb.AppendLine("<table>");
                sb.AppendLine("<thead>");
                sb.AppendLine("<tr>");
                foreach (var field in config.Fields)
                {
                    sb.AppendLine($@"<td>{field.Name}</td>");
                }
                sb.AppendLine("</tr>");

                sb.AppendLine("</thead>");

                foreach (var tag in repo.Tags)
                {
                    sb.AppendLine("<tr>");
                    Commit commit = tag.Target as Commit;
                    foreach (var field in config.Fields)
                    {
                        var text = field.GetTagText(tag);
                        sb.AppendLine($@"<td>{text}</td>");
                    }
                    var sha = new string(commit.Sha.Take(7).ToArray());
                    sb.AppendLine("</tr>");
                }
                sb.AppendLine("</table>");
                return FormatResult.FromHtml(sb.ToString());
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        public GitHistory()
        {
            Syntax = new GitHistorySyntax();
            Formatter = new GitHistoryFormatter();
        }

        public string Prefix => "git-history";
        public IElementType Type => ElementType.Block;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "Git history in a table";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
    public static class CommitExtensions
    {
        public static string ToText(this Commit commit)
        {
            var sha = new string(commit.Sha.Take(7).ToArray());
            return $@"🏷 {sha} {commit.Message} {commit.Author}";
        }
    }
}
