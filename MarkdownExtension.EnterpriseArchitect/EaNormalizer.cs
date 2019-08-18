using Markdig;
using Markdig.Extensions.Hardlines;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect
{
	internal class EaNormalizer
	{
		private readonly Html2Markdown.Converter _converter;
		public EaNormalizer()
		{
			_converter = new Html2Markdown.Converter();
		}

		// notes may contain html and markdown, so to include:
		// 1. Convert html into markdown
		// 2. Parse markdown into ast
		// 3. Normalize headings
		// 4. Convert ast into markdown text
		// 5. Add markdown text to stream
		public string Normalize(string text)
		{
			string markdownText = _converter.Convert(text);
			MarkdownDocument markdown = Markdown.Parse(markdownText);
			using (var writer = new StringWriter())
			{
				var pipeline = new MarkdownPipelineBuilder().Build();
				pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
				var renderer = new NormalizeRenderer(writer);
				pipeline.Setup(renderer);
				renderer.Render(markdown);
				writer.Flush();
				return writer.ToString();
			}
		}
	}
}
