using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtension.GitGraph.UnitTest
{
    [TestClass]
    public class SyntaxTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var text = "herpy derp derp";
            var history = (Syntax.History)new Syntax().Parse(text);

            Assert.IsTrue(history.Commits.Count == 1);
            Assert.IsTrue(history.Commits[0].Title == text);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var text = "[master]herpy derp derp";
            var history = (Syntax.History)new Syntax().Parse(text);

            Assert.IsTrue(history.Commits.Count == 1);
            Assert.AreEqual(history.Commits[0].Title, "herpy derp derp");
            Assert.AreEqual(history.Commits[0].BranchName.Name, "master");
        }

        private class Tests : Dictionary<string, Syntax.History>
        {
            public void Add(string text, params Syntax.Commit[] commits)
            {
                Add(text, new Syntax.History { Commits = commits.ToList() });
            }
        }

        [TestMethod]
        public void TestAll()
        {
            var tests = new Tests
            {
                //{ "[master]herpy derp derp",
                //    new Commit {
                //        Title = "herpy derp derp",
                //        BranchName = new BranchName("master")} },
                //{ "[master] herpy derp derp",
                //    new Commit {
                //        Title = "herpy derp derp",
                //        BranchName = new BranchName("master")} },
                //{ "[branch] #tag# first commit",
                //    new Commit {
                //        Title = "first commit",
                //        BranchName = new BranchName("branch"),
                //        Tag = new Tag("tag") } },
                //{ "[branch] {abcd1234} #tag# first commit",
                //    new Commit {
                //        Title = "first commit",
                //        BranchName = new BranchName("branch"),
                //        Tag = new Tag("tag"),
                //        CommitId = new CommitId("abcd1234") } },
                { @"[branch1] -> [branch2] {abcd1234} #tag# first commit @""Ruud Poutsma""<rtimon@gmail.com>",
                    new Syntax.Commit {
                        Title = "first commit",
                        BranchName = new Syntax.BranchName("branch1"),
                        Branch = new Syntax.BranchName("branch2"),
                        Tag = new Syntax.Tag("tag"),
                        CommitId = new Syntax.CommitId("abcd1234"),
                        Author = new Syntax.Author("Ruud Poutsma", "rtimon@gmail.com") } },
            };
            foreach (var test in tests)
            {
                var history = (Syntax.History)new Syntax().Parse(test.Key);
                Assert.AreEqual(test.Value, history);
            }
        }

        [TestMethod]
        public void TestAuthor()
        {
            var authorText = "@\"Ruud Poutsma\"<rtimon@gmail.com>";
            int i = 1;
            var author = Syntax.Author.Parse(authorText, ref i);
            Assert.AreEqual(author.Name, "Ruud Poutsma");
            Assert.AreEqual(author.Email, "rtimon@gmail.com");
        }
    }
}
