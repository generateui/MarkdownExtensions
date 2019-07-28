using System.Reflection;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	public class FolderFromDiskExtensionInfo : IExtensionInfo
	{
		public string Name => FolderFromDiskExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("Extensions.FolderFromDisk.CheatSheet.md");
	}
}
