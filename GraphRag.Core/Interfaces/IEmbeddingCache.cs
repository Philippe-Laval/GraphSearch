using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

public interface IEmbeddingCache
{
    Task<bool> ExistsAsync(string hash);

    Task StoreAsync(string hash, ReadOnlyMemory<float> embedding);
}