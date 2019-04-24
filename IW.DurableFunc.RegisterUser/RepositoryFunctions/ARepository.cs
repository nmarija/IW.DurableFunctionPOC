using IW.DurableFunctions.Data.DbTypes;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IW.DurableFunc.RegisterUser.RegisterUserFunctions
{
    public static class ARepository
    {
        [FunctionName("UsernameExistA")]
        public static bool UsernameExist([ActivityTrigger]  string username,
            [CosmosDB(
            databaseName: "Adb",
            collectionName: "Users",
            ConnectionStringSetting = "CosmosDBConnection",
            SqlQuery = "select * from c where c.username = {username}")] IEnumerable<User> res)
        {
            return res.Any();
        }

        [FunctionName("CreateUserA")]
        public static async Task<bool> CreateUserAsync([ActivityTrigger]  User user,
            [CosmosDB(
            databaseName: "Adb",
            collectionName: "Users",
            ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<object> users)
        {
            await users.AddAsync(user);
            return true;
        }

        [FunctionName("DeleteUserA")]
        public static async Task<bool> DeleteUser([ActivityTrigger]  string id,
            [CosmosDB(
            ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client)
        {
            
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("Adb", "Users");
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return false;
            }
            await client.DeleteDocumentAsync(document.SelfLink);
            return true;
        }

        [FunctionName("GetIdA")]
        public static string GetId([ActivityTrigger]  string username,
            [CosmosDB(
            databaseName: "Adb",
            collectionName: "Users",
            ConnectionStringSetting = "CosmosDBConnection",
            SqlQuery = "select * from c where c.username = {username}")] IEnumerable<User> res)
        {
            return res.ToList().FirstOrDefault().id;
        }
    }
}
