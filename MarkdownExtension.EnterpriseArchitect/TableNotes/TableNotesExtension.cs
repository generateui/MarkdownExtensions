using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesExtension : IExtension
    {
        public TableNotesExtension(IEaProvider provider)
        {
            Parser = new TableNotesSyntax();
            Renderer = new TableNotesRenderer(provider);
            Validator = new TableNotesValidator(provider);
        }

        public IParser Parser { get; }
        public IRenderer Renderer { get; }
		public IValidator Validator { get; }
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA element notes in a table";
        public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			throw new NotImplementedException();
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			throw new NotImplementedException();
		}
	}
}
