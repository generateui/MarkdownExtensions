namespace MarkdownExtensions.Extensions.XmlSnippet
{
	internal class XmlSnippetSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var sanitized = text.Trim();
			string[] lines = sanitized.Split('\n');
			XmlSnippet xmlSnippet = null;
			foreach (var line in lines)
			{
				string sanitizedLine = line.Trim().ToLower();
				if (line.StartsWith("file: "))
				{
					xmlSnippet = xmlSnippet ?? new XmlSnippet();
					xmlSnippet.File = line.Substring(6);
				}
				if (line.StartsWith("xpath: "))
				{
					xmlSnippet = xmlSnippet ?? new XmlSnippet();
					xmlSnippet.Xpath = line.Substring(7);
				}
			}
			if (xmlSnippet.File == null)
			{
				return new ParseFailure(1, "Expected an xml file to be spcified in the form of [file: {fileName}]");
			}
			if (xmlSnippet.Xpath == null)
			{
				return new ParseFailure(1, "Expected an xpath to be spcified in the form of [xpath: {xpath}]");
			}
			return new ParseSuccess(xmlSnippet);
		}
	}
}
