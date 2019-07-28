namespace MarkdownExtensions.Extensions.NestedBlock
{
	public class NestedBlockParser : BlockExtensionParser<NestedBlockBlock>
	{
		public NestedBlockParser()
		{
			InfoPrefix = "md-nested-block";
			_create = _ => new NestedBlockBlock(this);
		}
	}
}
