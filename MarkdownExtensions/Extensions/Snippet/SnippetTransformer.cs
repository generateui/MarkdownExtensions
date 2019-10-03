using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using IO = System.IO;

namespace MarkdownExtensions.Extensions.Snippet
{
	public static class ContainerInlineExtensions
	{
		public static string GetHeadingName(this ContainerInline containerInline)
		{
			// this should produce just the text of the heading inline. Its probably broken 
			// for more complex inline handling (i.e. "my **title**")
			return containerInline.FirstChild.ToString();
		}
	}
	public static class BlockExtensions
	{
		public static int GetHeadingLevel(this Block block)
		{
			ContainerBlock parent = block.Parent;
			if (parent != null)
			{
				int index = parent.IndexOf(block);
				for (int i = index; i > -1; i--)
				{
					if (parent[i] is HeadingBlock headingBlock)
					{
						return headingBlock.Level;
					}
				}
			}
			if (block is ContainerBlock containerBlock)
			{
				int level = 7;
				foreach (var child in containerBlock)
				{
					if (child is HeadingBlock headingBlock)
					{
						if (headingBlock.Level < level)
						{
							level = headingBlock.Level;
						}
					}
				}
				return level == 7 ? 0 : level;
			}

			return 0;
		}
	}
	public static class ContainerBlockExtensions
	{
		public static void IncreaseHeadingLevel(this ContainerBlock containerBlock, int levelDiff)
		{
			if (levelDiff == 0)
			{
				return;
			}
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
		private readonly RenderSettings _renderSettings;

		public SnippetTransformer(RenderSettings renderSettings)
		{
			_renderSettings = renderSettings;
		}
		public override void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, SnippetBlock block, Snippet astNode)
		{
			string fullFilePath = IO.Path.Combine(_renderSettings.SourceFolder.Absolute.FullPath, astNode.FileName);
			string content = IO.File.ReadAllText(fullFilePath);
			MarkdownDocument document = Markdown.Parse(content, extensionHtmlRenderer.Pipeline);

			MarkdownDocument blockToInsert = GetByHeadingName(document, astNode.HeadingName);
			int contextLevel = block.GetHeadingLevel();
			int levelDiff = 0;
			int blockToInsertLevel = blockToInsert.GetHeadingLevel();
			switch (astNode.InsertionMechanism)
			{
				case InsertionMechanism.AsSibling: levelDiff = contextLevel - blockToInsertLevel; break;
				case InsertionMechanism.AsChild: levelDiff = contextLevel - blockToInsertLevel + 1; break;
				case InsertionMechanism.H1: levelDiff = 1 - blockToInsertLevel; break;
				case InsertionMechanism.H2: levelDiff = 2 - blockToInsertLevel; break;
				case InsertionMechanism.H3: levelDiff = 3 - blockToInsertLevel; break;
				case InsertionMechanism.H4: levelDiff = 4 - blockToInsertLevel; break;
				case InsertionMechanism.H5: levelDiff = 5 - blockToInsertLevel; break;
				case InsertionMechanism.H6: levelDiff = 6 - blockToInsertLevel; break;
			}
			// what to do with child headings past 6? Bound to 6?
			blockToInsert.IncreaseHeadingLevel(levelDiff);
			Replace(block, blockToInsert);
		}

		// Note: this cannot be made an extension method as the parent is set
		internal static MarkdownDocument GetByHeadingName(MarkdownDocument source, string name)
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
