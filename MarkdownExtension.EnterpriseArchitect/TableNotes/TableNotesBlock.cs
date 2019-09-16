using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesBlock : FencedCodeBlock, IExtensionBlock
	{
		private ContainerBlock parent;

		public TableNotesBlock(BlockParser parser) : base(parser) { }
		public new ContainerBlock Parent { get => parent; set => parent = value; }
	}
}
