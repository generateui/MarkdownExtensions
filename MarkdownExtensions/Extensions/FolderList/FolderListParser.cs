namespace MarkdownExtensions.Extensions.FolderList
{
	public class FolderListParser : BlockExtensionParser<FolderListBlock>
	{
		public FolderListParser()
		{
			InfoPrefix = "folder";
			_create = _ => new FolderListBlock(this);
		}
	}
}
