using ITSM.ApiService.Contracts;

namespace ITSM.ApiService.Client.Tests.Generated
{
    [TestClass]
    public sealed class ITSM_ApiService_Client_Tests
    {
        protected readonly string _baseUrl;
        protected readonly System.Net.Http.HttpClient _httpClient;

        public ITSM_ApiService_Client_Tests()
        {
            _baseUrl = "http://localhost:5385";
            _httpClient = new System.Net.Http.HttpClient();
        }

        [TestMethod]
        public async Task TestCategoriesAllAsync()
        {
            IClient client = new Client(_baseUrl, _httpClient);

            ICollection<CategoryResponse> result = await client.CategoriesAllAsync();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestCountAsync()
        {
            IClient client = new Client(_baseUrl, _httpClient);

            int result = await client.CountAsync();

            Assert.IsTrue(result >= 0);
        }
    }
}
