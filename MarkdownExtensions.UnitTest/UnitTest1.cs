using CommonMark;
using CommonMark.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;

namespace MarkdownExtensions.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PrintAst()
        {
            var markdown = @"- item
	- ```
		block
		```
";
            var parseSettings = CommonMarkSettings.Default.Clone();
            parseSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            Block block = null;
            using (var reader = new StringReader(markdown))
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                block = CommonMarkConverter.ProcessStage1(reader, parseSettings);
                CommonMarkConverter.ProcessStage2(block, parseSettings);
            }
            var mw = new MarkdownWriter();
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                Printer.PrintBlocks(writer, block, parseSettings);
                System.Diagnostics.Debug.WriteLine(writer.ToString());
                Console.WriteLine(writer.ToString());
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
            var markdown = @"# h1
paragraphh1
## h2
paragraphh2 **bold** *emphasis* ~~strikethrough~~
- list item 1
	* list item 1.1
		- list item 1.1.1
		- list *item* ~~1.1.2~~
	* list item 1.2
- list item 2
";
            var parseSettings = CommonMarkSettings.Default.Clone();
            parseSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            Block block = null;
            using (var reader = new StringReader(markdown))
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                block = CommonMarkConverter.ProcessStage1(reader, parseSettings);
                CommonMarkConverter.ProcessStage2(block, parseSettings);
            }
            var mw = new MarkdownWriter();
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                mw.Write(writer, block);
                string text = writer.ToString();
                Assert.AreEqual(markdown, text);
            }
        }

		[DeploymentItem("TestCases.txt")]
		[TestMethod]
		public void TestAllCases()
		{
			var tests = File.ReadAllText("TestCases.txt")
				.Split(new[] { CreateTestCases.SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var test in tests)
			{
				var parseSettings = CommonMarkSettings.Default.Clone();
				parseSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
				Block block = null;
				using (var reader = new StringReader(test))
				using (var writer = new StringWriter(CultureInfo.CurrentCulture))
				{
					block = CommonMarkConverter.ProcessStage1(reader, parseSettings);
					CommonMarkConverter.ProcessStage2(block, parseSettings);
				}
				var mw = new MarkdownWriter();
				using (var writer = new StringWriter(CultureInfo.CurrentCulture))
				{
					mw.Write(writer, block);
					string text = writer.ToString();
					Assert.AreEqual(test, text);
				}
			}
		}
	}
}
