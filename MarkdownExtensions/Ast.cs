using Markdig.Syntax;
using System.Collections.Generic;
using MD = Markdig.Syntax;

namespace MarkdownExtensions
{
	public class Document : IContainerBlock
	{
		public Document(params IBlock[] blocks)
		{
			Blocks = blocks;
		}
		public IEnumerable<IBlock> Blocks { get; }
	}
	public interface IBlock
	{
		IEnumerable<IInline> Inlines { get; }
	}
	public interface IContainerBlock
	{
		IEnumerable<IBlock> Blocks { get; }
	}
	public interface IInline
	{

	}
	public class Paragraph : IBlock
	{
		public Paragraph(params IInline[] inlines)
		{
			Inlines = inlines;
		}
		public IEnumerable<IInline> Inlines { get; }
	}
	public class UnorderedList : IContainerBlock
	{
		public UnorderedList(params ListItem[] listItems)
		{
			ListItems = listItems;
		}
		public IEnumerable<ListItem> ListItems { get; }
		public IEnumerable<IBlock> Blocks => ListItems;
	}
	public class OrderedList { }
	public class ListItem : IBlock
	{
		public ListItem(params IInline[] inlines)
		{
			Inlines = inlines;
		}
		public IEnumerable<IInline> Inlines { get; }
	}
	public class ThematicBreak { }
	public class Heading
	{
		public Heading(int level, params IInline[] inlines)
		{
			Level = level;
			Inlines = inlines;
		}
		public Heading(int level, IInline inline)
		{
			Level = level;
			Inlines = new[] { inline };
		}
		public IEnumerable<IInline> Inlines { get; }
		public int Level { get; }
	}
	public class Heading1 : Heading
	{
		public Heading1(string title) : base(1, new Text(title)) { }
	}
	public class BlankLine { }

	public class Image { }
	public class CodeSpan { }
	public class Emphasis { }
	public class StrongEmphasis { }
	public class Link { }
	public class Autolink { }
	public class HardLineBreak { }
	public class SoftLineBreak { }
	public class Text : IInline
	{
		public Text(string value)
		{
			Value = value;
		}
		public string Value { get; }
	}

	public interface IAstVisitor
	{
		void Visit(Document document);
		void Visit(Paragraph paragraph);
		void Visit(Heading heading);
		void Visit(Text text);
	}
	public class MarkdigVisitor : IAstVisitor
	{
		private MD.Block _block;
		public MarkdownDocument Document { get; private set; }
		public MarkdigVisitor()
		{

		}

		public void Visit(Document document)
		{
			Document = new MarkdownDocument();
		}

		public void Visit(Paragraph paragraph)
		{
			var paragraphBlock = new MD.ParagraphBlock();
			_block = paragraphBlock;
			foreach (IInline inline in paragraph.Inlines)
			{
				//Visit(inline);
			}
			Document.Add(paragraphBlock);
		}

		public void Visit(Heading heading)
		{
			throw new System.NotImplementedException();
		}

		public void Visit(Text text)
		{
			throw new System.NotImplementedException();
		}
	}
}
