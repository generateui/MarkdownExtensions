using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.Note
{
	public class NoteParagraphBlock : LeafBlock, IExtensionBlock
	{
		public NoteParagraphBlock(BlockParser parser) : base(parser)
		{
			ProcessInlines = true;
		}

		public int LastLine => Line + Lines.Count - 1;
	}
}
