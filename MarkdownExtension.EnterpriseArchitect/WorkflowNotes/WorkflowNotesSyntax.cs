using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			string diagramName = text.Trim();
			return new ParseSuccess(new Diagram { Name = diagramName });
		}
	}
}
