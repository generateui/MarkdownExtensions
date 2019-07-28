using MarkdownExtensions;

namespace MarkdownExtension.GitHistory
{
	public class GitHistoryParser : BlockExtensionParser<GitHistoryBlock>
	{
		public GitHistoryParser()
		{
			InfoPrefix = "git-history";
			_create = _ => new GitHistoryBlock(this);
		}
	}
}
