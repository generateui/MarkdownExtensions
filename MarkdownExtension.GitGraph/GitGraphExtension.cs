using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GitGraphExtension.UnitTest")]
namespace MarkdownExtension.GitGraph
{
	public class GitGraphExtension : IExtension
    {
        public GitGraphExtension()
        {
            Parser = new Syntax();
            Renderer = new GitGraphRenderer();
        }

		public static ExtensionName NAME => "Git graph using syntax";
		public ExtensionName Name => NAME;

		public IParser Parser { get; }
        public IRenderer Renderer { get; }
        public IValidator Validator => null;
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new GitGraphParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
