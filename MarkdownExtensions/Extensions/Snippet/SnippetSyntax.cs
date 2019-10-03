namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var snippetReference = new Snippet();
			if (text.StartsWith("="))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.AsSibling;
				text = text.Substring(1, text.Length - 1);
			}
			else if (text.StartsWith(">"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.AsChild;
				text = text.Substring(1, text.Length - 1);
			}
			else if (text.StartsWith("######"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H6;
				text = text.Substring(6, text.Length - 6);
			}
			else if (text.StartsWith("#####"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H5;
				text = text.Substring(5, text.Length - 5);
			}
			else if (text.StartsWith("####"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H4;
				text = text.Substring(4, text.Length - 4);
			}
			else if (text.StartsWith("###"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H3;
				text = text.Substring(3, text.Length - 3);
			}
			else if (text.StartsWith("##"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H2;
				text = text.Substring(2, text.Length - 2);
			}
			else if (text.StartsWith("#"))
			{
				snippetReference.InsertionMechanism = InsertionMechanism.H1;
				text = text.Substring(1, text.Length - 1);
			}
			else
			{
				return new ParseFailure(0, "Expected [>], [=] or a heading [#, ##, ###, ####, #####, #######]");
			}
			string[] split = text.Split(':');
			snippetReference.FileName = split[0];
			snippetReference.HeadingName = split[1];
			return new ParseSuccess(snippetReference);
		}
	}
	//public class SnippetInlineSyntax : IParser
	//{
	//	public IParseResult Parse(string text)
	//	{
	//		var snippetReference = new Snippet();
	//		if (text.StartsWith("="))
	//		{
	//			snippetReference.AsSiblingHeading = true;
	//			text = text.Substring(1, text.Length - 1);
	//		}
	//		else if (text.StartsWith(">"))
	//		{
	//			snippetReference.AsChildHeading = true;
	//			text = text.Substring(1, text.Length - 1);
	//		}
	//		var split = text.Split(':');
	//		snippetReference.FileName = split[0];
	//		snippetReference.HeadingName = split[1];
	//		return new ParseSuccess(snippetReference);
	//	}
	//}
}
