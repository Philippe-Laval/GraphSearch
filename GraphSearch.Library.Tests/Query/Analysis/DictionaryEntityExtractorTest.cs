using GraphSearch.Library.Query.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Tests.Query.Analysis
{
    [TestClass]
    public sealed class DictionaryEntityExtractorTest
    {
        [TestMethod]
        public async Task TestExtractAsync()
        {
            IEntityExtractor extractor = DictionaryEntityExtractorFactory.Create();

            IReadOnlyList<ExtractedEntity> result = await extractor.ExtractAsync("Microsoft developped C# and .NET 10 for Windows, MacOS and Linux");
            Assert.IsNotNull(result);
            Assert.HasCount(6, result);

            ExtractedEntity? extractedEntity = result.FirstOrDefault(e => e.Text == "Microsoft");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual("Microsoft", extractedEntity.Text);
            Assert.AreEqual("Organization", extractedEntity.Type);

            extractedEntity = result.FirstOrDefault(e => e.Text == "C#");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual("C#", extractedEntity.Text);
            Assert.AreEqual("ProgrammingLanguage", extractedEntity.Type);

            extractedEntity = result.FirstOrDefault(e => e.Text == ".NET 10");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual(".NET 10", extractedEntity.Text);
            Assert.AreEqual("Technology", extractedEntity.Type);

            extractedEntity = result.FirstOrDefault(e => e.Text == "Windows");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual("Windows", extractedEntity.Text);
            Assert.AreEqual("OperatingSystem", extractedEntity.Type);

            extractedEntity = result.FirstOrDefault(e => e.Text == "MacOS");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual("MacOS", extractedEntity.Text);
            Assert.AreEqual("OperatingSystem", extractedEntity.Type);

            extractedEntity = result.FirstOrDefault(e => e.Text == "Linux");
            Assert.IsNotNull(extractedEntity);
            Assert.AreEqual("Linux", extractedEntity.Text);
            Assert.AreEqual("OperatingSystem", extractedEntity.Type);
        }
    }
}
