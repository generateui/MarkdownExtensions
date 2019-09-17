using System;
using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.XmlSnippet
{
	public class XmlSnippetExtension : IExtension
	{
		public XmlSnippetExtension(RenderSettings renderSettings)
		{
			Parser = new XmlSnippetSyntax();
			Transformer = new XmlSnippetTransformer(renderSettings);
			Validator = new XmlSnippetValidator(renderSettings);
		}

		public ExtensionName Name => NAME;
		private static readonly ExtensionName NAME = "xml snippet";
		public bool IsSummary => throw new NotImplementedException();

		public IParser Parser { get; }
		public IValidator Validator { get; }
		public IRenderer Renderer => null;
		public ITransformer Transformer { get; }

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new XmlSnippetParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
