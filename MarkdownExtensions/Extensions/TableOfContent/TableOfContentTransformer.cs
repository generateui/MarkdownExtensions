using Markdig;
using Markdig.Syntax;
using MarkdownExtensions.Extensions.Snippet;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	internal class TableOfContentTransformer : TransformerBase<TableOfContentBlock, TableOfContent>
	{
		public override void Transform(
			ExtensionHtmlRenderer renderer,
			TableOfContentBlock block,
			TableOfContent astNode)
		{
			List<Heading> headings = renderer.ContainerBlock.GetHeadings();
			var sb = new StringBuilder();
			sb.AppendLine("# Table of Content");
			foreach (var heading in headings)
			{
				var tabs = new string('\t', heading.Level - 1);
				sb.AppendLine(tabs + "- " + heading.Title);
			}
			MarkdownDocument document = Markdown.Parse(sb.ToString());
			Replace(block, document);
		}
	}

	class Heading
	{
		public string Title { get; internal set; }
		public int Level { get; internal set; }
		public HeadingBlock HeadingBlock { get; set; }
	}

	internal static class ContainerBlockExtensions
	{
		internal static List<Heading> GetHeadings(this ContainerBlock containerBlock)
		{
			var result = new List<Heading>();
			foreach (var child in containerBlock)
			{
				if (child is HeadingBlock headingBlock)
				{
					string name = headingBlock.Inline.GetHeadingName();
					result.Add(new Heading
					{
						Title = name,
						Level = headingBlock.Level,
						HeadingBlock = headingBlock
					});
				}
			}
			return result;
		}
	}
}
