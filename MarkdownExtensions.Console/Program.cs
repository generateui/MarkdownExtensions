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
using MarkdownExtensions.Extensions.MarkdownLink;
using MarkdownExtension.PanZoomImage;
using MarkdownExtension.KeyboardKeys;
using MarkdownExtension.Excel;
using MarkdownExtension.MsSql;
using MarkdownExtension.GitHistory;
using MarkdownExtension.GitGraph;
using System.Linq;
using MarkdownExtension.EnterpriseArchitect.WorkflowNotes;
using MarkdownExtension.BpmnGraph;
using MarkdownExtension.EnterpriseArchitect.Diagram;
using MarkdownExtension.EnterpriseArchitect.TableNotes;
using MarkdownExtension.EnterpriseArchitect.ObjectText;
using MarkdownExtension.EnterpriseArchitect.DatamodelApi;
using MarkdownExtensions.Extensions.TableOfContent;
using MarkdownExtensions.Extensions.Note;
using MarkdownExtensions.Extensions.XmlSnippet;
using Markdig.SyntaxHighlighting;

namespace MarkdownExtensions.Console
{
    class Program
    {
        static void Main(string[] args)
        {
			var container = new Container();
			var scope = new ThreadScopedLifestyle();
			container.Options.DefaultScopedLifestyle = scope;
			Ea.Plugin.Register(container);
			container.Collection.Register<IMarkdownExtension>(
				typeof(FolderFromDiskExtension),
				typeof(SnippetExtension),
				typeof(PanZoomImageExtension),
				typeof(ExcelTableExtension),
				typeof(MsSqlTableExtension),
				typeof(GitHistoryExtension),
				typeof(GitGraphExtension),
				typeof(DatamodelApiExtension),
				typeof(WorkflowNotesExtension),
				typeof(BpmnGraphExtension),
				typeof(DiagramImageExtension),
				typeof(ObjectTextExtension),
				typeof(TableNotesExtension),
				typeof(TableOfContentExtension),
				typeof(NoteExtension),
				typeof(XmlSnippetExtension),
				typeof(NestedBlockExtension)
			);
			container.Collection.Register<IExtensionInfo>(
				typeof(FolderListExtensionInfo),
				typeof(FolderFromDiskExtensionInfo),
				typeof(MsSqlTableExtensionInfo),
				typeof(PanZoomImageExtensionInfo),
				//typeof(GitGraphExtensionInfo),
				typeof(GitHistoryExtensionInfo),
				typeof(TableNotesExtensionInfo),
				typeof(KeyboardKeysExtensionInfo),
				typeof(ExcelTableExtensionInfo),
				typeof(WorkflowNotesExtensionInfo),
				typeof(ObjectTextExtensionInfo),
				typeof(BpmnGraphExtensionInfo),
				typeof(SnippetExtensionInfo)
			);

			// Arguments:
			// - selfcontained (embed all into single html)

			System.Console.WriteLine("Marking down...");
            if (args.Length > 0)
            {
				string path = args[0];
				if (System.IO.File.Exists(path))
				{
					var directory = Path.GetDirectoryName(path);
					var sourceFolder = new AbsoluteFolder(directory);
					var renderSettings = RenderSettings.DefaultFile(sourceFolder);
					renderSettings.EnsureFoldersExist();
					renderSettings.TryParseSettingsFile();
					container.RegisterInstance(renderSettings);
					var extensionSettings = container.GetAllInstances<IExtensionSettings>();
					renderSettings.TryParseExtensionSettings(extensionSettings);

					File(path, container, null);
				}
				if (System.IO.Directory.Exists(path))
				{
					Directory(path, container);
				}
			}
            else
            {
                AggregateCheatSheet(container);
            }
        }


		private static MarkdownPipeline CreatePipeline(Container container)
		{
			var pipelineBuilder = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.UsePipeTables()
				.UseGridTables()
				.UseSyntaxHighlighting();

			var folderFromDiskExtension = container.GetInstance<FolderFromDiskExtension>();
			pipelineBuilder.Extensions.Add(folderFromDiskExtension);

			pipelineBuilder.Extensions.AddIfNotAlready<NestedBlockExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<FolderListExtension>();

			var snippetExtension = container.GetInstance<SnippetExtension>();
			pipelineBuilder.Extensions.Add(snippetExtension);

			pipelineBuilder.Extensions.AddIfNotAlready<PanZoomImageExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<KeyboardKeysExtension>(); //interferes with autolinks
			pipelineBuilder.Extensions.AddIfNotAlready<ExcelTableExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<MsSqlTableExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<GitHistoryExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<GitGraphExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<BpmnGraphExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<MarkdownLinkExtension>();
			pipelineBuilder.Extensions.AddIfNotAlready<TableOfContentExtension>();
			//pipelineBuilder.Extensions.AddIfNotAlready<NoteExtension>();

			var xmlSnippetExtension = container.GetInstance<XmlSnippetExtension>();
			pipelineBuilder.Extensions.Add(xmlSnippetExtension);

			var datamodelApiExtension = container.GetInstance<DatamodelApiExtension>();
			pipelineBuilder.Extensions.Add(datamodelApiExtension);

			var workflowNotesExtension = container.GetInstance<WorkflowNotesExtension>();
			pipelineBuilder.Extensions.Add(workflowNotesExtension);

			var tableNotesExtension = container.GetInstance<TableNotesExtension>();
			pipelineBuilder.Extensions.Add(tableNotesExtension);

			var diagramImageExtension = container.GetInstance<DiagramImageExtension>();
			pipelineBuilder.Extensions.Add(diagramImageExtension);

			var objectTextExtension = container.GetInstance<ObjectTextExtension>();
			pipelineBuilder.Extensions.Add(objectTextExtension);

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
			renderer.RegisterBlock<BpmnGraphBlock, BpmnGraphExtension>();
			renderer.RegisterBlock<DiagramBlock, DiagramImageExtension>();
			renderer.RegisterBlock<TableNotesBlock, TableNotesExtension>();
			renderer.RegisterBlock<ObjectTextBlock, ObjectTextExtension>();
			renderer.RegisterBlock<DatamodelApiBlock, DatamodelApiExtension>();
			renderer.RegisterBlock<TableOfContentBlock, TableOfContentExtension>();
			//renderer.RegisterBlock<NoteParagraphBlock, NoteExtension>();
			renderer.RegisterBlock<XmlSnippetBlock, XmlSnippetExtension>();

			renderer.RegisterInline<KeyboardKeysInline, KeyboardKeysExtension>();
		}

		private static void Directory(string path, Container container)
		{
			var sourceFolder = new AbsoluteFolder(path);
			var renderSettings = RenderSettings.DefaultWiki(sourceFolder);
			renderSettings.EnsureFoldersExist();
			renderSettings.TryParseSettingsFile();
            container.RegisterInstance(renderSettings);
            var extensionSettings = container.GetAllInstances<IExtensionSettings>();
			renderSettings.TryParseExtensionSettings(extensionSettings);

			var markdownFiles = System.IO.Directory
				.GetFiles(path, "*.md", SearchOption.AllDirectories)
				.Where(f => !f.ToLower().EndsWith("normalized.md"));
			foreach (var file in markdownFiles)
			{
				File(file, container, renderSettings);
			}
			// 1. pickup markdown file
			// 2. generate html in /rendered
			// 3. write images to /images
			// 4. write css to /css
			// 5. write js to /javascript
			// 6. write extension caches to /{ExtensionName}
			// 7. write markdown to /rendered
		}

		private static void File(string fullFilePath, Container container, RenderSettings settings)
        {
            string body = null;
            string summaries = null;
			string cssHeader = null;
			string jsHeader = null;
			var sourceFolder = new AbsoluteFolder(Path.GetDirectoryName(fullFilePath));
			if (settings == null)
			{
				settings = RenderSettings.DefaultWiki(sourceFolder);
			}

			using (var scope = ThreadScopedLifestyle.BeginScope(container))
			using (var writer = new StringWriter())
            {
				MarkdownPipeline pipeline = CreatePipeline(container);
				var markdown = System.IO.File.ReadAllText(fullFilePath);
				var markdownDocument = Markdown.Parse(markdown, pipeline);
				var renderer = new ExtensionHtmlRenderer(writer, markdownDocument, settings, pipeline);
				renderer.RegisterDynamicCss(new Code("markdown-extensions", "0.0.1", () =>
					Assembly.GetExecutingAssembly().GetFileContent("vscode-markdown.css")));

				pipeline.Setup(renderer);
				RegisterBlocks(renderer);

				renderer.Parse(container);
				renderer.Validate(container);
				renderer.Transform();
				renderer.Render(markdownDocument);
				var fileName = Path.GetFileName(fullFilePath);
				renderer.RenderMarkdown(fileName, markdownDocument);
				writer.Flush();
				body = writer.ToString();

				using (var summaryWriter = new StringWriter())
				{
					// a bit of a hack to use different writer for summaries
					renderer.Writer = summaryWriter;
					renderer.RenderSummaries(markdownDocument);
					summaryWriter.Flush();
					summaries = summaryWriter.ToString();
				}

				cssHeader = renderer.RenderCssHeader();
				jsHeader = renderer.RenderJavascriptHeader();
			}
			var document = $@"
				<html>
					<head>
						{jsHeader}
						{cssHeader}
					</head>
					<body>
						{summaries}
						<main>
						{body}
						</main>
					</body>
				</html>";
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullFilePath);
            var file = new File(settings.OutputFolder, fileNameWithoutExtension + ".html");
            System.IO.File.WriteAllText(file.AbsolutePath, document);
        }

        private static void AggregateCheatSheet(Container container)
        {
			Dictionary<string, string> cheatSheetByExtensionName = container
				.GetAllInstances<IExtensionInfo>()
				.ToDictionary(x => x.Name, x => x.CheatSheet);
            var sb = new StringBuilder();
            int i = 1;
            var body = new StringBuilder();
			string cssHeader = null;
			string javascriptHeader = null;
			using (ThreadScopedLifestyle.BeginScope(container))
			{
				foreach (var entry in cheatSheetByExtensionName)
				{
					var markdown = entry.Value;
					var writer = new StringWriter();
					var pipeline = CreatePipeline(container);
					var document = Markdown.Parse(markdown, pipeline);
					var renderer = new ExtensionHtmlRenderer(writer, document, new RenderSettings(), pipeline);
					pipeline.Setup(renderer);
					RegisterBlocks(renderer);
					renderer.Parse(container);
					// validate errors
					renderer.Transform();
					// run validations
					renderer.Render(document);
					writer.Flush();

					cssHeader = renderer.RenderCssHeader();
					javascriptHeader = renderer.RenderJavascriptHeader();

					string isChecked = i == 1 ? "checked" : "";
					string name = entry.Key;
					string md = entry.Value;
					string html = writer.ToString();

					var tab = $@"
						<section>
							<input type='radio' name='sections' id='option{i}' {isChecked}>
							<label for='option{i}'>{name}</label>
							<article>{html}</article>
						</section>";
					body.AppendLine(tab);
					i += 1;
				}
			}
            var htmlDocument = $@"
				<html>
					<head>
						{javascriptHeader}
						{cssHeader}
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
