using Markdig.Syntax;

namespace MarkdownExtensions
{
	public interface ITransformer
	{
		void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, FencedCodeBlock block, object astNode);
	}

	public interface ITransformer<TBlock, TModel> : ITransformer
		where TBlock : FencedCodeBlock, IExtensionBlock
		where TModel : class
	{
		void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, TBlock block, TModel astNode);
	}

	public abstract class TransformerBase<TBlock, TModel> : ITransformer<TBlock, TModel>
		where TBlock : FencedCodeBlock, IExtensionBlock
		where TModel : class
	{
		public abstract void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, TBlock block, TModel astNode);

		public void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, FencedCodeBlock block, object astNode)
		{
			var typedBlock = block as TBlock;
			var typedAstNode = astNode as TModel;
			Transform(extensionHtmlRenderer, typedBlock, typedAstNode);
		}
		protected void Replace(Block block, MarkdownDocument document)
		{
			int index = block.Parent.IndexOf(block);
			foreach (Block child in document)
			{
				// hack since setting the parent is `internal`
				System.Reflection.PropertyInfo property = child.GetType().GetProperty("Parent");
				property.SetValue(child, null);
				block.Parent.Insert(index, child);
				index += 1;
			}
			block.Parent.RemoveAt(index);
		}
	}
}
