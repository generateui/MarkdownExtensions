# Ideas

## Extensions
- diagrams
	- Mermaid
	- plantuml
		- C# code to plantuml: https://github.com/pierre3/PlantUmlClassDiagramGenerator\
	- nomnoml
	- https://github.com/asciidoctor/asciidoctor-diagram
	- blockdiag
- Links to any item in any program
	- MS Paint icon
	- MS Paint menu item
- ~~XML content item using xpath~~
- Excel
	- Excel vanilla table
		- Just import contents of target worksheet range as table
	- Excel tree table
	- Excel graph as image
	- Random thoughts
		- conditional formatting to css
	- cell content
		- [excel-cell-content:file.xslx:sheet1:a5]
- Include content from any website as quote
	- url#anchor link
	- markdown normalization: html to md
	- directives on how to acquire content
		- as child parapgraph
		- as quoted verbatim text
- C# code references
	- File content
	- refer to Class/enum/delegate/struct types, get source code
	- refer to Class/enum/delegate/struct types, get *link to* source code
	- include git hash, so it will work versioned
	- caching
- C# code queries
	- include git hash for versioning
		- possibly multiple, to show tabbed result per version
	- nDepend query?
- Slideshow of a certain set of pictures
- Before|After picture on-hover/drag component
- Sql
	- sql-table
		- prints table
	- sql
- Redmine
	- link to redmine ticket with status
		- [redmine:12345] becomes url with name of redmine ticket to redmine ticket
- Trello
	- link to trello card with status?
	- render trello card
- Unicode
	- refer to unicode codeblocks
- ToC
	- filter on h1..h6
	- resizable toc divider
	- placement right/left/top


## Validators
- image uri
	- checks if referenced image exists
- markdown file link 
	- checks if referenced markdown file exists
- wiki markdown file completeness
	- checks if every file in the wiki is used (no dead content)
- headings in a document with duplicate id

## Engine
- ~~internal Markdown representation~~
	- ~~use pandoc to export to textile~~
- document settings
	- generate heading number i.e. `2.3.1`
- ~~release resources~~
- ~~errors~~
- ~~cheat sheet aggregation~~
- ~~example aggregation~~
- ~~introduce resolve/validate phase~~
	~~1. parse~~
	~~2. resolve (Ast -> Object)~~
	~~3. format~~
- ~~when formatting, pass header level~~
- progress
- async
- recursive markdown
- performance optimization (single pass)
- performance measurement
- separated input & output element types
- services ouside C#/CLR providing extensions
	- require async support
	- require progress
	- require performance benchmarking
- correct lineno+characterno model
	- support source markdown
	- support second phase markdown
	- support DOM references
	- support sourcemaps

## Editor
- autocomplete discovery
- windows-alike emoji inserter but then for all unicode charactersm or maybe block-specific inserters

## UI
- side-by-side generated docs: markdown | html | rendered html

## The ARS T&TT usecase

### Iteration 3
Datamodel document
- per diagram the diagram and diagram notes under it
- per table the table header + table notes
	- per attribute the attribute as list item `**{attribute.Name}** {attribute.Notes}`
	- per relation to other table the relation and notes
	- list item normalization (insert notes as nested list item)
	- broken CommonMark.NET tables
- option to output document into folder with linked (instead of embedded images) content

Redmine update script
- markdown document
- specify items from EA datamodel
	- diagram image
	- per used table/enum/class in diagram like TableNotes
- specify items from requirement
- html must be converted back to markdown
- output in markdown for all required extensions

### Iteration 2
- SSS should be generated from EA
	- Write requirements in html
		- input:
			- package
			- template
			- settings
		- output: table/markdown/html
- EA content should be shared in Redmine
	- easy export to textile using pandoc
- analysis 2
	1. define SSS in Markdown
	2. produce markdown document from EA requirements
		- imageify unconvertable elements
			- layouted tables
			- 
	3. produce docx using pandoc
- analysis 3
	1. define SSS in markdown
	2. generate html document
	3. pandoc html document to docx

## intermediate Markdown design
- assumption: an extension only produces markdown
- process
	1. determine if any extensions exist on the document with to-markdown behavior
	2. if any, have them expand into markdown. Then add markdown to "ast"
	3. generate markdown or generate html
- assumption: CommonMark.Net is able to geenrate markdown from "ast"
- add recursive markdown as option?

### TODO:
- ~~release resources~~
- ~~errors~~
- newline handling
	- output to markdown
	- use
		- environment
		- \r\n
		- \n
- have cst parser

# Idea list
- https://thisdavej.com/build-an-amazing-markdown-editor-using-visual-studio-code-and-pandoc/
- notable

## Links
- https://rmarkdown.rstudio.com/

## Markdown-it as host renderer
1. markdown-it extension has list of inline and block extension prefixes
2. on rendering, when matching a prefix it asks for html
3. supplied html is passed onto markdown-it