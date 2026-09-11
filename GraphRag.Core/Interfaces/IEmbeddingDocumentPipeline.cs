using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphRag.Core.Interfaces;

public interface IEmbeddingDocumentPipeline
{
    Task<string> BuildAsync(SubGraph2 graph, CancellationToken cancellationToken);
}
