using GraphRag.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

public interface ICommunityAnalyzer
{
    CommunityStatistics Analyze(GraphCommunity community);
}