using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesBlock : FencedCodeBlock, IExtensionBlock
	{
		public WorkflowNotesBlock(BlockParser parser) : base(parser) { }
	}
}
