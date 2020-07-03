using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	public class DatamodelApiSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var packagePath = text;//.Trim(); // breaks
			var lines = packagePath.Split('\n');
			var datamodelApi = new DatamodelApi();
			foreach (var line in lines)
			{
				var sanitized = line.ToLower();
                if (sanitized.StartsWith("package: "))
                {
                    datamodelApi.PackagePath = line.Substring(9);
                }
                if (sanitized.StartsWith("enums-package: "))
                {
                    datamodelApi.EnumsPackagePath = line.Substring(15);
                }
                if (sanitized.StartsWith("filename: "))
				{
					datamodelApi.FileName = line.Substring(10);
				}
			}
			return new ParseSuccess(datamodelApi);
		}
	}
}
