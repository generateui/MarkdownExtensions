using System.IO;

namespace MarkdownExtensions
{
	public class RenderSettings
	{
		private string _sourceFolder;

		public string AbsoluteImageFolder { get; private set; }
		public string RelativeImageFolder { get; } = "Images";
		public string SourceFolder
		{
			get => _sourceFolder;
			set
			{
				_sourceFolder = value;
				AbsoluteImageFolder = Path.Combine(SourceFolder, RelativeImageFolder);
			}
		}
	}
}