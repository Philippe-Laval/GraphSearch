using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

public interface IEmbeddingDocumentRenderer
{
    string Format { get; }

    string Render<T>(EmbeddingDocument<T> document);
}