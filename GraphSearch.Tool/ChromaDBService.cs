using Chroma;
using ChromaDB.Library;
using GraphSearch.Library.Embeddings;
using GraphSearch.Library.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Tool
{
    public class ChromaDBService
    {
        protected readonly ChromaDBClient _chromaDBClient;
        protected readonly string _tenant;
        protected readonly string _database;
        protected readonly string _collectionName;
        protected readonly IEmbeddingService _embeddingService;

        public ChromaDBService(ChromaDBClient chromaDBClient, string tenant, string database, string collectionName,
            IEmbeddingService embeddingService)
        {
            _chromaDBClient = chromaDBClient;
            _tenant = tenant;
            _database = database;
            _collectionName = collectionName;
            _embeddingService = embeddingService;
        }

        /// <summary>
        /// Code to initialize the ChromaDB database
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
        {
            var tenant = await _chromaDBClient.GetTenantAsync(_tenant, cancellationToken);
            if (tenant == null)
            {
                tenant = await _chromaDBClient.CreateTenantAsync(_tenant, cancellationToken);
            }

            if (tenant == null)
            {
                throw new Exception($"Failed to create or retrieve tenant '{_tenant}'.");
            }

            var databases = await _chromaDBClient.ListDatabasesAsync(_tenant, cancellationToken);
            if (!databases.Any(db => db.DatabaseName == _database))
            {
                await _chromaDBClient.CreateDatabaseAsync(_tenant, _database, cancellationToken);
            }

            databases = await _chromaDBClient.ListDatabasesAsync(_tenant, cancellationToken);
            var database = databases.FirstOrDefault(db => db.DatabaseName == _database);
            
            if (database == null)
            {
                throw new Exception($"Failed to create or retrieve database '{_database}'.");
            }

            var collections = await _chromaDBClient.ListCollectionsAsync(_tenant, _database, cancellationToken);
            if (!collections.Any(col => col.CollectionName == _collectionName))
            {
                // Be sure to use the correct space type for your embeddings.
                // Here we use Space.Cosine for GraphRAG since we are interested in cosine similarity
                // (same direction between embedding vectors).
                await _chromaDBClient.GetOrCreateCollection(_tenant, _database, _collectionName, 
                    collectionConfiguration : ChromaDBClient.SetupCollectionConfiguration(Space.Cosine),
                    collectionMetadata : new Chroma.HashMap {
                        AdditionalProperties = new Dictionary<string, object>
                        { 
                            { "description", "Collection for GraphSearch Tool" } 
                        } 
                    },
                    cancellationToken);
            }

            collections = await _chromaDBClient.ListCollectionsAsync(_tenant, _database, cancellationToken);
            var collection = collections.FirstOrDefault(col => col.CollectionName == _collectionName);

            if (collection == null)
            {
                throw new Exception($"Failed to create or retrieve collection '{_collectionName}'.");
            }

            Console.WriteLine("ChromaDB initialized successfully.");
        }

        public async Task AddDocumentAsync(DocumentForChromaDB document, CancellationToken cancellationToken)
        {
            var ids = new List<string> { document.nodeId.ToString() };

            // Include all fields in the result, but you can choose to include only the fields you need.
            var include = new List<Include> { Include.Documents,
                    Include.Embeddings,
                    Include.Distances,
                    Include.Metadatas,
                    Include.Uris };

            string documentText = document.GetDocumentText();
            var documents = new List<string?> { documentText };
            var uris = new List<string?> { null };

            var embeddings1 = await _embeddingService.EmbedAsync(documentText, cancellationToken);

            IList<IList<float>> embeddings = new List<IList<float>>
            {
                new List<float>(embeddings1.ToArray()),
            };

            Dictionary<string, object> meta1 = document.GetMetadata();

            IList<IDictionary<string, object>> metadatas = new List<IDictionary<string, object>>
            {
                meta1
            };

            // Add the document to the collection
            await _chromaDBClient.CollectionAddAsync(_tenant, _database, _collectionName,
                ids, embeddings, documents,
                uris, metadatas, cancellationToken);
        }

    }
}
