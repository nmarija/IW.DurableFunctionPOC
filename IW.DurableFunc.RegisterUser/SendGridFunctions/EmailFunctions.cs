using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;
using System;
using System.Text;

namespace IW.DurableFunc.RegisterUser.SendGridFunctions
{
    public static class EmailFunctions
    {
        [FunctionName(nameof(SendConfirmationEmailActivity))]
        public static void SendConfirmationEmailActivity([ActivityTrigger] string mailTo, [SendGrid(ApiKey = "sendGridApiKey")] out SendGridMessage message)
        {

            message = new SendGridMessage();
            message.AddTo(mailTo);
            message.SetSubject("User registration successfull");

            var htmlContent = new StringBuilder();
            htmlContent
                .AppendLine("<html>")
                .AppendLine($"<head><meta name=\"viewport\" content=\"width=device-width\" /><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /><title>Email confirmation</title>")
                .AppendLine("<body>")
                .AppendLine($"<p>Hello time.now = {DateTime.Now}</p>")
                .AppendLine("</body></html>");

            message.AddContent("text/html", htmlContent.ToString());
            message.SetFrom(new EmailAddress("marija.najdenoska@interworks.com.mk"));

        }
    }
}
