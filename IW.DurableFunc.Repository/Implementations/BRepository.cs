using IW.DurableFunctions.Data.DbTypes;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public class BRepository : IRepository
    {
        private DocumentClient _client;
        private string endpoint = "https://iwtestcosmosdb.documents.azure.com:443/";
        private string masterKey = "txFGtqt1wgxgfXw0smhfFetpGoeJYu2i8yAP7BepfAqRtrHQ82zuEkmD4JKxtvV13mNlKubffyoeNqmXFNA80A==";

        public BRepository()
        {            
            _client = new DocumentClient(new Uri(endpoint), masterKey);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            var document1 = await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri("Adb", "Users"),
                user);

            return true;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentCollectionUri("Adb", "Users"));
            return false;
        }

        public Task<string> GetId(string username)
        {
            var response = _client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri("Adb", "Users"), $"select * from c where c.username = {username}"
                ).ToList();

            var document = response.FirstOrDefault();

            return document.id;
        }

        public Task<bool> ValidateUsername(string username)
        {
            var response = _client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri("Adb", "Users"), $"select * from c where c.username = {username}"
                ).ToList();

            var document = response.FirstOrDefault();

            return document != null;
        }
    }
}
