using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramValidator : ValidatorBase<Diagram>
	{
		private readonly IEaProvider _provider;
		private readonly RenderSettings _renderSettings;

		public DiagramValidator(IEaProvider provider, RenderSettings renderSettings)
		{
			_provider = provider;
			_renderSettings = renderSettings;
		}
		public override IErrors ValidateTyped(Diagram tree, ValidationContext context)
		{
			if (tree.Path != null)
			{
				var path = new Path(tree.Path);
				var folder = _renderSettings.GetExtensionFolder(FileNames.ENTERPRISE_ARCHITECT);
				bool diagramExists = _provider.IsValidDiagramPath(path, folder);
				if (!diagramExists)
				{
					var error = new Error($@"Enterprise Architect diagram with path [{path}] does not exist");
					return new ValidationFailure(error);
				}
			}
			if (tree.PackagePath != null)
			{
				var path = new Path(tree.PackagePath);
				var folder = _renderSettings.GetExtensionFolder(FileNames.ENTERPRISE_ARCHITECT);
				bool packageExists = _provider.IsValidPackagePath(path, folder);
				if (!packageExists)
				{
					var error = new Error($@"Enterprise Architect diagram with path [{path}] does not exist");
					return new ValidationFailure(error);
				}
			}
			return new Valid();
		}
	}
}
