namespace MarkdownExtensions.Extensions.Snippet
{
	public class Snippet
	{
		public string FileName { get; set; }
		public string HeadingName { get; set; }
		public bool AsSiblingHeading { get; set; } // =
		public bool AsChildHeading { get; set; } // >
	}
}
