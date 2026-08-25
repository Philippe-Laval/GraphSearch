CREATE TABLE GraphEntity
(
Id          INTEGER PRIMARY KEY,
Name        TEXT NOT NULL,
Type        TEXT NOT NULL,
Description TEXT
);

CREATE TABLE GraphEntityAlias
(
EntityId    INTEGER NOT NULL,
Alias       TEXT NOT NULL,

    PRIMARY KEY (EntityId, Alias),

    FOREIGN KEY (EntityId)
        REFERENCES GraphEntity(Id)
);

CREATE INDEX IX_GraphEntity_Name
ON GraphEntity(Name);

CREATE INDEX IX_GraphEntity_Type
ON GraphEntity(Type);

CREATE INDEX IX_GraphEntityAlias_Alias
ON GraphEntityAlias(Alias);

For example:

GraphEntity

Id      Name          Type
---------------------------------
1       Microsoft     Organization
2       .NET          Technology
3       .NET 10       Technology
4       C#            Language

and:

GraphEntityAlias

EntityId    Alias
-------------------------
2           DotNet
2           .NET Framework
3           DotNet 10
4           C Sharp


--------------

SQL + FTS5

For a large graph, create:

CREATE VIRTUAL TABLE GraphEntitySearch
USING fts5(
Name,
Type,
Description,
EntityId UNINDEXED
);

Then index:

.NET 10
Technology
Microsoft .NET 10 runtime...
1842

You can also put aliases into the searchable content.

Then your search becomes much more suitable for entity resolution.

For example:

SELECT
EntityId,
Name,
Type,
bm25(GraphEntitySearch) AS Score
FROM GraphEntitySearch
WHERE GraphEntitySearch MATCH $query
ORDER BY Score
LIMIT $limit;

Because SQLite's BM25 score is lower-is-better, you'd normalize it before returning it from IEntityIndex.