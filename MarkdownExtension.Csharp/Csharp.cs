using MarkdownExtensions;
using System;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Emit;

namespace MarkdownExtension.Csharp
{
    // link to file where cs type exists
    // 1. name of type
    //      1. folder on hdd
    //      2. file on github
    //      3. 
    public class Csharp : IMarkdownExtension
    {
        private class CsharpReference
        {
            public string TypeName { get; set; }
        }
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new CsharpReference { TypeName = text });
            }
        }
        private class FormatterImpl : IFormatter
        {
            public FormatResult Format(object root, IFormatState state)
            {
                var location = @"C:\Users\Ruud\Documents\Visual Studio 2017\Projects\JustSomeTypes.sln";
                var provider = new CSharpCodeProvider();

                return null;
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;

            private static bool CompileSolution(string solutionUrl, string outputDir)
            {
                bool success = true;

                MSBuildWorkspace workspace = MSBuildWorkspace.Create();
                Solution solution = workspace.OpenSolutionAsync(solutionUrl).Result;
                ProjectDependencyGraph projectGraph = solution.GetProjectDependencyGraph();
                Dictionary<string, Stream> assemblies = new Dictionary<string, Stream>();

                foreach (ProjectId projectId in projectGraph.GetTopologicallySortedProjects())
                {
                    Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;
                    if (null != projectCompilation && !string.IsNullOrEmpty(projectCompilation.AssemblyName))
                    {
                        using (var stream = new MemoryStream())
                        {
                            EmitResult result = projectCompilation.Emit(stream);
                            if (result.Success)
                            {
                                string fileName = string.Format("{0}.dll", projectCompilation.AssemblyName);

                                using (FileStream file = File.Create(outputDir + '\\' + fileName))
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                    stream.CopyTo(file);
                                }
                            }
                            else
                            {
                                success = false;
                            }
                        }
                    }
                    else
                    {
                        success = false;
                    }
                }

                return success;
            }
        }

        public Csharp()
        {
            Syntax = new SyntaxImpl();
        }

        public string Prefix => "c#-link";
        public MarkdownExtensionName Name => throw new NotImplementedException();
        public IElementType Type => ElementType.Inline;
        public Output Output => Output.Html;

        public ISyntax Syntax { get; }
        public IValidator Validator => null;
        public IFormatter Formatter { get; }
    }
}
