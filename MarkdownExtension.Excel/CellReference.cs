namespace MarkdownExtension.Excel
{
	public class CellReference
	{
		public int Column { get; private set; }
		public int Row { get; private set; }
		public static CellReference Parse(string value)
		{
			int column = 0;
			string row = "";
			foreach (var c in value.ToLower())
			{
				if (char.IsLetter(c))
				{
					// TODO: multiply
					column += c.ToAlphabetIndex();
				}
				if (char.IsNumber(c))
				{
					row += c;
				}
			}
			return new CellReference
			{
				Column = column,
				Row = int.Parse(row)
			};
		}
	}
}
