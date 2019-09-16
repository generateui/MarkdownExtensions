using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;

namespace MarkdownExtension.KeyboardKeys
{
	public partial class KeyboardKeysExtension : IExtension
    {
        public KeyboardKeysExtension()
        {
            Parser = new KeyboardKeysSyntax();
            Renderer = new KeyboardKeysRenderer();
        }

        public IParser Parser { get; }
        public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "Keyboard keys";
        public ExtensionName Name => NAME;
		public bool IsSummary => false;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.InlineParsers.Insert(0, new KeyboardKeysParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
