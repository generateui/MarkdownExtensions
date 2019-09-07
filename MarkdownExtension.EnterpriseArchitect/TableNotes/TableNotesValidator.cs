using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesValidator : ValidatorBase<TableNotes>
	{
		private readonly IEaProvider _provider;

		public TableNotesValidator(IEaProvider provider)
		{
			_provider = provider;
		}
		public override IErrors ValidateTyped(TableNotes tableNotes, RenderSettings renderSettings)
		{
			if (tableNotes.PackagePath != null)
			{
				IEnumerable<string> nameCollisions = _provider
					.GetElementsByPackage(tableNotes.PackagePath)
					.GetElementsRecursively()
					.Where(e => TableNotes.Include(tableNotes, e))
					.GroupBy(x => x.Name)
					.Where(g => g.Count() > 1)
					.Select(g => g.Key);
				if (nameCollisions.Count() > 0)
				{
					var errors = nameCollisions.Select(nc => new Error($@"Element named {nc} has multiple elements in package"));
					return new ValidationFailure(errors);
				}
			}
			return new Valid();
		}
	}
}
