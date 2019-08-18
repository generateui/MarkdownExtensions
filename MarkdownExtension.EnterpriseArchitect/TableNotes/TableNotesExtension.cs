using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesExtension : IExtension
    {
        public TableNotesExtension(IEaProvider provider)
        {
            Parser = new TableNotesSyntax();
            Validator = new TableNotesValidator(provider);
			Transformer = new TableNotesTransformer(provider);
        }

        public IParser Parser { get; }
		public IRenderer Renderer => null;
		public IValidator Validator { get; }
		public ITransformer Transformer { get; }
		public static ExtensionName NAME => "EA element notes in a table";
        public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new TableNotesParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
