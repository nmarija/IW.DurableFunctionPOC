using IW.DurableFunctions.Contracts.Requests;
using IW.DurableFunctions.Contracts.Responses;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IW.DurableFunc.RegisterUser
{
    public static class StartRegistrationFunction
    {
        [FunctionName("RegisterUser")]
        public static async Task<RegisterUserResponse> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var req = context.GetInput<RegisterUserRequest>();

            if (req == null)
            {
                throw new ArgumentNullException(nameof(req), "Request required");
            }

            var isUnique = await context.CallSubOrchestratorAsync<bool>("ValidateUsername", req.Username);
            bool authorized = false;

            if (!isUnique)
            {
                return new RegisterUserResponse { Status = false };
            }
            if (isUnique)
            {
                var instanceId = Guid.NewGuid().ToString("N");
                context.SetCustomStatus(new
                {
                    InstanceId = instanceId,
                    Step = "SMSChallenge"
                });
                authorized = await context.CallSubOrchestratorAsync<bool>("SendSMSFunction", instanceId, req.PhoneNumber);
            }

            if (!authorized)
                return new RegisterUserResponse { Status = authorized };

            bool dbStatus = await context.CallSubOrchestratorAsync<bool>("CreateUserInDB", req); ;

            if (dbStatus)
            {
                context.SetCustomStatus(new
                {
                    context.InstanceId,
                    Step = "User created in DB"
                });

                bool oktaStatus = await context.CallActivityWithRetryAsync<bool>("CreateUserInOkta", new RetryOptions(TimeSpan.FromSeconds(5), 3), req);

                if (oktaStatus)
                {
                    context.SetCustomStatus(new
                    {
                        context.InstanceId,
                        Step = "User created in Okta"
                    });
                }
            }
            else
            {
                bool deleteFromDb = await context.CallSubOrchestratorAsync<bool>("DeleteUserFromDB", req.Username);
                context.SetCustomStatus(new
                {
                    context.InstanceId,
                    Step = "Failed to create user"
                });
            }

            await context.CallActivityAsync<SendGridMessage>("SendConfirmationEmailActivity", req.Email);

            return new RegisterUserResponse { Status = authorized };
        }
    }
}