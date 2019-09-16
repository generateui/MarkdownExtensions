using Markdig;
using Markdig.Renderers;
using MarkdownExtensions;

namespace MarkdownExtension.Excel
{
	// first row is header
	// first column is header
	// row uses 90deg angled text
	// styled cells
	// conditionally styled cells (use materialized)
	public partial class ExcelTableExtension : IExtension
    {
        public ExcelTableExtension()
        {
            Parser = new ExcelTableSyntax();
            Validator = new ExcelTableValidator();
            Renderer = new ExcelTableRenderer();
        }

        public ExtensionName Name => NAME;
        public static readonly ExtensionName NAME = "Excel table";
		public bool IsSummary => false;

		public IParser Parser { get; }
        public IValidator Validator { get; }
        public IRenderer Renderer { get; }
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new ExcelTableParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
