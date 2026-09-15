using GraphRag.Lucene.Data;
using GraphRag.Lucene.Interfaces;
using GraphRag.Lucene.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GraphRag.Lucene;

public sealed class MarkdownIngestor
{
    private readonly string _rootPath;
    private readonly IChunkRepository _repository;

    /// <summary>
    /// Creates a new instance of the MarkdownIngestor class.
    /// </summary>
    /// <param name="rootPath">The root path containing Markdown files to ingest.</param>
    /// <param name="repository">The repository to store ingested chunks.</param>
    public MarkdownIngestor(
        string rootPath,
        IChunkRepository repository)
    {
        _rootPath = Path.GetFullPath(rootPath);
        _repository = repository;
    }

    /// <summary>
    /// Ingest all Markdown files under the root path into the repository.
    /// </summary>
    /// <returns>The number of files that were changed or added.</returns>
    public int IngestAll()
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            ReturnSpecialDirectories = false
        };

        var changed = 0;

        // Directory.EnumerateFiles streams filenames rather than first
        // constructing an array of every filename, making it appropriate
        // for recursively walking larger trees
        foreach (var path in Directory.EnumerateFiles(
                     _rootPath,
                     "*.md",
                     options))
        {
            if (IngestFile(path))
            {
                changed++;
            }
        }

        return changed;
    }

    /// <summary>
    /// Ingest a single Markdown file into the repository. 
    /// If the file has not changed since the last ingestion, it will be skipped.
    /// 
    /// New chunks use Id: 0 and DocumentId: 0 until SQLite persists them
    /// </summary>
    /// <param name="path">The path of the Markdown file to ingest.</param>
    /// <returns>True if the file was ingested; false if it was skipped.</returns>
    private bool IngestFile(string path)
    {
        var relativePath = NormalizeRelativePath(
            Path.GetRelativePath(_rootPath, path));

        var markdown = File.ReadAllText(path);
        var contentHash = ComputeSha256(markdown);

        // If the content hash matches the existing document, skip processing
        if (_repository.IsCurrent(
                relativePath,
                contentHash))
        {
            return false;
        }

        var existing =
            _repository.FindDocumentByPath(relativePath);

        var source = new SourceDocument(
            Id: existing?.Id ?? 0,
            RelativePath: relativePath,
            ContentHash: contentHash,
            LastWriteTimeUtc:
                File.GetLastWriteTimeUtc(path),
            Metadata: new Dictionary<string, string>
            {
                ["extension"] = ".md",
                ["file_name"] = Path.GetFileName(path)
            });

        var textChunks = MarkdownChunker.Chunk(
            markdown,
            maximumCharacters: 2_000,
            overlapCharacters: 200);

        var chunks = textChunks
            .Select((text, ordinal) =>
                new ChunkDocument(
                    Id: 0,
                    DocumentId: source.Id,
                    Text: text,
                    EntityIds: [],
                    Metadata:
                        new Dictionary<string, string>
                        {
                            ["ordinal"] =
                                ordinal.ToString(
                                    System.Globalization
                                        .CultureInfo
                                        .InvariantCulture),

                            ["relative_path"] =
                                relativePath
                        }))
            .ToArray();

        _repository.ReplaceDocument(
            source,
            chunks);

        return true;
    }

    /// <summary>
    /// Normalizes a relative path by converting all directory separators 
    /// to forward slashes and removing leading slashes.
    /// </summary>
    /// <param name="path">The relative path to normalize.</param>
    /// <returns>The normalized relative path with forward slashes as separators and no leading slash.</returns>
    private static string NormalizeRelativePath(string path)
    {
        return path
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/')
            .TrimStart('/');
    }

    /// <summary>
    /// Compute a Sha256 hash of a string and return it as a hexadecimal string.
    /// </summary>
    /// <param name="value">The string to hash.</param>
    /// <returns>The hexadecimal representation of the SHA256 hash.</returns>
    private static string ComputeSha256(string value)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(bytes);
    }
}
