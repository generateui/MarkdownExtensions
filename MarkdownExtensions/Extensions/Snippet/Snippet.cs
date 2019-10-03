namespace MarkdownExtensions.Extensions.Snippet
{
	public class Snippet
	{
		public string FileName { get; set; }
		public string HeadingName { get; set; }
		public InsertionMechanism InsertionMechanism { get; set; }
	}
	public enum InsertionMechanism
	{
		AsSibling, // =
		AsChild, // >
		H1, // #
		H2, // ##
		H3, // ###
		H4, // ####
		H5, // #####
		H6  // ######
	}
}
