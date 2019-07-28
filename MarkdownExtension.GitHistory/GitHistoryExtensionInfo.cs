using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.GitHistory
{
	public class GitHistoryExtensionInfo : IExtensionInfo
	{
		public string Name => GitHistoryExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
