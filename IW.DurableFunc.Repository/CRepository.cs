using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public class CRepository : IRepository
    {
        public Task<bool> ValidateUsername(string validateUsername, ILogger log)
        {
            log.LogInformation("C start");
            Thread.Sleep(5000);
            log.LogInformation("C stop");
            return Task.FromResult(true);
        }
    }
}
