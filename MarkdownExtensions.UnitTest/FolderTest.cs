using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownExtensions.UnitTest
{
	[TestClass]
	public class FolderTest
	{
		[TestMethod]
		public void OneLevelRelativeFolder_HasCorrectFullPath()
		{
			var relativeFolder = new RelativeFolder(@"folder");
			var absoluteFolder = new AbsoluteFolder(@"c:\");
			var folder = new Folder(absoluteFolder, relativeFolder);
			Assert.AreEqual(@"c:\folder", folder.Absolute.FullPath);
		}

		[TestMethod]
		public void AbsoluteFolder_HasCorrectFullPath()
		{
			var absoluteFolder = new AbsoluteFolder(@"c:\folder");
			var folder = new Folder(absoluteFolder);
			Assert.AreEqual(@"c:\folder", folder.Absolute.FullPath);
		}
	}
}
