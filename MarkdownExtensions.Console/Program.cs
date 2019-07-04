using SimpleInjector;
using System.IO;
using SimpleInjector.Lifestyles;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System;
using MarkdownExtensions.Extensions.Folder;
using MarkdownExtensions.Extensions.FolderFromDisk;
using MarkdownExtensions.Extensions.NestedBlock;
using MarkdownExtensions.Extensions.NestedInline;
using Ea = MarkdownExtension.EnterpriseArchitect;
using MarkdownExtension.GitGraph;
using MarkdownExtension.GitHistory;
using MarkdownExtension.PanZoomImage;
using MarkdownExtension.Snippet;
using MarkdownExtension.Excel;
using MarkdownExtension.KeyboardKeys;
using MarkdownExtension.MsSql;

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
            container.Register<IMarkdownConverter, MarkdownExtensionConverter>(Lifestyle.Scoped);
            container.Register<Func<IMarkdownConverter>>(() => container.GetInstance<IMarkdownConverter>, Lifestyle.Scoped);
            container.Register<Ea.ObjectText>(scope);
            Ea.Plugin.Register(container);
            container.Collection.Register<IMarkdownExtension>(
                typeof(FolderFromDisk),
                typeof(Folder),
                typeof(GitGraph),
                typeof(GitHistory),
                typeof(Ea.ObjectText),
                typeof(Ea.DiagramImage),
                typeof(Ea.TableNotes),
                typeof(Ea.RequirementsTable),
                typeof(PanZoomImage),
                typeof(Snippet),
                typeof(MsSqlTable),
                typeof(NestedBlockExample),
                typeof(NestedInlineExample),
                typeof(ExcelTable),
                typeof(KeyboardKeys)
            );
            AggregateCheatSheet(container);

            using (ThreadScopedLifestyle.BeginScope(container))
            {
                //var converter = container.GetInstance<IMarkdownConverter>();
                //Folder(converter);
                //Ea(converter);
            }
        }

        private static void AggregateCheatSheet(Container container)
        {
            var files = Directory.EnumerateFiles(@"C:\Users\Ruud\Desktop\MarkdownExtensions", "CheatSheet.md", SearchOption.AllDirectories);
            var cheatSheetByExtensionName = new Dictionary<string, string>();
            foreach (var file in files)
            {
                var folder = Path.GetFileName(Path.GetDirectoryName(file));
                if (file.Contains("Debug"))
                {
                    continue;
                }
                //if (!folder.Contains("Nested"))
                //{
                //    continue;
                //}
                string name = folder.Replace("MarkdownExtension.", string.Empty);
                cheatSheetByExtensionName[name] = File.ReadAllText(file);
            }
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
                    var converter = container.GetInstance<IMarkdownConverter>();
                    string isChecked = i == 1 ? "checked" : "";
                    string name = entry.Key;
                    var md = entry.Value;
                    var html = converter.Convert(md);
                    csss.AppendCode(converter.GetCss());
                    scripts.AppendCode(converter.GetJs());

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
        <div class='tabordion'>
            {body}
        </div>
    </body>
</html>
";
            File.WriteAllText("CheatSheet.html", document);
        }
    }
}
