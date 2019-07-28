using MarkdownExtensions;
using System.Collections.Generic;

namespace MarkdownExtension.GitHistory
{
	public class GitHistorySyntax : IParser
	{
		/* branch (optional, default: master)
			* format (optional, sane default)
			* tag
			* commits
			* 
			* ```git-history:
			* show = "tags" #commits
			* fields = "date{d}|tag|message|author{name}"
			* ```
			*/
		public IParseResult Parse(string text)
		{
			var toml = Toml.Toml.Parse(text);
			string fieldsText = toml.fields as string;
			var split = fieldsText.Split('|');
			var fields = new List<IField>();
			foreach (var field in split)
			{
				if (field.StartsWith("date"))
				{
					string formatString = null;
					if (field.Contains("{") && field.Contains("}"))
					{
						formatString = field.Substring(5, field.Length - 6 - 1);

					}
					fields.Add(new Date(formatString));
					continue;
				}
				if (field.StartsWith("tag"))
				{
					fields.Add(new Tag());
					continue;
				}
				if (field.StartsWith("message"))
				{
					fields.Add(new Message());
					continue;
				}
				if (field.StartsWith("author"))
				{
					fields.Add(new Author());
					continue;
				}
			}
			return new ParseSuccess(new GitHistory { Fields = fields });
		}
	}
}
