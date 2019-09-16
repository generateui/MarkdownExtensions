using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.Note
{
	public class NoteExtension : IExtension
	{
		public NoteExtension()
		{
			Parser = new NoteSyntax();
			Renderer = new NoteRenderer();
		}

		public ExtensionName Name => NAME;
		private static readonly ExtensionName NAME = "note";
		public bool IsSummary => false;

		public IParser Parser { get; }
		public IValidator Validator => null;
		public IRenderer Renderer { get; }
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new NoteParagraphParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
