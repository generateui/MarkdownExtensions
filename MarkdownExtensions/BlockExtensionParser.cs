using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using System;

namespace MarkdownExtensions
{
	public abstract class BlockExtensionParser<T> : FencedBlockParserBase<T> where T : Block, IFencedBlock
	{
		protected Func<BlockExtensionParser<T>, T> _create;
		public BlockExtensionParser()
		{
			OpeningCharacters = new[] { '`' };
			MinimumMatchCount = 3;
			MaximumMatchCount = 3;
			InfoParser = (BlockProcessor s, ref StringSlice l, IFencedBlock fb, char oc) => 
				l.ToString() == InfoPrefix;
		}
		protected override T CreateFencedBlock(BlockProcessor processor) => _create(this);
	}
}
