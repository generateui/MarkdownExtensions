using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.Excel
{
	public class ExcelTableBlock : FencedCodeBlock, IExtensionBlock
	{
		public ExcelTableBlock(BlockParser parser) : base(parser) { }
	}
}
