using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

/*
 What I would actually store in Chroma
   
   I wouldn't store only:
   .NET 10
   
   I'd create an embedding document: 
   Type: Technology
   Name: .NET 10
   Aliases: DotNet 10, Microsoft .NET 10
   Description: Cross-platform runtime and development platform...
   
   Then metadata:
   {
       "nodeId": 1842,
       "type": "Technology",
       "name": ".NET 10"
   }
   
   The embedding represents the meaning of the entity, 
   while metadata gives you deterministic information.
 */

namespace GraphSearch.Library.Models
{
    public record DocumentForChromaDB(int nodeId, string Type, string Name, IEnumerable<string> Aliases, string Description)
    {
        public string GetDocumentText()
        {
            var aliasesText = string.Join(", ", Aliases);
            return $"Type: {Type}\nName: {Name}\nAliases: {aliasesText}\nDescription: {Description}";
        }

        public Dictionary<string, object> GetMetadata()
        {
            return new Dictionary<string, object>
            {
                { "nodeId", nodeId },
                { "type", Type },
                { "name", Name }
            };
        }
    }
}
