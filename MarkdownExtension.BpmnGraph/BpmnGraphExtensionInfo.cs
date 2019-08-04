using MarkdownExtensions;
using System.Reflection;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphExtensionInfo : IExtensionInfo
	{
		public string Name => "Bpmn graph";
		public string CheatSheet => Assembly.GetExecutingAssembly().GetFileContent("CheatSheet.md");
	}
}
