
ab12 [master] commit one
ab13 commit two
ab14 #tag# create tag @"ruud"<rtimon@gmail.com>
ab12 -> ab21 [branch]
ab22 [branch] commit 2
ab22 -> ab14

-allow 1 or ab12ef45 style commit ids
-#tag#
- -> move to branch

branch? (branch|merge -> otherBranch) id? tag? title? @"author-name"<author-email>?

tag = #tag name|style# <- only once is needed
branch = [branch name|style] <- only once is needed
id = guid-start (4-8 characters)

