using CommonMark;
using CommonMark.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace MarkdownExtensions.UnitTest
{
	[TestClass]
	public class CreateTestCases
	{
		private const char UNIT_SEPARATOR = '␟';
		public static readonly string SEPARATOR = UNIT_SEPARATOR + Environment.NewLine;

		private void UpdateTestCases()
		{
			var uri = "https://raw.githubusercontent.com/commonmark/commonmark-spec/master/spec.txt";
			var snippets = new List<string>();
			using (var client = new WebClient())
			{
				client.DownloadFile(uri, "test.txt");
				var lines = File.ReadAllLines("test.txt");
				string snippet = null;
				foreach (var line in lines)
				{
					if (Equals(line, "```````````````````````````````` example"))
					{
						snippet = "";
						continue;
					}
					if (Equals(line, "."))
					{
						snippets.Add(snippet);
						snippet = null;
						continue;
					}
					if (snippet != null)
					{
						snippet += line + Environment.NewLine;
					}
				}
			}
			var derp = "";
			foreach (var snippet in snippets)
			{
				derp += snippet;
				derp += Environment.NewLine;
				derp += UNIT_SEPARATOR;
				derp += Environment.NewLine;
			}
			File.WriteAllText("commonmark-test-cases.txt", derp);
		}

		[TestMethod]
		public void DownloadAndParseTestCasesFile()
		{
			var tests = File.ReadAllText("commonmark-test-cases.txt")
				.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
			var failed = new List<string>();
			int succeeded = 0;
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
					if (!Equals(text, test))
					{
						failed.Add(test);
					}
					else
					{
						succeeded += 1;
					}
				}
			}
			Console.WriteLine($@"succeeded: {succeeded}");
			Assert.AreEqual(0, failed.Count);
		}
	}
}
