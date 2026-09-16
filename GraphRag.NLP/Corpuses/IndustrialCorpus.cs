using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace GraphRag.NLP.Models
{
    // Don't start by asking “what information can I extract?” Start with a constrained ontology
    // of industrial terms and relations. Then ask “what information can I extract that fits into this ontology?”
    public class IndustrialCorpus
    {
        public static readonly string[] IndustrialTerms = new string[]
        {
            "Equipment",
            "Component",
            "Material",
            "Process",
            "Person",
            "Organization",
            "Location",
            "Standard",
            "Document",
            "FailureMode"
        };

        public static readonly string[] IndustrialRelations = new string[]
        {
            "USES",
            "PART_OF",
            "DEPENDS_ON",
            "IMPLEMENTED_BY",
            "CAUSES",
            "PRODUCES",
            "RELATED_TO",
            "MANUFACTURED_BY",
            "LOCATED_AT",
            "USES_MATERIAL",
            "COMPLIES_WITH",
            "REPAIRED_BY",
        };

    }
}
