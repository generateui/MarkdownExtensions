using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public partial class DiagramImageExtension : IExtension
	{
		public DiagramImageExtension(IEaProvider provider)
		{
			Parser = new DiagramSyntax();
			Renderer = new DiagramRenderer(provider);
		}

		public string Prefix => "ea-diagram";
		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public static ExtensionName NAME => "EA diagram";
		public ExtensionName Name => NAME;
		public IValidator Validator => null;

		public ITransformer Transformer => throw new System.NotImplementedException();

		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			throw new System.NotImplementedException();
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			throw new System.NotImplementedException();
		}
	}
}
