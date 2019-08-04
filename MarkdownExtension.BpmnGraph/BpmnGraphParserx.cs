using MarkdownExtensions;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphParser : BlockExtensionParser<BpmnGraphBlock>
	{
		public BpmnGraphParser()
		{
			InfoPrefix = "bpmn-graph";
			_create = _ => new BpmnGraphBlock(this);
		}
	}
}
