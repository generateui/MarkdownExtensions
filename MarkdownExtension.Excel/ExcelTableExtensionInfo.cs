using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.Excel
{
	public class ExcelTableExtensionInfo : IExtensionInfo
	{
		public string Name => ExcelTableExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
