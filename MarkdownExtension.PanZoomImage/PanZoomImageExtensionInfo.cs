using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.PanZoomImage
{
	public class PanZoomImageExtensionInfo : IExtensionInfo
	{
		public string Name => PanZoomImageExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
