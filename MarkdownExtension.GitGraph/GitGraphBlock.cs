using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.GitGraph
{
	public class GitGraphBlock : FencedCodeBlock, IExtensionBlock
	{
		public GitGraphBlock(BlockParser parser) : base(parser) { }
	}
}