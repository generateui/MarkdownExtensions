using MarkdownExtensions.GenericSyntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MarkdownExtensions.UnitTest
{
	[TestClass]
	public class CssLenghtUnitTest
	{
		private class Dict : Dictionary<string, CssLenghtUnit>
		{
			public void Add(string text, float value, CssLength cssLength)
			{
				Add(text, new CssLenghtUnit(value, cssLength));
			}
		}

		[TestMethod]
		public void TestCorrect()
		{
			var ok = new Dict()
			{
				{ "100px", 100, CssLength.px },
				{ "5em", 5, CssLength.em },
			};
			foreach (KeyValuePair<string, CssLenghtUnit> testCase in ok)
			{
				CssLenghtUnit parsed = CssLenghtUnit.Parse(testCase.Key);
				Assert.AreEqual(testCase.Value, parsed);
			}
		}
	}
}
