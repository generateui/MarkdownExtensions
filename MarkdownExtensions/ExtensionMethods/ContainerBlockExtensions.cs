using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Collections.Generic;

namespace MarkdownExtensions.ExtensionMethods
{
	public static class ContainerBlockExtensions
	{
		public static IEnumerable<IExtensionInline> GetInlinesRecursively(this ContainerBlock block, List<IExtensionInline> list = null)
		{
			list = list ?? new List<IExtensionInline>();
			foreach (var child in block)
			{
				if (child is ContainerBlock containerBlock)
				{
					containerBlock.GetInlinesRecursively(list);
				}
				if (child is LeafBlock leafBlock && leafBlock.Inline != null)
				{
					foreach (Inline inline in leafBlock.Inline)
					{
						if (inline is IExtensionInline extensionInline)
						{
							list.Add(extensionInline);
						}
					}
				}
			}
			return list;
		}

		public static IEnumerable<T> GetRecursivelyOfType<T>(this ContainerBlock block, List<T> list = null)
			where T : IExtensionBlock
		{
			list = list ?? new List<T>();
			if (block is IExtensionBlock extensionBlock)
			{
				list.Add((T)extensionBlock);
			}
			foreach (var child in block)
			{
				if (child is ContainerBlock cb)
				{
					cb.GetRecursivelyOfType(list);
				}
				if (child is IExtensionBlock eb)
				{
					list.Add((T)eb);
				}
			}
			return list;
		}
	}
}
