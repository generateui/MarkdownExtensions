using MarkdownExtensions;
using LibGit2Sharp;
using System.Text;
using System.Linq;

namespace MarkdownExtension.GitHistory
{
	public class GitHistoryFormatter : BlockRendererBase<GitHistory, GitHistoryBlock>
	{
		public override void Render(ExtensionHtmlRenderer renderer, GitHistory gitHistory, IFormatState formatState)
		{
			var repo = new Repository(@"c:\users\ruudp\desktop\markdig");
			//var commits = repo.Commits.QueryBy("README.md").ToList();
			var sb = new StringBuilder();
			sb.AppendLine("<table>");
			sb.AppendLine("<thead>");
			sb.AppendLine("<tr>");
			foreach (var field in gitHistory.Fields)
			{
				sb.AppendLine($@"<td>{field.Name}</td>");
			}
			sb.AppendLine("</tr>");

			sb.AppendLine("</thead>");

			foreach (var tag in repo.Tags)
			{
				sb.AppendLine("<tr>");
				Commit commit = tag.Target as Commit;
				foreach (var field in gitHistory.Fields)
				{
					var text = field.GetTagText(tag);
					sb.AppendLine($@"<td>{text}</td>");
				}
				var sha = new string(commit.Sha.Take(7).ToArray());
				sb.AppendLine("</tr>");
			}
			sb.AppendLine("</table>");
			renderer.Write(sb.ToString());
		}
	}
}
