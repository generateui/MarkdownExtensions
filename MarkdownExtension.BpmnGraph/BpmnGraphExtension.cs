using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphExtension : IExtension
	{
		public BpmnGraphExtension()
		{
			Parser = new BpmnGraphSyntax();
			Renderer = new BpmnGraphRenderer();
		}
		public static ExtensionName NAME = "BPMN graph";
		public ExtensionName Name => NAME;
		public bool IsSummary => false;

		public IParser Parser { get; }
		public IValidator Validator => null;
		public IRenderer Renderer { get; }
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new BpmnGraphParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
