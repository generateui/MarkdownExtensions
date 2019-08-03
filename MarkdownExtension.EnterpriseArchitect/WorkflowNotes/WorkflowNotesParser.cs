using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesParser : BlockExtensionParser<WorkflowNotesBlock>
	{
		public WorkflowNotesParser()
		{
			InfoPrefix = "ea-workflow-notes";
			_create = _ => new WorkflowNotesBlock(this);
		}
	}
}
