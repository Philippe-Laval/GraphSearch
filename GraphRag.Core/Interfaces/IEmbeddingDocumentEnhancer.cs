using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphRag.Core.Interfaces;

public interface IEmbeddingDocumentEnhancer
{
    Task<string> EnhanceAsync(string yaml, CancellationToken cancellationToken = default);
}
