using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.GitHistory
{
	public class GitHistoryBlock : FencedCodeBlock, IExtensionBlock
	{
		public GitHistoryBlock(BlockParser parser) : base(parser) { }
	}
}
