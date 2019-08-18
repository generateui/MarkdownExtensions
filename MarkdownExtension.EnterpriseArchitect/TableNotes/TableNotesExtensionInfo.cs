using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesExtensionInfo : IExtensionInfo
	{
		public string Name => "Enterprise Architect Table notes";
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("TableNotes.CheatSheet.md");
	}
}
