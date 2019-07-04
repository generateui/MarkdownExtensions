using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect
{
    public class RequirementsTable : IMarkdownExtension
    {
        private class RequirementsTableName
        {
            public string Name { get; set; }
        }
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new RequirementsTableName { Name = text.Trim() });
            }
        }

        private class FormatterImpl : IFormatter
        {
            private IEaProvider _provider;

            public FormatterImpl(IEaProvider provider)
            {
                _provider = provider;
            }

            public FormatResult Format(object root, IFormatState state)
            {
                var table = root as RequirementsTableName;
                var package = _provider.GetElementsByPackage(new Path(table.Name));
                if (package != null)
                {
                    var sb = new StringBuilder();
                    Format(package, sb, state.HeadingLevel);
                    return FormatResult.FromHtml(sb.ToString());
                }
                return null;
            }

            private void Format(Package package, StringBuilder sb, int level = 1)
            {
                sb.AppendLine($@"<h{level}>{package.Name}</h{level}>");
                foreach (var p in package.Packages)
                {
                    var childPackage = p as Package;
                    Format(childPackage, sb, level + 1);
                }
                sb.AppendLine($@"<table>");
                sb.AppendLine($@"<tbody>");
                foreach (var child in package.Elements)
                {
                    var element = child as Element;
                    if (element.Stereotype.Contains("Requirement"))
                    {
                        sb.AppendLine($@"<tr><td>{element.Name}</td><td>{element.Notes}</td></tr>");
                    }
                }
                sb.AppendLine($@"</tbody>");
                sb.AppendLine($@"</table>");
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        public RequirementsTable(IEaProvider provider)
        {
            Syntax = new SyntaxImpl();
            Formatter = new FormatterImpl(provider);
        }

        public string Prefix => "ea-requirements-table";
        public IElementType Type => ElementType.Block;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "EA requirements in a table";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
}
