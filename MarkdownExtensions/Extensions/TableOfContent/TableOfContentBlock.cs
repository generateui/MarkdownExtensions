using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	public class TableOfContentBlock : FencedCodeBlock, IExtensionBlock
	{
		public TableOfContentBlock(BlockParser parser) : base(parser) { }
	}
}
