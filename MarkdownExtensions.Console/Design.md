# Design

Html is the lingua franca of the digital world. Markdown is the humanly-writeable counterpart of html. Never before in history has there been a way for humans to write html easily. We've had WordPerfect and Word, enabling authoring of documents easily. However, these software packages are centered on creating paper documents. There has not been a tool to write html documents with the ease of use users are used to.

Writing html itself is not an option for users. Power-users rarely author an html document from start to finish. Looking at html itself, it becomes clear why: the content to mechanics-ratio is simply to high. The angle-bracketness is too high.

Writing text documents is what users *are* used to. Not only offer word processors this, the main text authoring happens on the web these days, where DSLs to author formatted content are widespread. WYSIWYM is happening. Today. On a massive scale. 

## Design
Inline extensions use the `[tag-prefix:data]` syntax. Block extension use the 

<code>
```tag-prefix:
data
```
</code>

syntax.

