﻿using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public partial class DiagramImageExtension : IExtension
	{
		public DiagramImageExtension(IEaProvider provider, RenderSettings renderSettings)
		{
			Parser = new DiagramSyntax();
			Renderer = new DiagramRenderer(provider, renderSettings);
			Validator = new DiagramValidator(provider, renderSettings);
		}

		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public IValidator Validator { get; }
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA diagram";
		public ExtensionName Name => NAME;
		public bool IsSummary => false;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new DiagramBlockParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
