using MarkdownExtensions.Extensions.MarkdownLink;
using System.Reflection;

namespace MarkdownExtensions.Extensions.MarkdownLinks
{
	class MarkdownLinkExtensionInfo : IExtensionInfo
	{
		public string Name => MarkdownLinkExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
