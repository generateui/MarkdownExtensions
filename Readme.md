# MarkdownExtensions
Standardized extensions on top of Markdown

## Features
MarkdownExtensions builds on top of [Markdig](https://github.com/lunet-io/markdig). It offers  and extension library and extension implementations on the Markdown syntax. It has the following features:
- include Javascript in resulting html
- include CSS in resulting html
- transform Markdown extension syntax into Markdown
- perform validation of the extension syntax
- optionally show syntax and model errors in generated html

## Extensions
MarkdownExtensions currently provides the following extensions:
- **FolderFromDisk**: specify a folder on disk, render the folder contents
- **FolderList**: Specify a folder structure with a nested list syntax and render as folder contents
- **KeyboardKeys**: Write a keyboard combination and see the keyboard keys nicely rendered
- **MarkdownLinks**: Link to a relative markdown file
- **Snippet**: Refer to a heading in a target markdown file and render the contents of the heading paragraph
- **TableOfContent**: Render a table of content on the left of the document in a `<nav>` html element
- **XmlSnippet**: Refer using an XPath expression of a target xml file and render the resulting xml in a codeblock

The following extensions are in development:
- **Note**: Start the paragraph with `Note:` and render a yellow box with paragraph contents
- **PanZoomImage**: Render an image within a box and having panning & zooming on that image
- **MsSql**: Refer to a target table and render the table (queried from the database) as html
- **GitHistory**: Render the history of a target git repo as a table
- **Excel**: Render the contents of an excel sheet as a table
- **EnterpriseArchitect**: Refer to object within an [Enterprise Architect](https://www.sparxsystems.eu/enterprise-architect/ea-purchase/) project and render it as html
- **BpmnGraph**: Render target bpmn file of graph
- **GitGraph**: Render target git repo history (using a custom syntax)

Go here to see examples.

## Standardized extensions
MarkdownExtensions allows easy implementation of block extensions:
````
```extension-name
internal extension syntax
```
````

As well as easy implementation of inline extensions:
`[extension-name:internal extension syntax]`
