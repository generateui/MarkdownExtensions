using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesExtension : IExtension
	{
		public WorkflowNotesExtension(IEaProvider provider)
		{
			Parser = new WorkflowNotesSyntax();
			Renderer = new WorkflowNotesRenderer(provider);
		}

		public string Prefix => "ea-workflow-notes";
		public ExtensionName Name => "EA workflow notes";

		public IValidator Validator => null;
		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new WorkflowNotesParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
