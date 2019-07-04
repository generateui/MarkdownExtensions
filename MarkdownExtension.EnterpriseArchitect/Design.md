# EA content in Markdown

## Features
- release resources

## Mapping the mdoel to syntax
On the one hand, there is object types in EA. On the other hand, are representations in html. 
EA object Examples:
- Requirement
- Diagram
- Table
- Table attribute
- Table relationship
Representation examples:
- link
- note content
- diagram image
- list (i.e. a package)

## Link
[ea-link:req_arch:Primary keys must be named Id]

## Note content
[ea-note:req_arch:Primary keys must be named Id]

## Diagram image
[ea-diagram:Starter Class Diagram]

## Table
[ea-table:Requirements package]
```
name|tag{release}|note
```

## Errors
- item does not exist
- multiple items exists with the same name