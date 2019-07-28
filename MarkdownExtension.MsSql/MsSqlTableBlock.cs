using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.MsSql
{
	public class MsSqlTableBlock : FencedCodeBlock, IExtensionBlock
	{
		public MsSqlTableBlock(BlockParser parser) : base(parser) { }
	}
}
