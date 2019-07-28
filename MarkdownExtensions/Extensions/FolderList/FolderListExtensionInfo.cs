using System.Reflection;

namespace MarkdownExtensions.Extensions.FolderList
{
	public class FolderListExtensionInfo : IExtensionInfo
	{
		public string Name => FolderListExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("Extensions.FolderList.CheatSheet.md");
	}
}
