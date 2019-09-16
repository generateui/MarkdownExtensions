using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.Note
{
	public class NoteParagraphBlock : ParagraphBlock, IExtensionBlock
	{
		public NoteParagraphBlock(BlockParser parser) : base(parser)
		{
		}
	}
}
