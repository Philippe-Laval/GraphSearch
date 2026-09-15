namespace GraphRag.Ontology.Itsm.Business.Metrics;

/// <summary>
/// Représente les fonctions d’agrégation disponibles pour calculer des valeurs statistiques sur un ensemble de données.
/// </summary>
/// <remarks>Inclut des agrégations standard (compte, somme, moyenne, minimum, maximum, médiane, percentile), des
/// variantes spécialisées comme le compte distinct et le ratio, ainsi qu’une option personnalisée.</remarks>
public enum AggregationFunction
{
    Count, 
    CountDistinct,
    Sum, 
    Avg,
    Min, 
    Max, 
    Median, 
    Percentile,
    Ratio, 
    Custom
}
