using CommonMark.Syntax;
using System.Collections.Generic;

namespace MarkdownExtensions.Console
{
    // unused, here for CommonMark.Net API reference
    class CommonMarkNet
    {
        private List<Block> GetCodeBlocks(Block block)
        {
            var blocks = new List<Block>();
            foreach (EnumeratorEntry entry in block.AsEnumerable())
            {
                if (entry.Block != null &&
                    entry.Block.Tag == BlockTag.FencedCode &&
                    entry.IsOpening)
                {
                    blocks.Add(entry.Block);
                }
            }
            return blocks;
        }

        private List<Inline> GetPlaceholders(Block block, List<Inline> links = null)
        {
            links = links ?? new List<Inline>();
            foreach (EnumeratorEntry entry in block.AsEnumerable())
            {
                if (entry.Inline != null &&
                    entry.Inline.Tag == InlineTag.Placeholder &&
                    entry.IsOpening)
                {
                    links.Add(entry.Inline);
                }
            }
            return links;
        }
    }
}
