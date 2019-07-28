using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesBlock : FencedCodeBlock, IExtensionBlock
	{
		public TableNotesBlock(BlockParser parser) : base(parser) { }
	}
}
