using IW.DurableFunc.Repository;
using IW.DurableFunctions.Contracts.Requests;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IW.DurableFunc.RegisterUser.RegisterUserFunctions
{
    public static class DbFunctions
    {
        public static IDictionary<string, IRepository> repositories;

        [FunctionName("ValidateUsername")]
        public static async Task<bool> ValidateUsername([ActivityTrigger] DurableActivityContext inputs, ILogger log)
        {
            RegisterUserRequest request = inputs.GetInput<RegisterUserRequest>();
            repositories = new Dictionary<string, IRepository>
            {
                { "A", new ARepository() },
                { "B", new BRepository() },
                { "C", new CRepository() }
            };
            var tasks = new Task<bool>[repositories.Count];
            int i = 0;

            foreach (string key in repositories.Keys)
            {
                tasks[i++] = repositories[key].ValidateUsername(request.Username, log);
            }

            var res = await Task.WhenAll(tasks);

            return res.All(x => true);
        }

        [FunctionName("CreateUserInDB")]
        public static async Task<bool> CreateUserInDB([ActivityTrigger] DurableActivityContext inputs, ILogger log)
        {
            int rnd = new Random().Next(50);

            await Task.Delay(1000);

            if (rnd % 2 != 0)
            {
                throw new Exception();
            }               
            else
            {
                return true;
            }
        }

        [FunctionName("DeleteUserFromDB")]
        public static async Task<bool> DeleteUserFromDB([ActivityTrigger] DurableActivityContext inputs, ILogger log)
        {
            int rnd = new Random().Next(50);

            await Task.Delay(1000);

            if (rnd % 2 != 0)
            {
                throw new Exception();
            }
            else
            {
                return true;
            }
        }
    }
}
