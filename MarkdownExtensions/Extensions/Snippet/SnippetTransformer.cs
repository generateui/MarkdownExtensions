using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.IO;

namespace MarkdownExtensions.Extensions.Snippet
{
	public static class ContainerInlineExtensions
	{
		public static string GetHeadingName(this ContainerInline containerInline)
		{
			// this should produce just the text of the heading inline
			return containerInline.FirstChild.ToString();
		}
	}
	public static class BlockExtensions
	{
		public static int GetHeadingLevel(this Block block)
		{
			ContainerBlock parent = block.Parent;
			int index = parent.IndexOf(block);
			for (int i = index; i > -1; i--)
			{
				if (parent[i] is HeadingBlock headingBlock)
				{
					return headingBlock.Level;
				}
			}
			return 0;
		}
	}
	public static class ContainerBlockExtensions
	{
		public static void IncreaseHeadingLevel(this ContainerBlock containerBlock, int levelDiff)
		{
			foreach (Block child in containerBlock)
			{
				if (child is HeadingBlock headingBlock)
				{
					headingBlock.Level += levelDiff;
				}
			}
		}
	}
	public class SnippetTransformer : TransformerBase<SnippetBlock, Snippet>
	{
		public override void Transform(SnippetBlock block, Snippet astNode)
		{
			string content = File.ReadAllText(astNode.FileName);
			MarkdownDocument document = Markdown.Parse(content);

			int level = block.GetHeadingLevel();
			if (level > 0 && astNode.AsSiblingHeading)
			{
				level -= 1;
			}
			document.IncreaseHeadingLevel(level);
			if (astNode.HeadingName != null)
			{
				document = GetByHeadingName(document, astNode.HeadingName);
			}
			Replace(block, document);
		}

		// Note: this cannot be made an extension method as the parent is set
		private MarkdownDocument GetByHeadingName(MarkdownDocument source, string name)
		{
			bool selecting = false;
			int level = 0;
			var document = new MarkdownDocument();
			foreach (var block in source)
			{
				if (!selecting && 
					block is HeadingBlock headingBlock1 && 
					headingBlock1.Inline.GetHeadingName() == name)
				{
					selecting = true;
					level = headingBlock1.Level;
				} else if (selecting && 
					block is HeadingBlock headingBlock2 &&
					headingBlock2.Level <= level)
				{
					selecting = false;
				}
				if (selecting)
				{
					// throws an exception
					//block.Parent = null;
					// the setter is marked internal so use reflection.
					var type = block.GetType();
					type.GetProperty("Parent").SetValue(block, null, null);
					document.Add(block);
				}
			}
			return document;
		}
	}
}
