using MarkdownExtensions;
using System;
using System.Reflection;

namespace MarkdownExtension.MsSql
{
	public class MsSqlTableExtensionInfo : IExtensionInfo
	{
		public string Name => MsSqlTableExtension.NAME.ToString();
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
