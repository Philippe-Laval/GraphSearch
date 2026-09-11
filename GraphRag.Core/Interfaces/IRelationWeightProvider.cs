using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.Interfaces
{
    public interface IRelationWeightProvider
    {
        float GetWeight(string relationType);
    }
}
