using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public class ObjectTextExtensionInfo : IExtensionInfo
	{
		public string Name => "EA object text";
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("ObjectText.CheatSheet.md");
	}
}
