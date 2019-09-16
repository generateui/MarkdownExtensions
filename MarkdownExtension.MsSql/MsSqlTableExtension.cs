using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;

namespace MarkdownExtension.MsSql
{
	public partial class MsSqlTableExtension : IExtension
    {
        internal static readonly ExtensionName NAME = "Sql table";
        public MsSqlTableExtension()
        {
            Parser = new MsSqlTableSyntax();
            Renderer = new MsSqlTableRenderer();
            Validator = new MsSqlTableValidator();
        }
        public ExtensionName Name => NAME;
        public IParser Parser { get; }
        public IValidator Validator { get; }
        public IRenderer Renderer { get; }
		public ITransformer Transformer => null;
		public bool IsSummary => false;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new MsSqlTableParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
