using MarkdownExtension.EnterpriseArchitect.EaProvider;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotes
	{
		public IEnumerable<Table> Tables { get; set; }
		public bool AllTables { get; set; }
		public Path PackagePath { get; set; }
		public static bool Include(TableNotes tnm, Element e)
		{
			if (e.Stereotype != "table")
			{
				return false;
			}
			if (tnm.AllTables)
			{
				return true;
			}
			return tnm.Tables.Any(t => Equals(t.Name, e.Name));
		}
	}
	public class Table
	{
		public string Name { get; set; }
	}
}
