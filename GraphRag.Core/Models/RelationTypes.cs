using System;
using System.Collections.Generic;
using System.Text;

/*
Control the relation vocabulary

A major issue is that the LLM may generate equivalent relation names:

USES
UTILIZES
IS_USING
EMPLOYS
BASED_ON
*/

namespace GraphRag.Core.Models;

/// <summary>
/// Define a controlled vocabulary
/// </summary>
public static class RelationTypes
{
    /// <summary>
    /// Represents an "IS_A" relationship constant.
    /// </summary>
    public const string IsA = "IS_A";
    /// <summary>
    /// Represents a "part-of" relationship constant.
    /// </summary>
    public const string PartOf = "PART_OF";
    /// <summary>
    /// Represents a "uses" relationship constant.
    /// </summary>
    public const string Uses = "USES";
    /// <summary>
    /// Represents a "depends-on" relationship constant.
    /// </summary>
    public const string DependsOn = "DEPENDS_ON";
    /// <summary>
    /// Represents an "implements" relationship constant.
    /// </summary>
    public const string Implements = "IMPLEMENTS";
    /// <summary>
    /// Represents a "produces" relationship constant.
    /// </summary>
    public const string Produces = "PRODUCES";
    /// <summary>
    /// Represents a "causes" relationship constant.
    /// </summary>
    public const string Causes = "CAUSES";
    /// <summary>
    /// Represent a "created by" relationship constant.
    /// </summary>
    public const string CreatedBy = "CREATED_BY";
    /// <summary>
    /// Represent a "located in" relation constant.
    /// </summary>
    public const string LocatedIn = "LOCATED_IN";
    /// <summary>
    /// Represent a "related to" relation constant.
    /// </summary>
    public const string RelatedTo = "RELATED_TO";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.Ordinal)
        {
            IsA,
            PartOf,
            Uses,
            DependsOn,
            Implements,
            Produces,
            Causes,
            CreatedBy,
            LocatedIn,
            RelatedTo
        };
}
