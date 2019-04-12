using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public class ARepository : IRepository
    {
        public Task<bool> ValidateUsername(string validateUsername, ILogger log)
        {
            log.LogInformation("A start");
            Task.Delay(20000);
            log.LogInformation("A stop");
            return Task.FromResult(true);
        }
    }
}
