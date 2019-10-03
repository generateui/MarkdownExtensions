using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtensions
{
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

	public class ValidationResult : IErrors
	{
		public ValidationResult(List<IError> errors)
		{
			Errors = errors;
		}
		public IEnumerable<IError> Errors { get; }
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
		public ValidationFailure(string errorMessage)
		{
			Errors = new List<IError> { new Error(errorMessage) };
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
			SyntaxTree = obj;
		}
		public static IErrors None = new NoErrors();
		public object SyntaxTree { get; }
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
		public ParseFailure(int line, string message)
		{
			var range = new Range(new Position(line, 0), new Position(line, 0));
			var error = new ParseError(range, message);
			Errors = new ErrorsImpl(new[] { error });
		}
		public object SyntaxTree => null;
		public IErrors Errors { get; }
	}
}
