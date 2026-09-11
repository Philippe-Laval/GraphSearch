using GraphRag.Core.Interfaces;

namespace GraphRag.Core.Models;

public sealed class RelationWeights: IRelationWeightProvider
{
    private readonly Dictionary<string, float> _weights =
        new()
        {
            ["Uses"] = 1.0f,
            ["DependsOn"] = 0.95f,
            ["Implements"] = 0.90f,
            ["Owns"] = 0.85f,
            ["CreatedBy"] = 0.75f,
            ["LocatedIn"] = 0.40f
        };

    public float GetWeight(string relation)
        => _weights.GetValueOrDefault(relation, 0.5f);
}

public sealed class DefaultRelationWeightProvider : IRelationWeightProvider
{
    private readonly Dictionary<string, float> _weights = new()
    {
        ["Uses"] = 1.0f,
        ["DependsOn"] = 0.95f,
        ["Implements"] = 0.90f,
        ["Calls"] = 0.80f,
        ["Owns"] = 0.70f,
        ["LocatedIn"] = 0.20f
    };

    public float GetWeight(string relationType)
        => _weights.GetValueOrDefault(relationType, 0.5f);
}