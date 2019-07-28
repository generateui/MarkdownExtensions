using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.GitGraph
{
	public class GitGraphExtensionInfo : IExtensionInfo
	{
		public string Name => GitGraphExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
