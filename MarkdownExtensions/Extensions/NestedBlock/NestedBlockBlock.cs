using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.NestedBlock
{
	public class NestedBlockBlock : FencedCodeBlock, IExtensionBlock
	{
		public NestedBlockBlock(BlockParser parser) : base(parser) { }
	}
}
