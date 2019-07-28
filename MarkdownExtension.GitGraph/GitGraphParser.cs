using MarkdownExtensions;

namespace MarkdownExtension.GitGraph
{
	public class GitGraphParser : BlockExtensionParser<GitGraphBlock>
	{
		public GitGraphParser()
		{
			InfoPrefix = "git-graph";
			_create = _ => new GitGraphBlock(this);
		}
	}
}
