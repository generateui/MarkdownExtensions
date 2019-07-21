using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarkdownExtensions
{
    public class MarkdownWriter
    {
        public void Write(TextWriter writer, Block block)
        {
            PrintBlocks(writer, block);
        }

        private static string format_str(string s)
        {
			if (s == null)
			{
				return string.Empty;
			}
            var buffer = new StringBuilder();
            int pos = 0;
            int len = s.Length;
            char c;

            buffer.Length = 0;
            while (pos < len)
            {
                c = s[pos];
                switch (c)
                {
                    case '\n':
                        buffer.Append("\\n");
                        break;
                    case '"':
                        buffer.Append("\\\"");
                        break;
                    case '\\':
                        buffer.Append("\\\\");
                        break;
                    default:
                        buffer.Append(c);
                        break;
                }
                pos++;
            }
            return buffer.ToString();
        }

        public static void PrintBlocks(TextWriter writer, Block block)
        {
            var nodes = block.AsEnumerable().ToList();
            int listIndent = -1;
			var bullet = new Stack<char>();
            foreach (var node in nodes)
            {
                if (node.Block != null)
                {
					switch (node.Block.Tag)
					{
						case BlockTag.Document:
							if (node.IsClosing)
							{
								//writer.WriteLine();
							}
							break;
						case BlockTag.Paragraph:
							if (node.IsClosing)
							{
								writer.WriteLine();
							}
							break;
						case BlockTag.AtxHeading:
							if (node.IsOpening)
							{
								var prefix = new string('#', node.Block.Heading.Level);
								writer.Write(prefix + " ");
							}
							else
							{
								writer.WriteLine();
							}
							break;
						case BlockTag.List:
							if (node.IsOpening)
							{
								bullet.Push(node.Block.ListData.BulletChar);
								listIndent += 1;
							}
							else
							{
								listIndent -= 1;
								bullet.Pop();
							}
							break;
						case BlockTag.ListItem:
							if (node.IsOpening)
							{
								var tab = new string('\t', listIndent);
								writer.Write(tab + bullet.Peek() + " ");
							}
							break;
						case BlockTag.FencedCode:
							writer.Write("```");
							var tag = node.Block.FencedCodeData.Info;
							if (tag != null)
							{
								writer.Write(tag);
							}
							writer.WriteLine();
							string content = node.Block.StringContent.ToString();
							content = content.Replace("\n", Environment.NewLine);
							writer.Write(content);
							writer.WriteLine("```");
							break;
					}
                }
                else if (node.Inline != null)
                {
                    switch (node.Inline.Tag)
                    {
                        case InlineTag.String:
                        case InlineTag.Code:
                        case InlineTag.RawHtml:
                            writer.Write(format_str(node.Inline.LiteralContent));
                            break;
                        case InlineTag.LineBreak:
                        case InlineTag.SoftBreak:
                            writer.WriteLine(Environment.NewLine);
                            break;

                        case InlineTag.Image:
							writer.Write($"![{node.Inline.LiteralContent}]({node.Inline.TargetUrl})");
							break;
                        case InlineTag.Link:
							if (node.IsOpening)
							{
								writer.Write("[");
							}
							else
							{
								writer.Write($"]({node.Inline.TargetUrl})");
							}
							break;
                        case InlineTag.Strong:
                            writer.Write("**");
                            break;
                        case InlineTag.Emphasis:
                            writer.Write("*");
                            break;
                        case InlineTag.Strikethrough:
                            writer.Write("~~");
                            break;
                        case InlineTag.Placeholder:
                            if (node.IsOpening)
                            {
                                writer.Write("[");
                            }
                            else
                            {
                                writer.Write("]");
                            }
                            break;
                    }
                }
            }
        }

    }
}
