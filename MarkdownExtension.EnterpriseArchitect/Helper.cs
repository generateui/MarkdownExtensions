using Markdig;
using Markdig.Extensions.Hardlines;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using System;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect
{
	public static class Helper
	{
		public static string Converter(string mixedHtmlAndMarkdown, Action<MarkdownDocument> transform)
		{
			var converter = new Html2Markdown.Converter();
			string markdownOnly = converter.Convert(mixedHtmlAndMarkdown);
			MarkdownDocument ast = Markdown.Parse(markdownOnly);
			transform(ast);
			using (var writer = new StringWriter())
			{
				var pipeline = new MarkdownPipelineBuilder().Build();
				pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
				var renderer = new NormalizeRenderer(writer);
				pipeline.Setup(renderer);
				renderer.Render(ast);
				writer.Flush();
				return writer.ToString();
			}
		}
	}
}
