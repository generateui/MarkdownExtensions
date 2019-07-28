using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;

namespace MarkdownExtension.PanZoomImage
{
	public partial class PanZoomImageExtension : IExtension
    {
        public IParser Parser { get; }
        public IRenderer Renderer { get; }
        public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "Pan & zoom an image";
        public ExtensionName Name => NAME;

		public PanZoomImageExtension()
        {
            Parser = new PanZoomImageSyntax();
            Renderer = new PanZoomImageRenderer();
        }

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new PanZoomImageParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
