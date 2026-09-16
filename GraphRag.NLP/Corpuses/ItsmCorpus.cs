using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.NLP.Corpuses
{
    // Don't start by asking “what information can I extract?” Start with a constrained ontology
    // of ITSM terms and relations. Then ask “what information can I extract that fits into this ontology?”
    public class ItsmCorpus
    {
        public static readonly string[] ItsmTerms = new string[]
        {
            // Core ITSM entities
            "incident",
            "problem",
            "change",
            "change request",
            "service request",
            "request for change",
            "known error",
            "workaround",
            "root cause",
            "major incident",
            "outage",
            "disruption",
            "degradation",

            // Configuration & assets
            "configuration item",
            "CI",
            "CMDB",
            "asset",
            "service",
            "business service",
            "technical service",
            "service catalog",
            "service portfolio",

            // Roles & organization
            "user",
            "end user",
            "requester",
            "caller",
            "assignee",
            "agent",
            "technician",
            "service desk",
            "help desk",
            "support group",
            "assignment group",
            "change manager",
            "problem manager",
            "incident manager",
            "service owner",
            "process owner",
            "approver",

            // Lifecycle & workflow
            "ticket",
            "priority",
            "impact",
            "urgency",
            "severity",
            "status",
            "state",
            "category",
            "subcategory",
            "resolution",
            "resolution code",
            "closure code",
            "escalation",
            "reassignment",
            "approval",
            "CAB",
            "change advisory board",

            // SLA & performance
            "SLA",
            "service level agreement",
            "OLA",
            "operational level agreement",
            "underpinning contract",
            "KPI",
            "MTTR",
            "MTBF",
            "response time",
            "resolution time",
            "breach",

            // Change management
            "standard change",
            "normal change",
            "emergency change",
            "change window",
            "maintenance window",
            "rollback plan",
            "implementation plan",
            "risk assessment",

            // Release & deployment
            "release",
            "deployment",
            "release package",
            "rollout",

            // Availability & continuity
            "availability",
            "capacity",
            "continuity",
            "disaster recovery",
            "backup",
            "restore",

            // Knowledge & communication
            "knowledge base",
            "knowledge article",
            "FAQ",
            "notification",
            "announcement",

            // Frameworks
            "ITIL",
            "ITSM",
            "COBIT",
            "ISO 20000"
        };

        public static readonly string[] ItsmRelations = new string[]
        {
            // Incident/problem relations
            "caused by",
            "causes",
            "resolved by",
            "resolves",
            "related to",
            "duplicate of",
            "parent of",
            "child of",
            "linked to",
            "blocks",
            "blocked by",
            "depends on",
            "dependency of",

            // Assignment & ownership
            "assigned to",
            "reported by",
            "raised by",
            "requested by",
            "owned by",
            "managed by",
            "handled by",
            "escalated to",
            "reassigned to",
            "approved by",
            "rejected by",
            "reviewed by",

            // Configuration relations
            "part of",
            "component of",
            "contains",
            "hosted on",
            "hosts",
            "runs on",
            "installed on",
            "connected to",
            "uses",
            "used by",
            "provides",
            "provided by",
            "consumes",
            "consumed by",
            "supports",
            "supported by",

            // Impact relations
            "impacts",
            "impacted by",
            "affects",
            "affected by",

            // Change relations
            "implements",
            "implemented by",
            "triggered by",
            "triggers",
            "scheduled for",
            "rolled back to",
            "supersedes",
            "superseded by",

            // Knowledge relations
            "documented in",
            "references",
            "referenced by",

            // SLA relations
            "governed by",
            "covered by",
            "breached by",
            "meets",
            "violates",

            // Categorization
            "categorized as",
            "classified as",
            "belongs to"
        };
    }
}
