# Include markdown snippets from other documents

## Named reference

### `>` As child
Insert target heading with content recursively as child of current heading

`[md-part:>loremipsum.md:Aliquam]`

> [md-part:>loremipsum.md:Aliquam]

### `=` As sibling
Insert target heading with content recursively as sibling of current heading

`[md-part:=loremipsum.md:Aliquam]`

> [md-part:=loremipsum.md:Aliquam]

```markdown-snippet:
file = "loremipsum.md"
heading = "## Aliquam"
```

## Hierarchy reference
```markdown-snippet:
file = "loremipsum.md"
heading = "2.1"
```
