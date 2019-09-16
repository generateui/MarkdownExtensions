using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.Note
{
	public class NoteParagraphParser : ParagraphBlockParser
	{
		public override BlockState TryOpen(BlockProcessor processor)
		{
			if (processor.IsBlankLine)
			{
				return BlockState.None;
			}
			if (processor.Line.Match("Note: "))
			{
				processor.NewBlocks.Push(new NoteParagraphBlock(this)
				{
					Column = processor.Column,
					Span = new SourceSpan(processor.Line.Start + 6, processor.Line.End)
				});
				return BlockState.Continue;
			}
			return BlockState.None;
		}
	}
}
