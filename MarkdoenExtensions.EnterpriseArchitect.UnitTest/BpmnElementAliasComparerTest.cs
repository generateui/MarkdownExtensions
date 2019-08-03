using MarkdownExtension.EnterpriseArchitect.EaProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MarkdoenExtensions.EnterpriseArchitect.UnitTest
{
	[TestClass]
	public class BpmnElementAliasComparerTest
	{
		[TestMethod]
		public void TestMethod1()
		{
			var e1 = new BpmnElement { Alias = "01" };
			var e2 = new BpmnElement { Alias = "02" };
			var list = new List<BpmnElement> { e1, e2 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("01", list[0].Alias);
			Assert.AreEqual("02", list[1].Alias);
		}

		[TestMethod]
		public void TestMethod2()
		{
			var e1 = new BpmnElement { Alias = "02" };
			var e2 = new BpmnElement { Alias = "01" };
			var list = new List<BpmnElement> { e1, e2 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("01", list[0].Alias);
			Assert.AreEqual("02", list[1].Alias);
		}

		[TestMethod]
		public void TestMethod6()
		{
			var e1 = new BpmnElement { Alias = "02.01" };
			var e2 = new BpmnElement { Alias = "01.02" };
			var e3 = new BpmnElement { Alias = "01.01" };
			var list = new List<BpmnElement> { e1, e2, e3 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("01.01", list[0].Alias);
			Assert.AreEqual("01.02", list[1].Alias);
			Assert.AreEqual("02.01", list[2].Alias);
		}

		[TestMethod]
		public void TestMethod3()
		{
			var e1 = new BpmnElement { Alias = "01" };
			var e2 = new BpmnElement { Alias = "" };
			var list = new List<BpmnElement> { e1, e2 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("01", list[0].Alias);
			Assert.AreEqual("", list[1].Alias);
		}

		[TestMethod]
		public void TestMethod4()
		{
			var e1 = new BpmnElement { Alias = "" };
			var e2 = new BpmnElement { Alias = "01" };
			var list = new List<BpmnElement> { e1, e2 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("01", list[0].Alias);
			Assert.AreEqual("", list[1].Alias);
		}

		[TestMethod]
		public void TestMethod5()
		{
			var e1 = new BpmnElement { Alias = "" };
			var e2 = new BpmnElement { Alias = "" };
			var list = new List<BpmnElement> { e1, e2 };
			list.Sort(new BpmnElement.AliasComparer());

			Assert.AreEqual("", list[0].Alias);
			Assert.AreEqual("", list[1].Alias);
		}
	}
}
