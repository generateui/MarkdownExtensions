using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.FolderList
{
	public sealed class FolderListExtension : IExtension
    {
		public FolderListExtension()
        {
            Parser = new FolderListSyntax();
            Renderer = new FolderListRenderer();
        }

        public IParser Parser { get; }
        public IRenderer Renderer { get; }
        public IValidator Validator => null;
		public ITransformer Transformer => null;

		public static ExtensionName NAME => "Folder using a Markdown list";
        public ExtensionName Name => NAME;
		public bool IsSummary => false;

		public void Setup(MarkdownPipelineBuilder pipeline) => 
			pipeline.BlockParsers.Insert(0, new FolderListParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) => 
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
