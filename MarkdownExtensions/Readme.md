# MarkdownExtensions
Standardized extensions on top of Markdown

## Standardized extensions
Many markdown extensions provide customized parsers for basically any pattern. Think emojis, link detectors for ${BugTracker}, specialized tables et cetera. While these are nice and solve problems, it's helpful to start with an extension using a standardized syntax. From there, a more sophisticated syntax can be impemented/developed, when it is proven the extension is useful.

The library proposed the following "standardized" extension syntax:
Blocks:
<code>
```syntax-name
{InternalSyntax}
```
</code>
Inlines:
`[syntax-name:{InternalSyntax}]`

`syntax-name` is a prefix used to detect the extension. `InternalSyntax` is used by the extension to specify something that needs to be rendered.

A simple example is the FolderList block extension, which uses the following syntax:
<code>
```
- [folder]
	- [subfolder]
	- file
	- another_file.txt
```
</code>
rendering a folder-like tree. 

A simple inline example is the MsSql extension, rendering a table from a database:
`[ms-sql-table:Test:Table1]`

## Ease of implementation
Implementors therefore can focus first and foremost on the internal syntax. In the case of the MsSql extension, it may be that the implementer wants to add rendering hints. The implementer may then decide to move to a block extension, extend the syntax with a hint that relations/indices should not be rendered:
<code>
```ms-sql-table
database: Test
table: Table1
show-indexes: false
show-relations: false
```
