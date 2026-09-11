using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphRag.Core.Interfaces;

public interface ICommunityDetector
{
    Task<IReadOnlyCollection<GraphCommunity>> DetectAsync(
        IKnowledgeGraph graph, 
        CancellationToken cancellationToken);
}
