using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GraphRag.Core.Models;
using GraphRag.Core.Interfaces;

namespace GraphRag.AI.Services;

public sealed class LlmReranker : ILlmReranker
{
    private readonly IChatClient _chatClient;


    public LlmReranker(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web)
            {
                PropertyNameCaseInsensitive = true,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,

                // Required when Microsoft.Extensions.AI generates the JSON schema.
                TypeInfoResolver = RerankerAnalysisJsonContext.Default
            };

    public async Task<RerankerAnalysis> ReRankAsync(
        string query, string document,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentException.ThrowIfNullOrWhiteSpace(document);

        string prompt = GetPrompt(query, document);

        ChatResponse<RerankerAnalysis> response =
            await _chatClient.GetResponseAsync<RerankerAnalysis>(
                prompt,
                JsonOptions,
                options: new ChatOptions
                {
                    Temperature = 0
                },
                useJsonSchemaResponseFormat: true,
                cancellationToken: cancellationToken);

        RerankerAnalysis result = response.Result
            ?? throw new InvalidOperationException(
                "The model did not return a structured result.");

        Validate(result);

        return result;
    }

    private string GetPrompt(string query, string document)
    {
        return $$"""
            You are an expert at evaluating document relevance. 
            Your task is to determine how relevant a document is for answering a specific query.

            Query: {{query}}
            Document: {{document}}

            Analyze the document and determine its relevance to the query.
            Consider:
            1. How directly the document answers the query
            2. The quality and specificity of the information provided
            3. The semantic relationship between the query and document content

            Provide your response as a JSON object with the following structure:
            {
              "relevance_score": <number between 0.0 and 1.0>,
              "explanation": "<brief explanation of the relevance score>"
            }

            The relevance_score should be:
            - 0.0-0.2: Not relevant or completely off-topic
            - 0.2-0.4: Somewhat relevant but lacks specificity
            - 0.4-0.6: Moderately relevant with some useful information
            - 0.6-0.8: Highly relevant with good information
            - 0.8-1.0: Extremely relevant and directly answers the query
            """;
    }


    private static void Validate(RerankerAnalysis result)
    {
        //if (string.IsNullOrWhiteSpace(result.Description))
        //{
        //    throw new InvalidDataException("Description must not be empty.");
        //}

        if (result.RelevanceScore is < 0 or > 1)
        {
            throw new InvalidDataException(
                "Relevance score must be between 0 and 1.");
        }
    }

}
