using System;
using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	// numbering scheme x.x.x.x, x.XIV.a etc
	// starting level
	// depth level
	public class TableOfContentExtension : IExtension
	{
		public TableOfContentExtension()
		{
			Parser = new TableOfContentSyntax();
			//Transformer = new TableOfContentTransformer();
			Renderer = new TableOfContentRenderer();
		}
		public ExtensionName Name => throw new NotImplementedException();
		public bool IsSummary => true;

		public IParser Parser { get; }
		public IValidator Validator => null;
		public IRenderer Renderer { get; }
		public ITransformer Transformer { get; }

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new TableOfContentParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
