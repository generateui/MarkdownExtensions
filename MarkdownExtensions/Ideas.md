# Ideas

## Extensions
- Mermaid
- Links to any item in any program
	- MS Paint icon
	- MS Paint menu item
- XML content item using xpath
- blockdiag 
- plantuml
- C# code to plantuml: https://github.com/pierre3/PlantUmlClassDiagramGenerator\
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

## Engine
- ~~internal Markdown representation~~
	- ~~use pandoc to export to textile~~
- document settings
	- generate heading number i.e. `2.3.1`
- ~~release resources~~
- ~~errors~~
- ~~cheat sheet aggregation~~
- ~~example aggregation~~
- introduce resolve/validate phase
	1. parse
	2. resolve (Ast -> Object)
	3. format
- ~~when formatting, pass header level~~
- progress
- async
- recursive markdown
- parsing performance (single pass)
- performance measurement
- side-by-side generated docs: markdown | html | rendered html
- autocomplete discovery
- separated input & output element types

## The ARS T&TT usecase
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

### Complete sequence
1. Parse markdown. When encountering triple backtick or `[syntax:data]`, resolve markdown extension
2. Parse extension syntax (record parse errors)
3. Validate extension syntax (record validation errors)
4. Generate markdown for all found extensions
5. Stop when only markdown, html continue
6. Parse ExtensionSyntax (record parse errors)
7. validate ExtensionSyntax (record validation errors)
8. Generate html for all html extensions

Currently, step 1+2 are not combined (need to fork CommonMark.Net)

### TODO:
- ~~release resources~~
- ~~errors~~
- 
