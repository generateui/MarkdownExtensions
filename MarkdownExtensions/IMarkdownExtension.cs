using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtensions
{
    public interface IMarkdownExtension
    {
        /// <summary>
        /// Prefix of the extension i.e. the text before the colon
        /// </summary>
        /// <example>
        /// [folder:c:\MyFolder]
        /// </example>
        string Prefix { get; }

        MarkdownExtensionName Name { get; }

        /// <summary>
        /// Produce html or markdown
        /// </summary>
        /// Html will be directly output to the resulting html document.
        /// Markdown will be either be output as markdown or as html,
        /// depending on the desired output of the document.
        Output Output { get; }

        /// <summary>
        /// <see cref="ElementType.Block"/>, <see cref="ElementType.Inline"/>
        /// </summary>
        /// <remarks><see cref="ElementType.Document"/> is not yet supported</remarks>
        IElementType Type { get; }

        /// <summary>
        /// Parse given content of [prefix:{content}] or ```prefix:\n{content}```.
        /// </summary>
        ISyntax Syntax { get; }

        /// <summary>
        /// Using the given parse tree, validate the parse tree content
        /// </summary>
        /// For example when referring to an external file, return an error if that file 
        /// does not exist.
        IValidator Validator { get; }

        /// <summary>
        /// Using the parse tree return html represnting the parse tree
        /// </summary>
        IFormatter Formatter { get; }
    }

    public enum Output
    {
        Markdown,
        Html,
        //MarkdownAst,
        //HtmlAst
    }

    public class FormatResult
    {
        public Html Html { get; private set; }
        public Markdown Markdown { get; private set; }
        // TODO: markdown ast
        // TODO: html ast
        public static FormatResult FromHtml(string html) => new FormatResult { Html = html };
        public static FormatResult FromMarkdown(string markdown) => new FormatResult { Markdown = markdown };
    }

    /// <summary>
    /// Represents a type-safe html string by wrapping a string
    /// </summary>
    public sealed class Html
    {
        public string Value { get; }
        public Html(string value)
        {
            Value = value;
        }
        public static implicit operator Html (string value) => new Html(value);
    }

    /// <summary>
    /// Represents a type-safe markdown string by wrapping a string
    /// </summary>
    public sealed class Markdown
    {
        public string Value { get; }
        public Markdown(string value)
        {
            Value = value;
        }
        public static implicit operator Markdown(string value) => new Markdown(value);
    }

    public interface ICodeByName : IDictionary<MarkdownExtensionName, string> { }
    public class CodeByName : Dictionary<MarkdownExtensionName, string>, ICodeByName
    {
        public CodeByName()
        {
        }
        public CodeByName(MarkdownExtensionName name, string code)
        {
            Add(name, code);
        }

        public void Add(ICodeByName other)
        {
            if (other == null)
            {
                return;
            }
            foreach (var entry in other)
            {
                if (!ContainsKey(entry.Key))
                {
                    Add(entry.Key, entry.Value);
                }
            }
        }

        public static CodeByName Combine(ICodeByName left, ICodeByName right)
        {
            var result = new CodeByName();
            foreach (var item in left)
            {
                result.Add(item.Key, item.Value);
            }
            foreach (var item in right)
            {
                if (!result.ContainsKey(item.Key))
                {
                    result.Add(item.Key, item.Value);
                }
            }
            return result;
        }
    }

    public class MarkdownExtensionName
    {
        public MarkdownExtensionName(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public override int GetHashCode() => Name.GetHashCode();
        public override string ToString() => Name.ToString();
        public static implicit operator MarkdownExtensionName(string name) => new MarkdownExtensionName(name);
    }

    public interface IFormatState
    {
        int HeadingLevel { get; }
    }
    public class FormatState : IFormatState
    {
        public int HeadingLevel { get; set;}
    }
    public class FormatSettings
    {
        /// <summary>
        /// Forces extensions to re-query their data sources instead of using cached data
        /// </summary>
        public bool ForceRefreshData { get; set; }
    }
    public interface IFormatter
    {
        FormatResult Format(object root, IFormatState state);

        ICodeByName GetCss();
        ICodeByName GetJs();
    }
    public interface ISyntax
    {
        IParseResult Parse(string text);
    }
    public interface IValidator
    {
        IErrors Validate(object tree, SourceSettings sourceSettings);
    }
    public interface IParseResult
    {
        object Object { get; }
        IErrors Errors { get; }
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
    public interface IError
    {
        string Message { get; }
    }
    public interface IParseError : IError
    {
        Range Range { get; }
    }
    public class ParseError : IParseError
    {
        public ParseError(Range range, string message)
        {
            Range = range;
            Message = message;
        }
        public Range Range { get; }
        public string Message { get; }
    }
    public class Error : IError
    {
        public Error(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
    public interface IErrors
    {
        IEnumerable<IError> Errors { get; }
    }
    public class NoErrors : IErrors
    {
        public IEnumerable<IError> Errors => Enumerable.Empty<IError>();
    }
    public class ValidationFailure : IErrors
    {
        public ValidationFailure(params IError[] errors)
        {
            Errors = errors;
        }
        public ValidationFailure(IEnumerable<IError> errors)
        {
            Errors = errors;
        }
        public IEnumerable<IError> Errors { get; }
    }
    public class Valid : IErrors
    {
        public IEnumerable<IError> Errors => Enumerable.Empty<IError>();
    }

    public class ParseSuccess : IParseResult
    {
        public ParseSuccess(object obj)
        {
            Object = obj;
        }
        public static IErrors None = new NoErrors();
        public object Object { get; }
        public IErrors Errors => None;
    }
    public class ParseFailure : IParseResult
    {
        private class ErrorsImpl : IErrors
        {
            public ErrorsImpl(IEnumerable<IError> errors)
            {
                Errors = errors;
            }
            public IEnumerable<IError> Errors { get; }
        }
        public ParseFailure(IErrors errors)
        {
            Errors = errors;
        }
        public ParseFailure(params IError[] errors)
        {
            Errors = new ErrorsImpl(errors);
        }
        public object Object => null;
        public IErrors Errors { get; }
    }

    public interface IElementType { }
    public static class ElementType
    {
        private class DocumentType : IElementType { }
        private class BlockType : IElementType { }
        private class InlineType : IElementType { }
        static ElementType()
        {
            Document = new DocumentType();
            Block = new BlockType();
            Inline = new InlineType();
        }
        public static IElementType Document { get; private set; }
        public static IElementType Block { get; private set; }
        public static IElementType Inline { get; private set; }
    }
}
