using Markdig.Syntax;

namespace MarkdownExtensions
{
	public interface ITransformer
	{
		void Transform(FencedCodeBlock block, object astNode);
	}

	public interface ITransformer<TBlock, TModel> : ITransformer
		where TBlock : FencedCodeBlock, IExtensionBlock
		where TModel : class
	{
		void Transform(TBlock block, TModel astNode);
	}

	public abstract class TransformerBase<TBlock, TModel> : ITransformer<TBlock, TModel>
		where TBlock : FencedCodeBlock, IExtensionBlock
		where TModel : class
	{
		public abstract void Transform(TBlock block, TModel astNode);

		public void Transform(FencedCodeBlock block, object astNode)
		{
			var typedBlock = block as TBlock;
			var typedAstNode = astNode as TModel;
			Transform(typedBlock, typedAstNode);
		}
		protected void Replace(Block block, MarkdownDocument document)
		{
			var index = block.Parent.IndexOf(block);
			block.Parent.Insert(index, document);
			block.Parent.RemoveAt(index + 1);
		}
	}
}
