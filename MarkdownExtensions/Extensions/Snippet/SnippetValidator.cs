using Markdig;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetValidator : ValidatorBase<Snippet>
	{
		public override IErrors ValidateTyped(Snippet tree, ValidationContext context)
		{
			var file = new File(context.RenderSettings.SourceFolder, tree.FileName);
			if (!file.Exists())
			{
				return new ValidationFailure($@"Could not find file named [{tree.FileName}]");
			}
			string content = System.IO.File.ReadAllText(file.AbsolutePath);
			MarkdownDocument document = Markdown.Parse(content, context.Pipeline);
			var blockToInsert = SnippetTransformer.GetByHeadingName(document, tree.HeadingName);
			if (blockToInsert.Count == 0)
			{
				return new ValidationFailure($@"Could not find heading named [{tree.HeadingName}] in the document [{tree.FileName}]");
			}
			return new NoErrors();
		}
	}
}
