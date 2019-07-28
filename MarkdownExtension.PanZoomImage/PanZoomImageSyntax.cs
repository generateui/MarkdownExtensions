using MarkdownExtensions;

namespace MarkdownExtension.PanZoomImage
{
	public class PanZoomImageSyntax : IParser
    {
        public IParseResult Parse(string text)
        {
            return new ParseSuccess(new PanZoomImage { Uri = text.Trim() });
        }
    }
}
