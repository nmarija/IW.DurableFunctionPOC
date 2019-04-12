using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IW.DurableFunc.RegisterUser.OktaFunctions
{
    public static class CreateUserOktaFunction
    {
        [FunctionName("CreateUserInOkta")]
        public static async Task<bool> CreateUserInOkta([ActivityTrigger] DurableActivityContext inputs, ILogger log)
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
