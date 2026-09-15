using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Entities;
using GraphRag.Conceptual.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphRag.Conceptual.Tool
{
    public class ConceptualDbSeeder : IConceptualDbSeeder
    {
        private readonly ILogger<ConceptualDbSeeder> _logger;
        private readonly OntologyDbContext _dbContext;

        public ConceptualDbSeeder(OntologyDbContext dbContext, ILogger<ConceptualDbSeeder> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Seed(CancellationToken token = default)
        {
            _logger.LogInformation("Seeding the database...");

            Concept? concept;

            if (!_dbContext.Terms.Any())
            {
                ConceptService conceptService = new ConceptService(_dbContext);
                concept = await conceptService.CreateAsync(new Concept
                {
                    Uri = "http://metaline.fr/concepts/ticket",
                    PreferredLabel = "Ticket",
                    SynonymsInline = "Ticket",
                    Kind = ConceptKind.Ticket,
                    Domain = "ITSM",
                    IsDeprecated = false,
                    Description = "This is a default concept created during seeding."
                }, token);
            }
            else
            {
                concept = await _dbContext.Concepts.FirstOrDefaultAsync(c => c.Uri == "http://metaline.fr/concepts/ticket", token);
            }

            if (!_dbContext.Terms.Any())
            {
                TermService termService = new TermService(_dbContext);
                await termService.CreateAsync(new Term { 
                    SurfaceForm = "Ticket",
                    ConceptId = concept!.Id,
                    Concept = concept,
                    Language = "en",
                    Uri = "http://metaline.fr/terms/ticket",
                    Kind = Ontology.Itsm.Linguistic.TermKind.PreferredLabel

                }, token);
            }

            // Add your seeding logic here, for example:
            // if (!_dbContext.Categories.Any())
            // {
            //     _dbContext.Categories.Add(new Category { Name = "Default Category" });
            //     _dbContext.SaveChanges();
            // }
            _logger.LogInformation("Database seeding completed.");
        }
    }
}
