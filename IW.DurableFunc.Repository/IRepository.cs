using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public interface IRepository
    {
        Task<bool> ValidateUsername(string validateUsername, ILogger log);
    }
}
