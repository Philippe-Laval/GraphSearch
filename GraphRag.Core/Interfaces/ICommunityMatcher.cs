using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces;

public interface ICommunityMatcher
{
    CommunityStability Compare(CommunityFingerprint previous, CommunityFingerprint current);
}