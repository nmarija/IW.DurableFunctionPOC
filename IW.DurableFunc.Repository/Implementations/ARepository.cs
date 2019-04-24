using IW.DurableFunctions.Data.DbTypes;
using System;
using System.Threading.Tasks;

namespace IW.DurableFunc.Repository.Implementations
{
    public class ARepository : IRepository
    {
        public Task<bool> CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetId(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateUsername(string username)
        {
            throw new NotImplementedException();
        }
    }
}
