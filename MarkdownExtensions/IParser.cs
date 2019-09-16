namespace MarkdownExtensions
{
	public interface IParser
    {
        IParseResult Parse(string text);
    }
	public interface IParseResult
	{
		object SyntaxTree { get; }
		IErrors Errors { get; }
	}
}
