# Include markdown snippets from other documents

## Named reference

### `>` As child
Insert target heading with content recursively as child of current heading

`[md-part:>Snippet.md:Header 1]`

```md-snippet
>Extensions\Snippet\Snippet.md:Header 1
```

### `=` As sibling
Insert target heading with content recursively as sibling of current heading

`[md-part:=Snippet.md:Header 1]`

> [md-part:=Snippet.md:Header 1]

```md-snippet
=Extensions\Snippet\Snippet.md:Header 1
```

```markdown-snippet:
file = "Snippet.md"
heading = "## Header 1"
```

## Hierarchy reference
```markdown-snippet:
file = "Snippet.md"
heading = "2.1"
```
