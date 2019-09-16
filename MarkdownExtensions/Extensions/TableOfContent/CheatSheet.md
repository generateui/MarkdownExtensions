# Table of Content
Shows a table with all headings. Each heading links to the heading within the document.

## Configuration
Each level can be configured to use a CSS numbering style. The syntax is as follows:
```
Level{x}: {NumberingStyle}
```

- `{x}` is the level (levels 1, 2, 3, 4, 5 and 6 are supported).
- `NumberingStyle` is a [CSS list-style-type value](https://developer.mozilla.org/en-US/docs/Web/CSS/list-style-type#Values). `disc`, `circle`, `square`, `disclosure-open` and `disclosure-closed` are not supported.

### Example
```toc
Level1: Decimal
Level2: Upper-Roman
Level3: Lower-latin
Level4: Decimal-Leading-Zero
Level5: Lower-Greek
Level6: Simp-Chinese-Formal
```