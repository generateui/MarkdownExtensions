# Design

MarkdownExtensions exposes an inline and block element API. The inline element follows the syntax of

> [{tag-name}:{parameters}]

and the block syntax of the form

> ```{tag-name}:
> {configuration-syntax}
> ```

```excel-table:
Sheet.xlsx:Sheet1:B2:C4]
```

When the input element is inline, the output html must be either inline or block. When the input element is block, the output must be html block.

The heart of an extension is defined in the [c#-link:IMakrdownExtension] interface. 