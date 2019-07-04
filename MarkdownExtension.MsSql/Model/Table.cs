using System.Collections.Generic;

namespace MarkdownExtension.MsSql.Model
{
    // https://dataedo.com/kb/query/sql-server/most-used-data-type-in-the-database
    class Table
    {
        internal List<Field> Fields { get; set; } = new List<Field>();
        internal PrimaryKey PrimaryKey { get; set; }
        internal List<Index> Indexes { get; set; } = new List<Index>();
        internal List<ForeignKey> ForeignKeys { get; set; } = new List<ForeignKey>();
    }
    class PrimaryKey
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
    class ForeignKey
    {
        public string Name { get; set; }
        public string SchemaName { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        //public string Type { get; set; }
    }
    class Index
    {
        public string Name { get; set; }
        public string ColumnNames { get; set; }
        public string IndexType { get; set; }
    }
    class Field
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsNullable { get; set; }
        public int MaxLength { get; set; }
        public bool IsMax { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
        public virtual string ToText()
        {
            return $@"{Name}({Type})";
        }
    }
}
