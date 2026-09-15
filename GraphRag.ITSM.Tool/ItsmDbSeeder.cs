using GraphRag.ITSM.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.ITSM.Tool
{
    public class ItsmDbSeeder : IItsmDbSeeder
    {
        private readonly ILogger<ItsmDbSeeder> _logger;
        private readonly ItsmDbContext _dbContext;

        public ItsmDbSeeder(ItsmDbContext dbContext, ILogger<ItsmDbSeeder> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Seed()
        {
            _logger.LogInformation("Seeding the database...");
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
