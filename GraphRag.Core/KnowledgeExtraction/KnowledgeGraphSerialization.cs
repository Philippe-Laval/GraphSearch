using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.Core.KnowledgeExtraction
{
    public class KnowledgeGraphSerialization
    {
        public static string Serialize(KnowledgeGraph knowledgeGraph)
        {
            return System.Text.Json.JsonSerializer.Serialize(knowledgeGraph, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
        }

        public static KnowledgeGraph Deserialize(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<KnowledgeGraph>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("Deserialization failed.");
        }
    }
}
