using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace IW.DurableFunc.RegisterUser
{
    public static class SendSMSFunction
    {
        [FunctionName("SendSMSFunction")]
        public static async Task<bool> SendSMSOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            string phoneNumber = context.GetInput<string>();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException(
                nameof(phoneNumber),
                "A phone number input is required.");
            }
            int challengeCode = await context.CallActivityAsync<int>("SendSmsChallenge", phoneNumber);
            
            using (var timeoutCts = new CancellationTokenSource())
            {
                DateTime expiration = context.CurrentUtcDateTime.AddSeconds(180);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);
                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<int> challengeResponseTask = context.WaitForExternalEvent<int>("SmsChallengeResponse");                    
                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);

                    if (winner == challengeResponseTask)
                    {
                        if (challengeResponseTask.Result == challengeCode)
                        {
                            authorized = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (!timeoutTask.IsCompleted)
                {
                    timeoutCts.Cancel();
                }
                return authorized;
            }
        }

        [FunctionName("SendSmsChallenge")]
        public static int SendSmsChallenge(
        [ActivityTrigger] string phoneNumber,
        ILogger log,
        [TwilioSms(AccountSidSetting = "TwilioAccountSid", AuthTokenSetting = "TwilioAuthToken", From = "%TwilioPhoneNumber%")]
                      out CreateMessageOptions message)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            int challengeCode = rand.Next(10000);
            log.LogInformation($"Sending verification code {challengeCode} to {phoneNumber}.");

            message = new CreateMessageOptions(new PhoneNumber(phoneNumber));

            message.Body = $"Your verification code is {challengeCode:0000}";
            return challengeCode;
        }
    }
}