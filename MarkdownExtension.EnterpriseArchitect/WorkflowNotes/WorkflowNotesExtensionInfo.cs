using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesExtensionInfo : IExtensionInfo
	{
		public string Name => "Show notes of BPMN 2.0 workflows";
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent(@"WorkflowNotes.CheatSheet.md");
	}
}
