PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;

CREATE TABLE IF NOT EXISTS documents
(
    id                  INTEGER PRIMARY KEY,
    relative_path       TEXT NOT NULL UNIQUE,
    content_hash        TEXT NOT NULL,
    last_write_time_utc TEXT NOT NULL,
    metadata_json       TEXT NULL,
    indexed_revision    INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS chunks
(
    id            INTEGER PRIMARY KEY,
    document_id   INTEGER NOT NULL,
    ordinal       INTEGER NOT NULL,
    text          TEXT NOT NULL,
    metadata_json TEXT NULL,

    FOREIGN KEY (document_id)
        REFERENCES documents(id)
        ON DELETE CASCADE,

    UNIQUE(document_id, ordinal)
);

CREATE INDEX IF NOT EXISTS ix_chunks_document_id
    ON chunks(document_id);

CREATE TABLE IF NOT EXISTS chunk_entities
(
    chunk_id  INTEGER NOT NULL,
    entity_id TEXT NOT NULL,

    PRIMARY KEY(chunk_id, entity_id),

    FOREIGN KEY (chunk_id)
        REFERENCES chunks(id)
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_chunk_entities_entity_id
    ON chunk_entities(entity_id);

CREATE TABLE IF NOT EXISTS indexing_queue
(
    document_id INTEGER PRIMARY KEY,
    queued_utc  TEXT NOT NULL,

    FOREIGN KEY (document_id)
        REFERENCES documents(id)
        ON DELETE CASCADE
);