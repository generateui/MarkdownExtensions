using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.XmlSnippet
{
	public class XmlSnippetBlock : FencedCodeBlock, IExtensionBlock
	{
		public XmlSnippetBlock(BlockParser parser) : base(parser)
		{
		}
	}
}
