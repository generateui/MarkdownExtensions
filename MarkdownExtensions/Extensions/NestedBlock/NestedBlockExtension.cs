using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.NestedBlock
{
	public partial class NestedBlockExtension : IExtension
    {
        public string Prefix => "nested-block-md";
        public ExtensionName Name => "Nested markdown example";
        public IParser Parser { get; } = new NestedBlockSyntax();
        public IValidator Validator => null;
		public IRenderer Renderer => null;
		public ITransformer Transformer => new NestedBlockTransformer();

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new NestedBlockParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
