using JetBrains.Annotations;
using Markdig.Syntax;

namespace MarkdownExtensions
{
	public interface IExtensionBlock { }
	public interface IExtensionInline
	{
		string Content { get; }
	}
	public static class ExtensionBlockExtensions
	{
		public static string GetContent(this IExtensionBlock extensionBlock)
		{
			var fencedCodeBlock = extensionBlock as FencedCodeBlock;
			return fencedCodeBlock.Lines.ToString();
		}
	}
    public interface IExtension : Markdig.IMarkdownExtension
    {
        ExtensionName Name { get; }

        /// <summary>
        /// Parse given content of [prefix:{content}] or ```prefix:\n{content}```.
        /// </summary>
        IParser Parser { get; }

		/// <summary>
		/// Using the given parse tree, validate the parse tree content
		/// </summary>
		/// For example when referring to an external file, return an error if that file 
		/// does not exist.
		[CanBeNull] IValidator Validator { get; }

		/// <summary>
		/// Render html using the model
		/// </summary>
		[CanBeNull] IRenderer Renderer { get; }

		[CanBeNull] ITransformer Transformer { get; }
    }

    public class ExtensionName// alias string
    {
        public ExtensionName(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => Name.ToString();
        public static implicit operator ExtensionName(string name) => new ExtensionName(name);
    }

    public interface IFormatState
    {
        int HeadingLevel { get; }
		// listlevel?
		// in blockquote?
	}
	public class FormatState : IFormatState
    {
        public int HeadingLevel { get; set; }
    }

    public class Position
    {
        public Position(int line, int column)
        {
            Line = line;
            Column = column;
        }
        public int Line { get; }
        public int Column { get; }
        public override string ToString() => $@"Line {Line},{Column}";
    }
	// TODO: use markdig class instead
    public class Range
    {
        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }
        Position Start { get; }
        Position End { get; }
        public override string ToString() => $@"[{Start} → {End}]";
    }
}
