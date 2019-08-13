using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesExtensionInfo : IExtensionInfo
	{
		public string Name => "EA Table notes";
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("TablNotes.CheatSheet.md");
	}
}
