using System.Reflection;

namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetExtensionInfo : IExtensionInfo
	{
		public string Name => SnippetExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("Extensions.Snippet.CheatSheet.md");
	}
}
