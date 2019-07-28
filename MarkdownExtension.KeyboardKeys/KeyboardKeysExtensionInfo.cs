using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.KeyboardKeys
{
	public class KeyboardKeysExtensionInfo : IExtensionInfo
	{
		public string Name => KeyboardKeysExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
