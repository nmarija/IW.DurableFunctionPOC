using IW.DurableFunctions.Data.DbTypes;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository
{
    public interface IRepository
    {
        Task<bool> ValidateUsername(string username);

        Task<bool> CreateUserAsync(User user);

        Task<bool> DeleteUserAsync(string id);

        Task<string> GetId(string username);
    }
}
