using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

public interface IKnowledgeTextGenerationAgent
{
    Task<string> GenerateAsync(string instruction, CancellationToken cancellationToken);
}
