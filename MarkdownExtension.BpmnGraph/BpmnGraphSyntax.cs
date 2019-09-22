using MarkdownExtensions;
using System;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var lines = text.Split('\n');
			var bpmnGraph = new BpmnGraph();
			foreach (var line in lines)
			{
				var sanitized = line.Trim().ToLower();
				if (sanitized.StartsWith("height: "))
				{
					bpmnGraph.Height = sanitized.Substring(8);
				}
				if (sanitized.StartsWith("file: "))
				{
					bpmnGraph.FileUri = sanitized.Substring(6);
				}
			}
			if (bpmnGraph.FileUri != null)
			{
				return new ParseSuccess(bpmnGraph);
			}
			return new ParseFailure(0, "Expected a reference to a bpmn file");
		}
	}
}
