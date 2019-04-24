using IW.DurableFunctions.Data.DbTypes;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IW.DurableFunc.RegisterUser.RepositoryFunctions
{
    public static class BRepository
    {
        [FunctionName("UsernameExistB")]
        public static bool UsernameExist(
               [ActivityTrigger]  string username,
               [CosmosDB(
                    databaseName: "Bdb",
                    collectionName: "Users",
                    ConnectionStringSetting = "CosmosDBConnection",
                    SqlQuery = "select * from c where c.username = {username}")] IEnumerable<IW.DurableFunctions.Data.DbTypes.User> res)
        {
            return res.Any();
        }

        [FunctionName("CreateUserB")]
        public static async Task<bool> CreateUserAsync([ActivityTrigger]  User user,
            [CosmosDB(
            databaseName: "Bdb",
            collectionName: "Users",
            ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<object> users)
        {
            await users.AddAsync(user);
            return true;
        }

        [FunctionName("DeleteUserB")]
        public static async Task<bool> DeleteUser([ActivityTrigger]  string id,
            [CosmosDB(
            ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("Bdb", "Users");
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return false;
            }
            await client.DeleteDocumentAsync(document.SelfLink);
            return true;
        }

        [FunctionName("GetIdB")]
        public static string GetId([ActivityTrigger]  string username,
        [CosmosDB(
            databaseName: "Bdb",
            collectionName: "Users",
            ConnectionStringSetting = "CosmosDBConnection",
            SqlQuery = "select * from c where c.username = {username}")] IEnumerable<User> res)
        {
            return res.ToList().FirstOrDefault().id;
        }
    }
}
