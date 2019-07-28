using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.RequirementsTable
{
	public class RequirementsTableRenderer : BlockRendererBase<RequirementsTableName, RequirementsTableBlock>
	{
		private IEaProvider _provider;

		public RequirementsTableRenderer(IEaProvider provider)
		{
			_provider = provider;
		}

		public override void Render(ExtensionHtmlRenderer renderer, RequirementsTableName table, IFormatState formatState)
		{
			var package = _provider.GetElementsByPackage(new Path(table.Name));
			if (package != null)
			{
				var sb = new StringBuilder();
				Format(package, sb, formatState.HeadingLevel);
				renderer.Write(sb.ToString());
			}
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
	}
}
