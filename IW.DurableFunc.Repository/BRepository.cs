using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public class BRepository : IRepository
    {
        public Task<bool> ValidateUsername(string validateUsername, ILogger log)
        {
            log.LogInformation("B start");
            Task.Delay(10000);
            log.LogInformation("B stop");
            return Task.FromResult(true);
        }
    }
}
