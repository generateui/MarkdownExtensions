using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	internal class DatamodelApiParser : BlockExtensionParser<DatamodelApiBlock>
	{
		public DatamodelApiParser()
		{
			InfoPrefix = "ea-datamodel-api";
			_create = _ => new DatamodelApiBlock(this);
		}
	}
}
