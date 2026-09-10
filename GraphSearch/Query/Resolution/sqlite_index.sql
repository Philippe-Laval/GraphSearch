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