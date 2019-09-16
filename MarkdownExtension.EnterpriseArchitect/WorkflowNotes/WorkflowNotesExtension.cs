using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesExtension : IExtension
	{
		public WorkflowNotesExtension(IEaProvider provider)
		{
			Parser = new WorkflowNotesSyntax();
			Transformer = new WorkflowNotesTransformer(provider);
		}

		public string Prefix => "ea-workflow-notes";
		public ExtensionName Name => "EA workflow notes";
		public bool IsSummary => false;

		public IValidator Validator => null;
		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public ITransformer Transformer { get; }

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new WorkflowNotesParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
