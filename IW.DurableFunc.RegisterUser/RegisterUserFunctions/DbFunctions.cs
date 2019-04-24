using IW.DurableFunctions.Contracts.Requests;
using IW.DurableFunctions.Data.DbTypes;
using Microsoft.Azure.WebJobs;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace IW.DurableFunc.RegisterUser.RegisterUserFunctions
{
    public static class DbFunctions
    {
        [FunctionName("ValidateUsername")]
        public static async Task<bool> ValidateUsername([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            string username = context.GetInput<string>();
           
            var tasks = new Task<bool>[2];
            tasks[0] = context.CallActivityWithRetryAsync<bool>("UsernameExistA", new RetryOptions(TimeSpan.FromSeconds(5), 3), username);
            tasks[1] = context.CallActivityWithRetryAsync<bool>("UsernameExistB", new RetryOptions(TimeSpan.FromSeconds(5), 3), username);

            var res = await Task.WhenAll(tasks);

            return res.All(x => !x);
        }

        [FunctionName("CreateUserInDB")]
        public static async Task<bool> CreateUserInDB([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            RegisterUserRequest request = context.GetInput<RegisterUserRequest>();
            var dbRequest = new User
            {
                firstName = request.FirstName,
                lastName = request.LastName,
                username = request.Username,
                email = request.Email,
                password = request.Password,
                phoneNumber = request.PhoneNumber
            };

            var tasks = new Task<bool>[2];
            tasks[0] = context.CallActivityWithRetryAsync<bool>("CreateUserA", new RetryOptions(TimeSpan.FromSeconds(5), 3), dbRequest);
            tasks[1] = context.CallActivityWithRetryAsync<bool>("CreateUserB", new RetryOptions(TimeSpan.FromSeconds(5), 3), dbRequest);

            var res = await Task.WhenAll(tasks);

            return res.All(x => x);
        }

        [FunctionName("DeleteUserFromDB")]
        public static async Task<bool> DeleteUserFromDB([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            string username = context.GetInput<string>();
            
            var idA = await context.CallActivityWithRetryAsync<string>("GetIdA", new RetryOptions(TimeSpan.FromSeconds(5), 3), username);
            var idB = await context.CallActivityWithRetryAsync<string>("GetIdB", new RetryOptions(TimeSpan.FromSeconds(5), 3), username);

            var tasks = new Task<bool>[2];
            tasks[0] = context.CallActivityWithRetryAsync<bool>("DeleteUserA", new RetryOptions(TimeSpan.FromSeconds(5), 3), idA);
            tasks[1] = context.CallActivityWithRetryAsync<bool>("DeleteUserB", new RetryOptions(TimeSpan.FromSeconds(5), 3), idB);

            var res = await Task.WhenAll(tasks);

            return res.All(x => x);
        }
    }
}
