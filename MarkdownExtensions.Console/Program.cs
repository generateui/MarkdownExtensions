﻿using SimpleInjector;
using System.IO;
using SimpleInjector.Lifestyles;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Markdig;
using Ea = MarkdownExtension.EnterpriseArchitect;
using MarkdownExtensions.Extensions.FolderFromDisk;
using MarkdownExtensions.Extensions.NestedBlock;
using MarkdownExtensions.Extensions.Snippet;
using MarkdownExtensions.Extensions.FolderList;
using MarkdownExtension.PanZoomImage;
using MarkdownExtension.KeyboardKeys;
using MarkdownExtension.Excel;
using MarkdownExtension.MsSql;
using MarkdownExtension.GitHistory;
using MarkdownExtension.GitGraph;
using System.Linq;
using MarkdownExtension.EnterpriseArchitect.WorkflowNotes;

namespace MarkdownExtensions.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var formatSettings = new FormatSettings
            {
                ForceRefreshData = false
            };
			var container = new Container();
			var scope = new ThreadScopedLifestyle();
			container.Options.DefaultScopedLifestyle = scope;
			container.RegisterInstance(formatSettings);
			Ea.Plugin.Register(container);
			container.Collection.Register<IMarkdownExtension>(
				typeof(FolderFromDiskExtension),
				typeof(SnippetExtension),
				typeof(PanZoomImageExtension),
				typeof(ExcelTableExtension),
				typeof(MsSqlTableExtension),
				typeof(GitHistoryExtension),
				typeof(GitGraphExtension),
				typeof(WorkflowNotesExtension),
				typeof(NestedBlockExtension)
			);
			container.Collection.Register<IExtensionInfo>(
				typeof(FolderListExtensionInfo),
				typeof(FolderFromDiskExtensionInfo),
				typeof(MsSqlTableExtensionInfo),
				typeof(PanZoomImageExtensionInfo),
				typeof(GitGraphExtensionInfo),
				typeof(GitHistoryExtensionInfo),
				typeof(KeyboardKeysExtensionInfo),
				typeof(ExcelTableExtensionInfo),
				typeof(WorkflowNotesExtensionInfo),
				typeof(SnippetExtensionInfo)
			);
			//typeof(Ea.ObjectText),
			//typeof(Ea.DiagramImage),
			//typeof(Ea.TableNotes),
			//typeof(Ea.RequirementsTable),

			System.Console.WriteLine("Marking down...");
            if (args.Length > 0)
            {
                File(args[0], null);
            }
            else
            {
                AggregateCheatSheet(container);
            }
        }

		private static MarkdownPipeline CreatePipeline(Container container)
		{
			var pipelineBuilder = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions();
			pipelineBuilder.Extensions.AddIfNotAlready<FolderFromDiskExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<NestedBlockExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<FolderListExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<SnippetExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<PanZoomImageExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<KeyboardKeysExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<ExcelTableExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<MsSqlTableExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<GitHistoryExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<GitGraphExtension>();
			var workflowNotesExtension = container.GetInstance<WorkflowNotesExtension>();
			pipelineBuilder.Extensions.Add(workflowNotesExtension);
			var pipeline = pipelineBuilder.Build();
			return pipeline;
		}
		private static void RegisterBlocks(ExtensionHtmlRenderer renderer)
		{
			renderer.RegisterBlock<FolderFromDiskBlock, FolderFromDiskExtension>();
			renderer.RegisterBlock<FolderListBlock, FolderListExtension>();
			renderer.RegisterBlock<NestedBlockBlock, NestedBlockExtension>();
			renderer.RegisterBlock<SnippetBlock, SnippetExtension>();
			renderer.RegisterBlock<PanZoomImageBlock, PanZoomImageExtension>();
			renderer.RegisterBlock<ExcelTableBlock, ExcelTableExtension>();
			renderer.RegisterBlock<MsSqlTableBlock, MsSqlTableExtension>();
			renderer.RegisterBlock<GitHistoryBlock, GitHistoryExtension>();
			renderer.RegisterBlock<GitGraphBlock, GitGraphExtension>();
			renderer.RegisterBlock<WorkflowNotesBlock, WorkflowNotesExtension>();

			renderer.RegisterInline<KeyboardKeysInline, KeyboardKeysExtension>();
		}

        private static void File(string fileName, Container container)
        {
            var scripts = new StringBuilder();
            var csss = new StringBuilder();
            csss.Append(Assembly.GetExecutingAssembly().GetFileContent("vscode-markdown.css"));
            string body = null;
            using (ThreadScopedLifestyle.BeginScope(container))
            {
				var writer = new StringWriter();
				var pipeline = CreatePipeline(container);
				var markdown = System.IO.File.ReadAllText(fileName);
				var markdownDocument = Markdown.Parse(markdown, pipeline);
				var renderer = new ExtensionHtmlRenderer(writer, markdownDocument);
				pipeline.Setup(renderer);
				RegisterBlocks(renderer);
				renderer.Parse(container);
				// validate errors
				renderer.Transform();
				// run validations
				renderer.Render(markdownDocument);
				var css = renderer.CollectCss();
				csss.Append(css);
				writer.Flush();
				var html = writer.ToString();
			}
			var document = $@"
				<html>
					<head>
						<link rel='stylesheet' type='text/css' href='Template.css'>
						<script type='text/javascript'>
							{scripts}
						</script>
						<style>
							{csss}
						</style>
					</head>
					<body>
						{body}
					</body>
				</html>";
            var folder = Path.GetDirectoryName(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var fullFilePath = Path.Combine(folder, $@"{fileNameWithoutExtension}.html");
            System.IO.File.WriteAllText(fullFilePath, document);
        }

        private static void AggregateCheatSheet(Container container)
        {
			Dictionary<string, string> cheatSheetByExtensionName = container
				.GetAllInstances<IExtensionInfo>()
				.ToDictionary(x => x.Name, x => x.CheatSheet);
            var sb = new StringBuilder();
            int i = 1;
            var scripts = new StringBuilder();
            var csss = new StringBuilder();
            csss.Append(Assembly.GetExecutingAssembly().GetFileContent("vscode-markdown.css"));
            var body = new StringBuilder();
            foreach (var entry in cheatSheetByExtensionName)
            {
				using (ThreadScopedLifestyle.BeginScope(container))
				{
					var markdown = entry.Value;
					var writer = new StringWriter();
					var pipeline = CreatePipeline(container);
					var document = Markdown.Parse(markdown, pipeline);
					var renderer = new ExtensionHtmlRenderer(writer, document);
					pipeline.Setup(renderer);
					RegisterBlocks(renderer);
					renderer.Parse(container);
					// validate errors
					renderer.Transform();
					// run validations
					renderer.Render(document);
					// do css + js collection
					writer.Flush();
					var css = renderer.CollectCss();
					csss.Append(css);
					var js = renderer.CollectJavascript();
					scripts.Append(js);

					string isChecked = i == 1 ? "checked" : "";
					string name = entry.Key;
					var md = entry.Value;
					var html = writer.ToString();

					var tab = $@"
						<section>
							<input type='radio' name='sections' id='option{i}' {isChecked}>
							<label for='option{i}'>{name}</label>
							<article>{html}</article>
						</section>";
					body.AppendLine(tab);
				}
                i += 1;
            }
            var htmlDocument = $@"
				<html>
					<head>
						<link rel='stylesheet' type='text/css' href='Template.css'>
						<script type='text/javascript'>
							{scripts}
						</script>
						<style>
							{csss}
						</style>
					</head>
					<body>
						<div class='tabordion'>
							{body}
						</div>
					</body>
				</html>";
            System.IO.File.WriteAllText("CheatSheet.html", htmlDocument);
        }
    }
}
