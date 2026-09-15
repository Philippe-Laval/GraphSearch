using GraphRag.Ontology.Itsm.Core;

namespace GraphRag.Ontology.Itsm.Business.Classification;

/// <summary>
/// Abstract class : Any qualitative labelling scheme applied to tickets.
/// Priority, Impact, Urgency, Severity are deliberately modelled as distinct
/// concepts even though they may share the same underlying column values.
/// </summary>
public abstract class ClassificationConcept : BusinessConcept;
