using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetBlock : FencedCodeBlock, IExtensionBlock
	{
		public SnippetBlock(BlockParser parser) : base(parser) { }
	}
}
