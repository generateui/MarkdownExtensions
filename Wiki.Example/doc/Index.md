```toc
Level2: Decimal
Level3: Decimal
Level4: Decimal
Level5: Decimal
Level6: Decimal
```

# Index
This document is a showcase document of extensions implemented in MarkdownExtensions.

## Links
- [Introduction](Introduction.md)
- [License](License.md)
- [Enterprise Architect](EnterpriseArchitect.md)
- [Enterprise Architect datamodel API](EnterpriseArchitectDatamodelApi.md)
- [Errors](Errors.md)

Link to the [Links] header.

## Folder
### Folder using syntax
````
```folder
- [root folder]
	- [sub folder]
	- file1
	- file2
```
````
```folder
- [root folder]
	- [sub folder]
	- file1
	- file2
```
### Folder using a relative path on disk
````
```folder-from-disk
TestFolder
```
````
```folder-from-disk
TestFolder
```
### Folder using an absolute path on disk
````
```folder-from-disk
C:\Windows\Help\Windows
```
````
```folder-from-disk
C:\Windows\Help\Windows
```

## Snippets from other markdown files
### Snippet placed as sibling
````
```md-snippet
=SnippetExample.md:Header 1
```
```` 

```md-snippet
=SnippetExample.md:Header 1
```
### Snippet placed as child of current header
````
```md-snippet
>SnippetExample.md:Header 1
```
````
```md-snippet
>SnippetExample.md:Header 1
```

## Keyboard keys
- `[keys:ctrl+a]` [keys:ctrl+a] 
- `[keys:ctrl+alt+delete]` [keys:ctrl+alt+delete]
- `[keys:⌘+a]` [keys:⌘+a]

## Note
```
Note: a paragraph starting with the text "Note:" will be rendered as a note.
```

Note: a paragraph with a note in it will be rendered as a note.

## Xml snippet
Uses an xpath query on a target xml document to retrieve an xml snippet. This xml content is then rendered as an xml codeblock.

````
```xml-snippet
file: Example.xml
xpath: /books/book[1]
```
````

```xml-snippet
file: Example.xml
xpath: /books/book[1]
```