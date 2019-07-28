using MarkdownExtensions;

namespace MarkdownExtension.PanZoomImage
{
	public class PanZoomImageParser : BlockExtensionParser<PanZoomImageBlock>
	{
		public PanZoomImageParser()
		{
			InfoPrefix = "pan-zoom-image";
			_create = _ => new PanZoomImageBlock(this);
		}
	}
}
